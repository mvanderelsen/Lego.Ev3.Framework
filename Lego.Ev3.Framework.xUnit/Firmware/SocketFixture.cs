using System;

namespace Lego.Ev3.Framework.xUnit.Firmware
{
    public class SocketFixture : IDisposable
    {

        public SocketFixture()
        {
            Socket socket = Socket.Instance;
        }

        public void Dispose()
        {
            Socket.Instance.Disconnect();
        }
    }
}
