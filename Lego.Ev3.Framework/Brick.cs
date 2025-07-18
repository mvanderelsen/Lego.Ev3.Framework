﻿using Lego.Ev3.Framework.Configuration;
using Lego.Ev3.Framework.Core;
using Lego.Ev3.Framework.Devices;
using Lego.Ev3.Framework.Firmware;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework
{
    /// <summary>
    /// LEGO® MINDSTORMS® EV3 Brick
    /// </summary>
    public class Brick : IOPorts
    {
        #region internal static singleton fields
        private static Socket _socket;
        /// <summary>
        /// The socket to communicate with the brick. Set in Connect method
        /// </summary>
        /// <exception cref="SocketException">On unexpected disconnect</exception>
        internal static Socket Socket
        {
            get
            {
                if (!IsConnected) throw new SocketException("No socket or socket disconnected. Connect to brick!");
                return _socket;
            }
        }

        /// <summary>
        /// Check if socket is connected without throwing an exception
        /// </summary>
        internal static bool IsConnected
        {
            get
            {
                if (_socket == null) return false;
                return _socket.IsConnected;
            }
        }

       
        internal static ILogger<Brick> Logger;
        internal static BrickOptions Options;
        #endregion

        #region private, internal

        /// <summary>
        /// Port for all methods addressing all IO ports in chain
        /// </summary>
        internal IOPort IOPort { get; }

        #endregion

        /// <summary>
        /// Name of the brick
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// A FileSystem Drive
        /// </summary>
        public Drive Drive { get; }

        /// <summary>
        /// A SDCard Drive will remain null if no SD card is present
        /// </summary>
        public SDCard SDCard { get; private set; }

        /// <summary>
        /// Battery
        /// </summary>
        public Battery Battery { get; }


        /// <summary>
        /// Led light 
        /// </summary>
        public Led Led { get; }

        /// <summary>
        /// Buttons on the brick
        /// </summary>
        public Buttons Buttons { get; }

        /// <summary>
        /// Port for all sound methods
        /// </summary>
        public Sound Sound { get; }

        /// <summary>
        /// Port for all display methods
        /// </summary>
        public Display Display { get; }

        /// <summary>
        /// Port to memory methods
        /// </summary>
        public Memory Memory { get; set; }

        /// <summary>
        /// Second daisy chained brick IO Ports
        /// </summary>
        public IOPorts Slave1 { get; }

        /// <summary>
        /// Third daisy chained brick IO Ports
        /// </summary>
        public IOPorts Slave2 { get; }

        /// <summary>
        /// Fourth daisy chained brick IO Ports
        /// </summary>
        public IOPorts Slave3 { get; }

        /// <summary>
        /// Console for advanced methods
        /// </summary>
        public BrickConsole Console { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options">if <c>null</c> set to default <see cref="BrickOptions"/></param>
        /// <param name="logger">if <c>null</c> set to default <see cref="NullLogger"/></param>
        public Brick(BrickOptions options = null, ILogger<Brick> logger = null) : base(ChainLayer.One)
        {
            Options = options ?? new BrickOptions();
            Logger = logger ?? new NullLogger<Brick>();

            Console = new BrickConsole();
            Sound = new Sound();
            Display = new Display();
            Led = new Led();
            Battery = new Battery();
            Drive = new Drive();
            Buttons = new Buttons();
            Memory = new Memory();

            //Init IO Ports
            IOPort = new IOPort();
            Slave1 = new IOPorts(ChainLayer.Two);
            Slave2 = new IOPorts(ChainLayer.Three);
            Slave3 = new IOPorts(ChainLayer.Four);

            //add ports to internal dictionary
            IOPort.AddPorts(this);
            IOPort.AddPorts(Slave1);
            IOPort.AddPorts(Slave2);
            IOPort.AddPorts(Slave3);

            //connect any devices from configuration if available
            DeviceConfiguration.Connect(IOPort, Options.Devices);
        }


        /// <summary>
        /// Connect the brick and start event monitor
        /// </summary>
        /// <param name="cancellationToken">optional cancellationToken</param>
        /// <returns>true if connected</returns>
        public async Task<bool> Connect(CancellationToken cancellationToken = default)
        {
            if (IsConnected) return true;

            switch (Options.Socket.Type)
            {
                case SocketType.Usb:
                    {
                        _socket = new Socket(new Sockets.UsbSocket());
                        break;
                    }
                case SocketType.Bluetooth:
                    {
                        _socket = new Socket(new Sockets.BlueToothSocket(Options.Socket.Address));
                        break;
                    }
                case SocketType.Network:
                    {
                        _socket = new Socket(new Sockets.NetworkSocket(Options.Socket.Address));
                        break;
                    }
                default: throw new NotImplementedException(nameof(Options.Socket.Type));
            }

            Logger.LogInformation($"Connecting to brick on {_socket.ConnectionInfo}");

            try
            {
                if (!_socket.IsConnected) await _socket.Connect(cancellationToken);

                //reset and stop all devices. Might be overkill but just to make sure nothing is running.
                await Stop();
                await Reset();

                Name = await Console.GetBrickName();

                //TODO Usb drive attached to brick
                bool sdCardPresent = await MemoryMethods.Exists(Socket, BrickExplorer.SDCARD_PATH);
                if (sdCardPresent)
                {
                    SDCard = new SDCard();
                }


                if (Options.PowerUpSelfTest != null && Options.PowerUpSelfTest.Enabled) await PowerUpSelfTest();
                else await InitializeDevices();

                if(Options.EventMonitor.Enabled && IsConnected) Socket.StartEventMonitor(this);

            }
            catch (Exception e)
            {
                Logger.LogError(e, "Unexpected exception occured");
                if (IsConnected) _socket.Disconnect();
            }

            if (IsConnected) Logger.LogInformation("Connected to brick");
            else Logger.LogError($"Failed to connect to brick on {_socket.ConnectionInfo}");
            
            return IsConnected;
        }

        /// <summary>
        /// Stops all ports and devices
        /// </summary>
        public async Task Stop()
        {
            if (IsConnected)
            {
                await Task.WhenAll(Sound.Stop(), IOPort.Stop());
            }
        }

        /// <summary>
        /// Resets all ports and devices
        /// </summary>
        public async Task Reset()
        {
            if (IsConnected)
            {
                await IOPort.Reset();
                await Led.Reset();
            }
        }

        /// <summary>
        /// Disconnect brick from socket.
        /// Will call Stop before closing socket
        /// </summary>
        public async Task Disconnect()
        {
            if (IsConnected)
            {
                await Stop();
                await Led.Reset();
                _socket.Disconnect();
                Logger.LogInformation("Disconnected from brick");
            }
        }

        /// <summary>
        /// Finds a device that is connected to the brick by id 
        /// </summary>
        /// <typeparam name="T">Any device LargeMotor, ColorSensor etc.</typeparam>
        /// <param name="id">id of the device</param>
        /// <returns><c>T</c> if found otherwise <c>null</c></returns>
        /// <exception cref="DeviceException">device found is not of the expected device type of <c>T</c></exception>
        /// <exception cref="System.ArgumentNullException">id is required</exception>
        public T FindDevice<T>(string id) where T : Device
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));
            if (typeof(IInputDevice).IsAssignableFrom(typeof(T)))
            {

                IInputDevice device = IOPort.FindInputDevice(id);
                if (device == null) return null;
                T obj = Activator.CreateInstance<T>();
                if (device.Type != obj.Type) throw new DeviceException($"{obj.Type} expected but found {device.Type}");
                return (T)device;
            }
            else
            {
                IOutputDevice device = IOPort.FindOutputDevice(id);
                if (device == null) return null;
                T obj = Activator.CreateInstance<T>();
                if (device.Type != obj.Type) throw new DeviceException($"{obj.Type} expected but found {device.Type}");
                return (T)device;
            }
        }

        private async Task InitializeDevices()
        {
            foreach (int key in IOPort.Input.Ports.Keys)
            {
                InputPort port = IOPort.Input.Ports[key];
                if (port.Status == PortStatus.OK) await port.InitializeDevice();
            }

            foreach (int key in IOPort.Output.Ports.Keys)
            {
                OutputPort port = IOPort.Output.Ports[key];
                if (port.Status == PortStatus.OK) await port.InitializeDevice();
            }
        }


        /// <summary>
        /// Checks if all devices are properly connected to the brick. False on any device/port error forcing brick to disconnect
        /// If true will beep once for OK, twice for OK and devices found (autoConnectDevices = true), Three times ERROR for device/port error(s)
        /// Test runs on setting PowerUpSelfTest  <see cref="Configuration.PowerUpSelfTestOptions"/>
        /// Brick must be connected.
        /// </summary>
        /// <returns>False on Error, otherwise true</returns>
        private async Task<bool> PowerUpSelfTest()
        {
            //do not stop on error detected to enable logging for all errors.
            bool errorDetected = false;
            bool autoConnectDevices = Options.PowerUpSelfTest.AutoConnectDevices;
            bool autoDevicesConnected = false;

            IEnumerable<PortInfo> list = await InputMethods.PortScan(Socket);
            List<PortInfo> devices = [.. list];

            for (int layer = 0; layer < 4; layer++)
            {
                for (int portNumber = 0; portNumber < 4; portNumber++)
                {
                    int index;
                    PortInfo entry;
                    PortStatus status;
                    string message;

                    #region Input Ports

                    index = (layer * 4) + portNumber;
                    entry = devices[index];

                    InputPort inputPort = IOPort.Input.Ports[index];

                    message = $"Brick {(ChainLayer)layer} Port {inputPort.Name}";

                    switch (entry.Status)
                    {
                        case PortStatus.OK:
                            {
                                status = inputPort.CheckDevice(entry.Device.Value, Options.PowerUpSelfTest.AutoConnectDevices);
                                switch (status)
                                {
                                    case PortStatus.Error:
                                        {
                                            Logger.LogWarning($"Invalid device {inputPort.Device.Type} {message}");
                                            inputPort.Status = PortStatus.Error;
                                            errorDetected = true;
                                            break;
                                        }
                                    case PortStatus.Initializing:
                                        {
                                            autoDevicesConnected = true;
                                            break;
                                        }
                                }
                                break;
                            }
                        case PortStatus.Empty:
                            {
                                if (inputPort.Status == PortStatus.OK)
                                {
                                    Logger.LogWarning($"Device {inputPort.Device.Type} is not connected {message}");
                                    inputPort.Status = PortStatus.Error;
                                    //unconnected device!
                                    errorDetected = true;
                                }
                                break;
                            }
                        case PortStatus.Error:
                            {
                                Logger.LogWarning($"{message} Device {entry.Device}");
                                inputPort.Status = PortStatus.Error;
                                // Output device connected to InputPort
                                errorDetected = true;
                                break;
                            }
                        default:
                            {
                                Logger.LogWarning($"{message} Device {entry.Device}");
                                inputPort.Status = PortStatus.Error;
                                errorDetected = true;
                                break;
                            }
                    }

                    if (!errorDetected && inputPort.Status == PortStatus.OK) await inputPort.InitializeDevice();


                    #endregion

                    #region Output Ports

                    index = 16 + (layer * 4) + portNumber;
                    entry = devices[index];

                    OutputPort outputPort = IOPort.Output.Ports[index];

                    message = $"Brick {(ChainLayer)layer} Port {outputPort.Name}";

                    switch (entry.Status)
                    {
                        case PortStatus.OK:
                            {
                                status = outputPort.CheckDevice(entry.Device.Value, autoConnectDevices);
                                switch (status)
                                {
                                    case PortStatus.Error:
                                        {
                                            Logger.LogWarning($"Invalid device {outputPort.Device.Type} {message}");
                                            outputPort.Status = PortStatus.Error;
                                            errorDetected = true;
                                            break;
                                        }
                                    case PortStatus.Initializing:
                                        {
                                            autoDevicesConnected = true;
                                            break;
                                        }
                                }
                                break;
                            }
                        case PortStatus.Empty:
                            {
                                if (outputPort.Status == PortStatus.OK)
                                {
                                    Logger.LogWarning($"Device {outputPort.Device.Type} is not connected {message}");
                                    outputPort.Status = PortStatus.Error;
                                    //unconnected device!
                                    errorDetected = true;
                                }
                                break;
                            }
                        case PortStatus.Error:
                            {
                                Logger.LogWarning($"{message} Device {entry.Device}");
                                outputPort.Status = PortStatus.Error;
                                // InputPort device connected to Outputport
                                errorDetected = true;
                                break;
                            }
                        default:
                            {
                                Logger.LogWarning($"{message} Device {entry.Device}");
                                outputPort.Status = PortStatus.Error;
                                errorDetected = true;
                                break;
                            }
                    }

                    if (!errorDetected && outputPort.Status == PortStatus.OK) await outputPort.InitializeDevice();

                    #endregion
                }
            }


            int numberOfLoops = 1; // beep once for OK
            if (errorDetected) numberOfLoops = 3; //beep three times
            else if (autoDevicesConnected) numberOfLoops = 2; //beep two times devices found, but all is still well :-)

            bool beep = false;
            switch (numberOfLoops)
            {
                case 1:
                    {
                        beep = Options.PowerUpSelfTest.BeepOnOK;
                        break;
                    }
                case 2:
                    {
                        // all is still well so beep once if set anyhow.
                        beep = (Options.PowerUpSelfTest.BeepOnAutoConnect) ? true : Options.PowerUpSelfTest.BeepOnOK;
                        break;
                    }
                case 3:
                    {
                        beep = Options.PowerUpSelfTest.BeepOnError;
                        break;
                    }
            }

            if (beep)
            {
                CancellationToken token = Socket.CancellationToken;
                await Task.Run(async () =>
                {
                    for (int i = 0; i < numberOfLoops; i++)
                    {
                        await SoundMethods.Tone(Socket, 50, 850, 150);
                        await Task.Delay(400, token);
                    }
                }, token);
            }

            if (errorDetected && Options.PowerUpSelfTest.DisconnectOnError)
            {
                await Disconnect();
            }

            Logger.LogInformation($"PowerUpSelfTest: {(errorDetected ? "ERROR" : "OK")}");
            return !errorDetected;
        }

    }
}
