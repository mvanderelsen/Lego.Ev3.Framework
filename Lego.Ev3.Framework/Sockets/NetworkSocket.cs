using Lego.Ev3.Framework.Firmware;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework.Sockets
{
    internal class NetworkSocket : SocketBase, IDisposable, ISocket
    {
        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private const string UNLOCK = "GET /target?sn=\r\nProtocol:EV3\r\n\r\n";

        private CancellationTokenSource _cancellationTokenSource;
        public bool IsConnected => _stream!=null && _stream.CanRead && _stream.CanWrite;

        public CancellationToken CancellationToken { get; private set; }

        public override string ConnectionInfo => $"Network {_ipAddress}";


        private readonly string _ipAddress;
        public NetworkSocket(string ipAddress) : base()
        {
            _ipAddress = ipAddress;
        }

        public async Task<bool> Connect()
        {
            if (!IsConnected)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                CancellationToken = _cancellationTokenSource.Token;

                _tcpClient = new TcpClient();
                _tcpClient.Connect(_ipAddress, 5555);
                _stream = _tcpClient.GetStream();

                byte[] payLoad = Encoding.UTF8.GetBytes(UNLOCK);
                await _stream.WriteAsync(payLoad, 0, payLoad.Length);

                // read the "Accept:EV340\r\n\r\n" response
                int read = await _stream.ReadAsync(payLoad, 0, payLoad.Length);
                string response = Encoding.UTF8.GetString(payLoad, 0, read);
                if (string.IsNullOrEmpty(response)) throw new Exception("LEGO EV3 brick did not respond to the unlock command.");

                OpenSocket();
            }
            return IsConnected;
        }

        private void OpenSocket()
        {
            byte[] buffer = new byte[2];

            Task.Factory.StartNew(async () =>
            {
                while (!CancellationToken.IsCancellationRequested)
                {
                    await _stream.ReadAsync(buffer, 0, buffer.Length, CancellationToken);

                    short size = (short)(buffer[1] | buffer[2] << 8);
                    if (size > 0)
                    {
                        byte[] payLoad = new byte[size];
                        await _stream.ReadAsync(payLoad, 0, payLoad.Length);
                        Responses.TryAdd(Response.GetId(payLoad), payLoad);
                    }
                }
            }, CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);

            Task.Factory.StartNew(async () =>
            {
                while (!CancellationToken.IsCancellationRequested)
                {
                    if (NoReplyCommands.TryDequeue(out Command command))
                    {
                        await _stream.WriteAsync(command.PayLoad, 0, command.PayLoad.Length);
                        _stream.Flush();
                        Responses.TryAdd(command.Id, null);
                    }

                    byte[] payLoad;

                    if (Commands.TryDequeue(out payLoad))
                    {
                        await _stream.WriteAsync(payLoad, 0, payLoad.Length);
                        _stream.Flush();
                    }

                    if (Events.TryDequeue(out payLoad))
                    {
                        await _stream.WriteAsync(payLoad, 0, payLoad.Length);
                        _stream.Flush();
                    }
                }
            }, CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);
        }

        /// <summary>
        /// Disconnect from the EV3 brick.
        /// </summary>
        public Task Disconnect()
        {
            // close up the stream and handle
            if (IsConnected)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _tcpClient.Close();

                Clear();
            }
            return Task.CompletedTask;
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
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~NetworkSocket()
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
