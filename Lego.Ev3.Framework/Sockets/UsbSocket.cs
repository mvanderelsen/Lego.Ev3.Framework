using HidSharp;
using Lego.Ev3.Framework.Firmware;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework.Sockets
{
    internal class UsbSocket : Socket
    {
        private byte[] _input;
        private byte[] _output;
        private HidStream _stream;

        public override string ConnectionInfo { get { return "Usb"; } }

        protected override Task<bool> ConnectSocket()
        {
            HidDevice device = DeviceList.Local.GetHidDevices(vendorID: 0x0694, productID: 0x0005).FirstOrDefault();
            if (device == null) return Task.FromResult(false);

            OpenConfiguration openConfiguration = new();
            openConfiguration.SetOption(OpenOption.Exclusive, true);


            _stream = device.Open(openConfiguration);
            _input = new byte[device.GetMaxInputReportLength()];
            _output = new byte[device.GetMaxOutputReportLength()];


            if (_stream != null && _stream.CanRead && _stream.CanWrite) return Task.FromResult(true);
            throw new SocketException("Failed to open socket to Ev3");
        }


        protected override void DisconnectSocket()
        {
            _stream.Dispose();
            _stream = null;
            _input = null;
            _output = null;
        }

        protected override async Task Write(Command command)
        {
            command.PayLoad.CopyTo(_output, 1);
            await _stream.WriteAsync(_output, CancellationToken);
            _stream.Flush();
        }

        protected override async Task<byte[]> Read(ushort id)
        {
            try
            {
                await _stream.ReadExactlyAsync(_input, 0, _input.Length, CancellationToken);
            }
            catch (OperationCanceledException)
            {
                return null;
            }

            short size = (short)(_input[1] | _input[2] << 8);
            if (size > 0)
            {
                byte[] payLoad = new byte[size];
                Array.Copy(_input, 3, payLoad, 0, size);
                return payLoad;
            }

            return null;
        }

    }
}
