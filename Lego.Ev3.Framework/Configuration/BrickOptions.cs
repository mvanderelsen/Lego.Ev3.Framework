using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Lego.Ev3.Framework.Configuration
{
    /// <summary>
    /// Brick Options
    /// </summary>
    public class BrickOptions : Options
    {
        /// <summary>
        /// Socket Options
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public SocketOptions Socket { get; set; } = new SocketOptions();

        /// <summary>
        /// PowerUpSelfTest Options
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public PowerUpSelfTestOptions PowerUpSelfTest { get; set; } = new PowerUpSelfTestOptions();

        /// <summary>
        /// EventMonitor Options
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public EventMonitorOptions EventMonitor { get; set; } = new EventMonitorOptions();

        /// <summary>
        /// Device List Options
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<DeviceOptions> Devices { get; set; } = new List<DeviceOptions>();

        #region eventmonitor

        /// <summary>
        /// Configures the event monitor
        /// </summary>
        /// <param name="interval">poll interval in  milliseconds. Default <c>100</c></param>
        public void ConfigureEventMonitor(int interval = 100)
        {
            EventMonitor.Enabled = true;
            EventMonitor.Interval = interval;
        }

        /// <summary>
        /// No events will be monitored. Only use if you will manually poll all events
        /// </summary>
        public void DisableEventMonitor()
        {
            EventMonitor.Enabled = false;
        }
        #endregion

        #region powerupselftest

        /// <summary>
        /// Change powerupselftest settings
        /// </summary>
        /// <param name="beepOnOK"></param>
        /// <param name="beepOnAutoConnect"></param>
        /// <param name="beepOnError"></param>
        /// <param name="autoConnectDevices"></param>
        /// <param name="disconnectOnError"></param>
        public void ConfigurePowerUpSelfTest(bool beepOnOK = true, bool beepOnAutoConnect = true, bool beepOnError = true, bool autoConnectDevices = false, bool disconnectOnError = true)
        {
            PowerUpSelfTest.Enabled = true;
            PowerUpSelfTest.BeepOnOK = beepOnOK;
            PowerUpSelfTest.BeepOnAutoConnect = beepOnAutoConnect;
            PowerUpSelfTest.BeepOnError = beepOnError;
            PowerUpSelfTest.AutoConnectDevices = autoConnectDevices;
            PowerUpSelfTest.DisconnectOnError = disconnectOnError;
        }

        /// <summary>
        /// Disables the powerupself test
        /// </summary>
        public void DisablePowerUpSelfTest()
        {
            PowerUpSelfTest.Enabled = false;
        }
        #endregion

        #region socket

        /// <summary>
        /// Connect the brick through an usb socket
        /// </summary>
        public void UseUsbSocket()
        {
            Socket = new SocketOptions { Type = SocketType.Usb };
        }


        /// <summary>
        /// Connect the brick through a bluetooth socket
        /// </summary>
        /// <param name="comPortNumber"></param>
        public void UseBlueToothSocket(int comPortNumber)
        {
            if (comPortNumber < 1 || comPortNumber > 256) throw new ArgumentOutOfRangeException(nameof(comPortNumber));
            Socket = new SocketOptions { Type = SocketType.Bluetooth, Address = $"COM{comPortNumber}" };
        }


        /// <summary>
        /// Connect the brick through a network socket
        /// </summary>
        /// <param name="ipAddress">ip adrress e.g. '192.168.2.11'</param>
        public void UseNetworkSocket(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress)) throw new ArgumentNullException(nameof(ipAddress), "IPAddress is required");
            Socket = new SocketOptions { Type = SocketType.Bluetooth, Address = ipAddress };
        }
        #endregion

        #region devices
        private void AddDevice(string id, DeviceType type, string port, string mode, ChainLayer layer)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException(nameof(id));
            Devices.Add(new DeviceOptions { Id = id, Type = type, Port = port, Mode = mode, Layer = layer });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="outputPort"></param>
        /// <param name="polarity"></param>
        /// <param name="layer"></param>
        public void AddLargeMotor(string id, OutputPortName outputPort, Polarity? polarity = null, ChainLayer layer = ChainLayer.One)
        {
            AddDevice(id, DeviceType.LargeMotor, outputPort.ToString(), polarity?.ToString(), layer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="outputPort"></param>
        /// <param name="polarity"></param>
        /// <param name="layer"></param>
        public void AddMediumMotor(string id, OutputPortName outputPort, Polarity? polarity = null, ChainLayer layer = ChainLayer.One)
        {
            AddDevice(id, DeviceType.MediumMotor, outputPort.ToString(), polarity?.ToString(), layer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="inputPort"></param>
        /// <param name="mode"></param>
        /// <param name="layer"></param>
        public void AddTouchSensor(string id, InputPortName inputPort, TouchSensorMode? mode = null, ChainLayer layer = ChainLayer.One)
        {
            AddDevice(id, DeviceType.TouchSensor, inputPort.ToString(), mode?.ToString(), layer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="inputPort"></param>
        /// <param name="layer"></param>
        public void AddColorSensor(string id, InputPortName inputPort, ChainLayer layer = ChainLayer.One)
        {
            AddDevice(id, DeviceType.ColorSensor, inputPort.ToString(), null, layer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="inputPort"></param>
        /// <param name="layer"></param>
        public void AddGyroscopeSensor(string id, InputPortName inputPort, ChainLayer layer = ChainLayer.One)
        {
            AddDevice(id, DeviceType.GyroscopeSensor, inputPort.ToString(), null, layer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="inputPort"></param>
        /// <param name="layer"></param>
        public void AddInfraredSensor(string id, InputPortName inputPort, ChainLayer layer = ChainLayer.One)
        {
            AddDevice(id, DeviceType.InfraredSensor, inputPort.ToString(), null, layer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="inputPort"></param>
        /// <param name="layer"></param>
        public void AddUltrasonicSensor(string id, InputPortName inputPort, ChainLayer layer = ChainLayer.One)
        {
            AddDevice(id, DeviceType.UltrasonicSensor, inputPort.ToString(), null, layer);
        }
        #endregion

    }
}
