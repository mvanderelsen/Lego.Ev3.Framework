using Lego.Ev3.Framework.Firmware;
using System.IO;
using System.IO.Ports;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework.Sockets
{
    internal class BlueToothSocket : Socket
    {
        private SerialPort _serialPort;
        private BinaryReader _reader;
        private readonly string _comPort;

        public override string ConnectionInfo => $"BlueTooth {_comPort}";

       
        public BlueToothSocket(string comPort) : base()
        {
            _comPort = comPort;
        }

        protected override Task<bool> ConnectSocket()
        {
            _serialPort = new SerialPort(_comPort, 115200);
            _serialPort.Open();
            _reader = new BinaryReader(_serialPort.BaseStream);
            _serialPort.WriteTimeout = 5000;
            _serialPort.ReadTimeout = 5000;
            _reader = new BinaryReader(_serialPort.BaseStream);
            _serialPort.DataReceived += DataReceived;
            return Task.FromResult(_serialPort.IsOpen);
        }

        protected override void DisconnectSocket()
        {
            _serialPort.DataReceived -= DataReceived;
            _serialPort.Close();
            _serialPort.Dispose();
            _serialPort = null;

            _reader.Dispose();
            _reader = null;
          
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int size = _reader.ReadUInt16();
            byte[] payLoad = _reader.ReadBytes(size);
            Responses.TryAdd(Response.GetId(payLoad), payLoad);
        }

        protected override Task Write(Command command)
        {
            lock (_serialPort) _serialPort.Write(command.PayLoad, 0, command.PayLoad.Length);
            return Task.CompletedTask;
        }

        protected override Task<byte[]> Read(ushort id)
        {
            if (Responses.TryGetValue(id, out byte[] payload))     
            {
                return Task.FromResult(payload);
            }
            return Task.FromResult((byte[]) null);
        }
    }
}
