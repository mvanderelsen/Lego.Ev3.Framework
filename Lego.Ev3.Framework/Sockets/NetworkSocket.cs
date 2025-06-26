using Lego.Ev3.Framework.Firmware;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework.Sockets
{
    internal class NetworkSocket : Socket
    {
        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private const string UNLOCK = "GET /target?sn=\r\nProtocol:EV3\r\n\r\n";
        private readonly string _ipAddress;

        public override string ConnectionInfo => $"Network {_ipAddress}";


        public NetworkSocket(string ipAddress) : base()
        {
            _ipAddress = ipAddress;
        }

        protected override async Task<bool> ConnectSocket()
        {
            _tcpClient = new TcpClient();
            _tcpClient.Connect(_ipAddress, 5555);
            _stream = _tcpClient.GetStream();

            byte[] payLoad = Encoding.UTF8.GetBytes(UNLOCK);
            await _stream.WriteAsync(payLoad, 0, payLoad.Length);

            // read the "Accept:EV340\r\n\r\n" response
            int read = await _stream.ReadAsync(payLoad, 0, payLoad.Length);
            string response = Encoding.UTF8.GetString(payLoad, 0, read);
            if (string.IsNullOrEmpty(response)) throw new SocketException("LEGO EV3 brick did not respond to the unlock command.");
            return true;
        }


        protected override void DisconnectSocket()
        {
            _tcpClient?.Close();
            _tcpClient = null;
            _stream?.Close();
            _stream = null;
        }

        protected override async Task Write(Command command)
        {
            await _stream.WriteAsync(command.PayLoad, 0, command.PayLoad.Length);
            _stream.Flush();
        }

        protected override async Task<byte[]> Read(ushort id)
        {
            try
            {
                byte[] buffer = new byte[2];

                await _stream.ReadExactlyAsync(buffer, 0, buffer.Length, CancellationToken);

                short size = (short)(buffer[1] | buffer[2] << 8);
                if (size > 0)
                {
                    byte[] payLoad = new byte[size];
                    await _stream.ReadExactlyAsync(payLoad, 0, payLoad.Length);
                    return payLoad;
                }
            }
            catch (OperationCanceledException)
            {
            }
            return null;
        }
    }
}
