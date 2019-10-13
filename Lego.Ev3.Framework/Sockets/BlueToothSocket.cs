using Lego.Ev3.Framework.Firmware;
using System;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework.Sockets
{
    internal class BlueToothSocket : SocketBase, IDisposable, ISocket
    {

        private SerialPort _serialPort;
        private BinaryReader _reader;
        private CancellationTokenSource _cancellationTokenSource;
        public bool IsConnected => _serialPort != null && _serialPort.IsOpen;

        public CancellationToken CancellationToken { get; private set; }

        public override string ConnectionInfo => $"BlueTooth {_comPort}";


        private readonly string _comPort;
        public BlueToothSocket(string comPort) : base()
        {
            _comPort = comPort;
        }

        public Task<bool> Connect()
        {
            if (!IsConnected)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                CancellationToken = _cancellationTokenSource.Token;
                _serialPort = new SerialPort(_comPort, 115200);
                _serialPort.Open();
                _reader = new BinaryReader(_serialPort.BaseStream);
                _serialPort.WriteTimeout = 5000;
                _serialPort.ReadTimeout = 5000;
                _reader = new BinaryReader(_serialPort.BaseStream);
                _serialPort.DataReceived += DataReceived;
                OpenSocket();
            }
            return Task.FromResult(IsConnected);
        }

        public Task Disconnect()
        {
            if (IsConnected)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _serialPort.DataReceived -= DataReceived;
                _serialPort.Close();
                _serialPort.Dispose();
                _reader.Dispose();
                _reader = null;
                _serialPort = null;
                Clear();
            }
            return Task.CompletedTask;
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int size = _reader.ReadInt16();
            byte[] payLoad = _reader.ReadBytes(size);
            ResponseBuffer.TryAdd(Response.GetId(payLoad), payLoad);
        }

        private void OpenSocket()
        {
            Task.Factory.StartNew(async () =>
            {
                while (!CancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        Command command;

                        if (NoReplyCommandBuffer.TryDequeue(out command))
                        {
                            lock (_serialPort) _serialPort.Write(command.PayLoad, 0, command.PayLoad.Length);
                            ResponseBuffer.TryAdd(command.Id, null);
                        }


                        if (CommandBuffer.TryDequeue(out command))
                        {
                            lock (_serialPort) _serialPort.Write(command.PayLoad, 0, command.PayLoad.Length);
                            int retry = 0;
                            while (!ResponseBuffer.ContainsKey(command.Id) && retry < 20)
                            {
                                await Task.Delay(50, CancellationToken);
                                retry++;
                            }
                        }

                        if (EventBuffer.TryDequeue(out command))
                        {
                            lock (_serialPort) _serialPort.Write(command.PayLoad, 0, command.PayLoad.Length);
                        }
                    }
                    catch (TaskCanceledException) { }
                }
            }, CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls



        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Disconnect().ConfigureAwait(false).GetAwaiter().GetResult();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~BlueToothSocket()
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
