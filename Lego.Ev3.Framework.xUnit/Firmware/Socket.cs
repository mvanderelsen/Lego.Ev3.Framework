using Lego.Ev3.Framework.Firmware;
using System;
using System.Threading.Tasks;
namespace Lego.Ev3.Framework.xUnit.Firmware
{
    public class Socket : ISocket
    {
        private readonly Sockets.UsbSocket UsbSocket = new Sockets.UsbSocket();

        private static Socket _socket;
        private Socket() { }

        public static Socket Instance
        {
            get
            {
                if (_socket != null) return _socket;
                _socket = new Socket();
                _socket.UsbSocket.Connect(false).GetAwaiter().GetResult();
                return _socket;
            }
        }

        public void Disconnect() 
        {
            _socket.UsbSocket.Disconnect().GetAwaiter().GetResult();
            _socket.UsbSocket.Dispose();
            _socket = null;
        }

        public async Task<Response> Execute(Command command)
        {
            await UsbSocket.Write(command);
            switch (command.Type) 
            {
                case CommandType.DIRECT_COMMAND_NO_REPLY:
                case CommandType.SYSTEM_COMMAND_NO_REPLY: 
                    {
                        return Response.Ok(command.Id);
                    }
            }
            byte[] payLoad = await UsbSocket.Read();
            Response response = Response.FromPayLoad(payLoad);
            if (response.Type == ResponseType.ERROR) throw new FirmwareException(response);
            if (response.Id != command.Id) throw new InvalidOperationException("response id does not match command id");
            return response;
        }
    }
}
