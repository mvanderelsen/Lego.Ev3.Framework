using Lego.Ev3.Framework.Core;
using Lego.Ev3.Framework.Devices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework.Firmware
{
    internal class Socket : ISocket, IDisposable
    {
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

        public void StartEventMonitor(Brick brick)
        {
            Dictionary<int, InputPort> inputPorts = brick.IOPort.Input.Ports;
            Buttons buttons = brick.Buttons;
            Battery battery = brick.Battery;
            BrickConsole console = brick.Console;

            int INTERVAL = Brick.Options.EventMonitor.Interval;

            Task task = Task.Factory.StartNew(async () =>
            {
                while (!_socket.CancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        //do not overload event message pump
                        if (_socket.EventBuffer.Count == 0)
                        {
                            byte[] batch = null;
                            ushort index = 0;

                            ushort buttonByteLength = 0;
                            ushort batteryByteLength = 0;
                            ushort warningByteLength = 0;

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

                                warningByteLength = console.BatchCommand(cb, index);
                                index += warningByteLength;

                                batch = cb.ToBytes();
                            }

                            //no need to send batch, it has no content.
                            if (batch.Length == 0)
                            {
                                try
                                {
                                    await Task.Delay(INTERVAL, _socket.CancellationToken);
                                }
                                catch (TaskCanceledException) { }
                                continue;
                            }

                            Command cmd = null;
                            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, index, 0, useEventId: true))
                            {
                                cb.Raw(batch);
                                cmd = cb.ToCommand();
                            }

                            Response response = await Brick.Socket.Execute(cmd, true);

                            if (response.Type == ResponseType.ERROR)
                            {
                                if (!_socket.CancellationToken.IsCancellationRequested) throw new FirmwareException(response);
                                else continue;
                            }

                            byte[] data = response.PayLoad;
                            if (data.Length != index)
                            {
                                if (!_socket.CancellationToken.IsCancellationRequested) throw new FirmwareException(response);
                                else continue;
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

                            if (warningByteLength > 0) 
                            {
                                console.SetValue(data[index]);
                                index += warningByteLength;
                            }
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
            }
        }

        public async Task<Response> Execute(Command command)
        {
            Response response = await Execute(command, false);
            if (response.Type == ResponseType.ERROR && _socket.IsConnected) throw new FirmwareException(response);
            if (response.Id != command.Id) throw new InvalidOperationException("response id does not match command id");
            return response;
        }

        private async Task<Response> Execute(Command command, bool isEvent = false)
        {
            switch (command.Type)
            {
                case CommandType.DIRECT_COMMAND_NO_REPLY:
                case CommandType.SYSTEM_COMMAND_NO_REPLY:
                    {
                        _socket.NoReplyCommandBuffer.Enqueue(command);
                        return await Task.Run(async () =>
                        {
                            int retry = 0;
                            ushort id = command.Id;
                            while (!_socket.CancellationToken.IsCancellationRequested && retry < 20)
                            {

                                if (_socket.ResponseBuffer.ContainsKey(id))
                                {
                                    _socket.ResponseBuffer.TryRemove(id, out _);
                                    return Response.Ok(id);
                                }
                                try
                                {
                                    await Task.Delay(50, _socket.CancellationToken);
                                }
                                catch (TaskCanceledException) { }
                                retry++;
                            }
                            return Response.Error(id);
                        });

                    }
                case CommandType.DIRECT_COMMAND_REPLY:
                case CommandType.SYSTEM_COMMAND_REPLY:
                    {

                        if (isEvent) _socket.EventBuffer.Enqueue(command);
                        else _socket.CommandBuffer.Enqueue(command);

                        return await Task.Run(async () =>
                        {
                            int retry = 0;
                            ushort id = command.Id;
                            while (!_socket.CancellationToken.IsCancellationRequested && retry < 50)
                            {

                                if (_socket.ResponseBuffer.ContainsKey(id))
                                {
                                    _socket.ResponseBuffer.TryGetValue(id, out byte[] payLoad);
                                    Response response = Response.FromPayLoad(payLoad);
                                    _socket.ResponseBuffer.TryRemove(id, out _);
                                    return response;
                                }

                                try
                                {
                                    await Task.Delay(100, _socket.CancellationToken);
                                }
                                catch (TaskCanceledException) { }
                                retry++;
                            }
                            return Response.Error(id);
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
