using Lego.Ev3.Framework.Firmware;
using Lego.Ev3.Framework.Core;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework.Devices
{
    /// <summary>
    /// A device that can connected to the brick input ports.
    /// Use this class to derive from if adding a new custom input device.
    /// It contains all output methods protected so they can be exposed or used in deriving class for new custom output device.
    /// Call methods only after connection to the brick is made.
    /// </summary>
    public abstract class InputDevice : Device, IInputDevice
    {
        #region Internal Port members
        internal ChainLayer Layer { get; set; }
        internal InputPortName PortName { get; set; }
        internal int PortNumber { get; set; }
        #endregion

        /// <summary>
        /// Gets or sets if this device is automatically polled and fires onchange events
        /// If false value must be read manually
        /// </summary>
        public bool MonitorEvents { get; set; }

        /// <summary>
        /// Sets or gets the general device mode [0-7]
        /// Not all devicemodes apply for any specific inputdevice.
        /// </summary>
        internal DeviceMode DeviceMode { get; set; }

        /// <summary>
        /// Constructs InputDevice <see cref="Device(DeviceType)"/>
        /// </summary>
        /// <param name="type">The Device type of the device</param>
        protected InputDevice(DeviceType type) : base(type) 
        {
            MonitorEvents = true;
        }


        /// <summary>
        /// Connect the device to a port
        /// </summary>
        /// <param name="port">Any input port on brick</param>
        public void Connect(InputPort port)
        {
            if (Brick.IsConnected) throw new DeviceException("Can not connect devices after connection to the brick is made!");
            if (IsConnected) throw new DeviceException("Can not reconnect devices to other ports!");
            port.Set(this);
        }

        /// <summary>
        /// Internal method called from Brick upon connect through Port
        /// </summary>
        internal async Task InitializeDevice()
        {
            await Initialize();
        }

        /// <summary>
        /// Method is called from Brick connect to initialize the device, Eg. set correct mode and type and starting values.
        /// Override for custom implementation
        /// </summary>
        protected abstract Task Initialize();


        /// <summary>
        /// Method to override to the correct batchcommand to get the proper value
        /// </summary>
        /// <param name="payLoadBuilder">CodeBuilder</param>
        /// <param name="index">global index</param>
        /// <returns>Value type on return after executing command</returns>
        internal virtual DataType BatchCommand(PayLoadBuilder payLoadBuilder, int index)
        {
            //TODO switch mode/ valuemode etc.. to allow custom devices here
            InputMethods.GetRawValue_BatchCommand(payLoadBuilder, Layer, PortNumber, index);
            return DataType.DATA_A4;
        }

        /// <summary>
        /// Internal method called from Brick EventMonitor polling
        /// </summary>
        /// <param name="value">The polled value</param>
        internal bool SetDeviceValue(object value)
        {
            return SetValue(value);
        }


        /// <summary>
        /// Method to override when creating custom device to override to set new Value for device
        /// </summary>
        /// <param name="value">object depending on method return DataType eg. DataType.DATA_A4 returns byte[4], DATA32 returns int32</param>
        /// <returns>bool if oldValue != newValue : device value changed</returns>
        protected abstract bool SetValue(object value);

        #region Firmware Methods


        #endregion
    }
}
