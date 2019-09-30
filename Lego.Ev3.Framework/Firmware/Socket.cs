using Lego.Ev3.Framework.Core;
using Lego.Ev3.Framework.Devices;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Lego.Ev3.Framework.Firmware
{
    internal class Socket : IDisposable
    {
        public ConcurrentDictionary<ushort, CommandHandle> Handles { get; } = new ConcurrentDictionary<ushort, CommandHandle>();

        private readonly Sockets.ISocket _socket;

        public string ConnectionInfo { get { return _socket.ConnectionInfo; } }

        public CancellationToken CancellationToken { get { return _socket.CancellationToken; } }
        public readonly SynchronizationContext SynchronizationContext = SynchronizationContext.Current;

        public bool IsConnected
        {
            get { return _socket.IsConnected; }
        }

        public Socket(Sockets.ISocket socket)
        {
            _socket = socket;
        }

        public async Task Connect()
        {
            await _socket.Connect();
        }

        public void StartEventMonitor(Dictionary<int, InputPort> inputPorts, Buttons buttons, Battery battery)
        {
            int INTERVAL = Brick.Options.EventMonitor.Interval;

            Task task = Task.Factory.StartNew(async () =>
            {
                while (!_socket.CancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        byte[] batch = null;
                        ushort index = 0;

                        ushort buttonByteLength = 0;
                        ushort batteryByteLength = 0;

                        Dictionary<InputPort, DataType> triggeredPorts = new Dictionary<InputPort, DataType>();
                        using (PayLoadBuilder cb = new PayLoadBuilder())
                        {
                            foreach (int key in inputPorts.Keys)
                            {
                                InputPort port = inputPorts[key];
                                if (port.Status != PortStatus.OK) continue; // no device connected so continue
                                InputDevice device = (InputDevice)port.Device;

                                if (!device.MonitorEvents) continue; // device will get value on manual poll

                                DataType type = device.BatchCommand(cb, index); // get batchcommand
                                if (type == DataType.NONE) continue;

                                index += type.ByteLength();
                                triggeredPorts.Add(port, type);
                            }


                            buttonByteLength = buttons.ClickBatchCommand(cb, index);
                            index += buttonByteLength;

                            batteryByteLength = battery.BatchCommand(cb, index);
                            index += batteryByteLength;

                            batch = cb.ToBytes();
                        }

                        //no need to send batch, it has no content.
                        if (triggeredPorts.Count == 0 && buttonByteLength == 0)
                        {
                            try
                            {
                                await Task.Delay(INTERVAL, _socket.CancellationToken);
                            }
                            catch (TaskCanceledException) { }
                            continue;
                        }

                        Command cmd = null;
                        using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, index, 0, useEventId:true))
                        {
                            cb.Raw(batch);
                            cmd = cb.ToCommand();
                        }

                        Response response = await Brick.Socket.Execute(cmd, true);

                        if (response.Type == ResponseType.ERROR)
                        {
                            try
                            {
                                await Task.Delay(INTERVAL, _socket.CancellationToken);
                            }
                            catch (TaskCanceledException) { }
                            continue;
                        }

                        byte[] data = response.PayLoad;
                        if (data.Length != index)
                        {
                            try
                            {
                                await Task.Delay(INTERVAL, _socket.CancellationToken);
                            }
                            catch (TaskCanceledException) { }
                            continue;
                        }

                        index = 0;
                        foreach (InputPort port in triggeredPorts.Keys)
                        {
                            InputDevice device = (InputDevice)port.Device;

                            bool hasChanged = false;
                            DataType type = triggeredPorts[port];
                            switch (type)
                            {
                                case DataType.DATA8:
                                    {
                                        hasChanged = device.SetDeviceValue(data[index]);
                                        break;
                                    }
                                case DataType.DATAF:
                                    {
                                        hasChanged = device.SetDeviceValue(BitConverter.ToSingle(data, index));
                                        break;
                                    }
                                case DataType.DATA32:
                                    {
                                        hasChanged = device.SetDeviceValue(BitConverter.ToInt32(data, index));
                                        break;
                                    }
                                case DataType.DATA16:
                                    {
                                        hasChanged = device.SetDeviceValue(BitConverter.ToInt16(data, index));
                                        break;
                                    }
                                case DataType.DATA_A4:
                                    {
                                        byte[] values = new byte[4];
                                        Array.Copy(data, index, values, 0, 4);
                                        hasChanged = device.SetDeviceValue(values);
                                        break;
                                    }
                            }

                            index += type.ByteLength();
                        }

                        if (buttonByteLength > 0)
                        {
                            byte[] buttonData = new byte[buttonByteLength];
                            Array.Copy(data, index, buttonData, 0, buttonByteLength);
                            buttons.ClickBatchCommandReturn(buttonData);
                            index += buttonByteLength;
                        }

                        if (batteryByteLength > 0)
                        {
                            byte[] batteryData = new byte[batteryByteLength];
                            Array.Copy(data, index, batteryData, 0, batteryByteLength);
                            battery.SetValue(batteryData);
                            index += batteryByteLength;
                        }
                    }
                    catch (Exception e)
                    {
                        Brick.Logger.LogError(e, e.Message);
                    }

                    try
                    {
                        await Task.Delay(INTERVAL, _socket.CancellationToken);
                    }
                    catch (TaskCanceledException) { }
                }

            }, _socket.CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);
        }

        public async Task Disconnect()
        {
            if (IsConnected)
            {
                await _socket.Disconnect();
                Handles.Clear();
            }
        }


        public async Task<Response> Execute(Command command)
        {
            return await Execute(command, false);
        }

        private async Task<Response> Execute(Command command, bool isEvent = false)
        {
            switch (command.Type)
            {
                case CommandType.DIRECT_COMMAND_NO_REPLY:
                case CommandType.SYSTEM_COMMAND_NO_REPLY:
                    {
                        _socket.NoReplyCommands.Enqueue(command);
                        return await Task.Run(async () =>
                        {
                            int retry = 0;
                            while (!_socket.CancellationToken.IsCancellationRequested && retry < 20)
                            {

                                foreach (ushort id in _socket.Responses.Keys)
                                {
                                    if (id == command.Id)
                                    {
                                        _socket.Responses.TryRemove(id, out _);
                                        return Response.Ok(command);
                                    }
                                }
                                try
                                {
                                    await Task.Delay(10, _socket.CancellationToken);
                                }
                                catch (TaskCanceledException) { }
                                retry++;
                            }
                            return Response.Error(command);
                        });

                    }
                case CommandType.DIRECT_COMMAND_REPLY:
                case CommandType.SYSTEM_COMMAND_REPLY:
                    {
                        Handles.TryAdd(command.Id, command);

                        if (isEvent) _socket.Events.Enqueue(command.PayLoad);
                        else _socket.Commands.Enqueue(command.PayLoad);


                        return await Task.Run(async() => 
                        {
                            int retry = 0;
                            while (!_socket.CancellationToken.IsCancellationRequested && retry < 20)
                            {
                                
                                foreach (ushort id in _socket.Responses.Keys)
                                {
                                    if (id == command.Id)
                                    {
                                        _socket.Responses.TryGetValue(id, out byte[] payLoad);
                                        Response response = Response.FromPayLoad(Handles[id], payLoad);
                                        Handles.TryRemove(id, out _);
                                        _socket.Responses.TryRemove(id, out _);
                                        return response;
                                    }
                                }

                                try
                                {
                                    await Task.Delay(10, _socket.CancellationToken);
                                }
                                catch (TaskCanceledException) { }
                                retry++;
                            }
                            Handles.TryRemove(command.Id, out _);
                            return Response.Error(command);
                        });
                    }
                default: throw new NotImplementedException(nameof(command.Type));
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _socket.Dispose();
                    Handles.Clear();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Socket()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
