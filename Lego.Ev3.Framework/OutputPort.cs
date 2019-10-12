using Lego.Ev3.Framework.Devices;
using Lego.Ev3.Framework.Internals;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework
{

    /// <summary>
    /// An output port
    /// </summary>
    public sealed class OutputPort
    {
        /// <summary>
        /// The Layer of this port
        /// </summary>
        internal ChainLayer Layer { get; private set; }

        /// <summary>
        /// The name of this port
        /// </summary>
        public OutputPortName Name { get; private set; }

        /// <summary>
        /// Port number in chain
        /// 16-31 OutputPorts (4 per brick eg. 16-19 are output ports on brick one)
        /// </summary>
        public int Number { get; private set; }

        /// <summary>
        /// The current status of this port
        /// </summary>
        public PortStatus Status { get; internal set; }

        /// <summary>
        /// The device connected to this port. (can be null)
        /// </summary>
        public IOutputDevice Device { get; private set; }

        /// <summary>
        /// Create the port
        /// </summary>
        /// <param name="layer">the brick layer</param>
        /// <param name="port">the port name</param>
        internal OutputPort(ChainLayer layer, OutputPortName port)
        {
            Layer = layer;
            Name = port;
            Status = PortStatus.Empty;
            Number = port.AbsolutePortNumber(layer); 
        }

        /// <summary>
        /// Sets the device to the port Layer,Number
        /// </summary>
        /// <param name="device">The device to connect to this port</param>
        internal void Set(IOutputDevice device)
        {
            OutputDevice idevice = (OutputDevice)device;
            idevice.Layer = Layer;
            idevice.PortName = Name;
            idevice.PortFlag = Name.ToFlag();
            idevice.PortNumber = Number;
            idevice.IsConnected = true;
            Device = device;
            Status = PortStatus.OK;
        }

        /// <summary>
        /// Initialize a device if connected to the Port
        /// Called from brick Initialize
        /// </summary>
        /// <returns></returns>
        internal async Task InitializeDevice()
        {
            if(Status == PortStatus.OK)
            {
                await ((OutputDevice)Device).InitializeDevice();
            }
        }

        /// <summary>
        /// Checks if a device is connected to the port for real
        /// Or if it is a setup error
        /// </summary>
        /// <param name="type">The type of the device</param>
        /// <param name="autoConnectDevice">Set up device if not connected through code but is connected to brick</param>
        /// <returns>Status Initializing if device is autoconnected, else Error or OK</returns>
        internal PortStatus CheckDevice(DeviceType type, bool autoConnectDevice)
        {
            if (Status == PortStatus.OK && Device.Type == type) return PortStatus.OK;
            if (Status == PortStatus.OK && Device.Type != type)
            {
                // Port is connected to unexpected device so set status to Error to avoid sending actions to wrong device
                Status = PortStatus.Error;
                return PortStatus.Error;
            }

            if(Status == PortStatus.Empty)
            {
                if (autoConnectDevice)
                {
                    switch (type)
                    {
                        case DeviceType.LargeMotor:
                            {
                                Set(new LargeMotor());
                                return PortStatus.Initializing;
                            }
                        case DeviceType.MediumMotor:
                            {
                                Set(new MediumMotor());
                                return PortStatus.Initializing;
                            }
                        default:
                            {
                                //unknow device so do not connect automatically
                                return PortStatus.OK;
                            }
                    }
                }
                else return PortStatus.OK;
            }
            return  PortStatus.Error;
        }
    }
}
