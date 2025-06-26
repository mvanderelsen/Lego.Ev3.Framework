using Lego.Ev3.Framework.Core;
using Lego.Ev3.Framework.Devices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task Connect(CancellationToken cancellationToken = default)
        {
            await _socket.Connect(cancellationToken);
        }

        public void StartEventMonitor(Brick brick)
        {
            Dictionary<int, InputPort> inputPorts = brick.IOPort.Input.Ports;
            Buttons buttons = brick.Buttons;
            Battery battery = brick.Battery;
            BrickConsole console = brick.Console;

            int INTERVAL = Brick.Options.EventMonitor.Interval;

            try
            {
                Task task = Task.Factory.StartNew(async () =>
                {
                    while (!_socket.CancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            //do not overload event message pump
                            if (_socket.Events.IsEmpty)
                            {
                                byte[] batch = null;
                                ushort index = 0;

                                ushort buttonByteLength = 0;
                                ushort batteryByteLength = 0;
                                ushort warningByteLength = 0;

                                Dictionary<InputPort, DataType> triggeredPorts = [];
                                using (PayLoadBuilder cb = new())
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


                                    buttonByteLength = buttons.BatchCommand(cb, index);
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
                                    await Task.Delay(INTERVAL, _socket.CancellationToken);
                                    continue;
                                }

                                Command cmd = null;
                                using (CommandBuilder cb = new(CommandType.DIRECT_COMMAND_REPLY, index, 0, useEventId: true))
                                {
                                    cb.Raw(batch);
                                    cmd = cb.ToCommand();
                                }

                                Response response = await Brick.Socket.Execute(cmd, true);

                                if (response.Type == ResponseType.ERROR) continue;

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
                                    buttons.BatchCommandReturn(buttonData);
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

                        await Task.Delay(INTERVAL, _socket.CancellationToken);
                    }

                }, _socket.CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);
            }
            catch (TaskCanceledException) { }
        }

        public void Disconnect()
        {
            _socket.Disconnect();
        }

        public async Task<Response> Execute(Command command)
        {
            Response response;
            try
            {
                response = await Execute(command, false);
            }
            catch (TaskCanceledException)
            {
                response = Response.Ok(command.Id);
            }
            catch (Exception e) 
            {
                response = Response.Error(command.Id);
                Brick.Logger.LogWarning(e, "Package dropped command {id} {type}", command.Id, command.Type);
            }
            //if (response.Type == ResponseType.ERROR && _socket.IsConnected) throw new FirmwareException(response);
            //if (response.Id != command.Id) throw new InvalidOperationException("response id does not match command id");
            return response;
        }

        private async Task<Response> Execute(Command command, bool isEvent)
        {
            _socket.Enqueue(command, isEvent);

            int retry = 0;
            ushort id = command.Id;

            while (!_socket.CancellationToken.IsCancellationRequested && retry < 200)
            {
                while (!_socket.Commands.ContainsKey(id))
                {
                    await Task.Delay(10, _socket.CancellationToken);
                }

                if (_socket.Responses.ContainsKey(id))
                {
                    _socket.Commands.TryRemove(id, out _);
                    Response response;
                    if (command.NoReply)
                    {
                        response = Response.Ok(id);
                    }
                    else
                    {
                        _socket.Responses.TryGetValue(id, out byte[] payLoad);
                        response = Response.FromPayLoad(payLoad);

                    }
                    _socket.Responses.TryRemove(id, out _);
                    return response;
                }

                await Task.Delay(10, _socket.CancellationToken);
                retry++;
            }
            _socket.Commands.TryRemove(id, out _);
            return Response.Error(id);
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
