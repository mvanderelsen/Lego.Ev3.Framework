using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Lego.Ev3.Framework.Configuration
{
    public class BrickOptions : Options
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public SocketOptions Socket { get; set; } = new SocketOptions();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public PowerUpSelfTestOptions PowerUpSelfTest { get; set; } = new PowerUpSelfTestOptions();


        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<DeviceOptions> Devices { get; set; } = new List<DeviceOptions>();

        #region powerupselftest
        public void ConfigurePowerUpSelfTest(bool beepOnOK = true, bool beepOnAutoConnect = true, bool beepOnError = true, bool autoConnectDevices = false, bool disconnectOnError = true)
        {
            PowerUpSelfTest.Enabled = true;
            PowerUpSelfTest.BeepOnOK = beepOnOK;
            PowerUpSelfTest.BeepOnAutoConnect = beepOnAutoConnect;
            PowerUpSelfTest.BeepOnError = beepOnError;
            PowerUpSelfTest.AutoConnectDevices = autoConnectDevices;
            PowerUpSelfTest.DisconnectOnError = disconnectOnError;
        }

        public void DisablePowerUpSelfTest()
        {
            PowerUpSelfTest.Enabled = false;
        }
        #endregion

        #region socket
        public void UseUsbSocket()
        {
            Socket = new SocketOptions { Type = SocketType.Usb };
        }

        //public void UseBlueToothSocket(int comPortNumber)
        //{
        //    if (comPortNumber < 1 || comPortNumber > 256) throw new ArgumentOutOfRangeException(nameof(comPortNumber));
        //    Socket = new SocketOptions { Type = SocketType.Bluetooth, Address=$"COM{comPortNumber}" };
        //}

        //public void UseNetworkSocket(string ipAddress)
        //{
        //    if (string.IsNullOrEmpty(ipAddress)) throw new ArgumentNullException(nameof(ipAddress), "IPAddress is required");
        //    Socket = new SocketOptions { Type = SocketType.Bluetooth, Address = ipAddress };
        //}
        #endregion

        #region devices
        private void AddDevice(string id, DeviceType type, string port, string mode, ChainLayer layer)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException(nameof(id));
            Devices.Add(new DeviceOptions { Id = id, Type = type, Port = port, Mode = mode, Layer = layer });
        }

        public void AddLargeMotor(string id, OutputPortName outputPort, Polarity? polarity = null, ChainLayer layer = ChainLayer.One)
        {
            AddDevice(id, DeviceType.LargeMotor, outputPort.ToString(), polarity?.ToString(), layer);
        }

        public void AddMediumMotor(string id, OutputPortName outputPort, Polarity? polarity = null, ChainLayer layer = ChainLayer.One)
        {
            AddDevice(id, DeviceType.MediumMotor, outputPort.ToString(), polarity?.ToString(), layer);
        }

        public void AddTouchSensor(string id, InputPortName inputPort, TouchSensorMode? mode = null, ChainLayer layer = ChainLayer.One)
        {
            AddDevice(id, DeviceType.TouchSensor, inputPort.ToString(), mode?.ToString(), layer);
        }

        public void AddColorSensor(string id, InputPortName inputPort, ChainLayer layer = ChainLayer.One)
        {
            AddDevice(id, DeviceType.ColorSensor, inputPort.ToString(), null, layer);
        }

        public void AddGyroscopeSensor(string id, InputPortName inputPort, ChainLayer layer = ChainLayer.One)
        {
            AddDevice(id, DeviceType.GyroscopeSensor, inputPort.ToString(), null, layer);
        }

        public void AddInfraredSensor(string id, InputPortName inputPort, ChainLayer layer = ChainLayer.One)
        {
            AddDevice(id, DeviceType.InfraredSensor, inputPort.ToString(), null, layer);
        }

        public void AddUltrasonicSensor(string id, InputPortName inputPort, ChainLayer layer = ChainLayer.One)
        {
            AddDevice(id, DeviceType.UltrasonicSensor, inputPort.ToString(), null, layer);
        }
        #endregion

    }
}
