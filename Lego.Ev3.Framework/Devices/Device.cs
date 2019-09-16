using Lego.Ev3.Framework.Firmware;
using System;
namespace Lego.Ev3.Framework.Devices

{
    /// <summary>
    /// Base device class
    /// All devices input or output derive from this class
    /// </summary>
    public abstract class Device
    {
        /// <summary>
        /// Gets or sets an unique Id to identify device
        /// Default set to new System.Guid
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Set to true when device is connected to port
        /// Default set to false
        /// </summary>
        internal bool IsConnected { get; set; }

        internal Socket Socket
        {
            get
            {
                if (!IsConnected) throw new DeviceException("Device is not connected to brick!");
                return Brick.Socket;
            }
        }

        /// <summary>
        /// The type of device
        /// </summary>
        public DeviceType Type { get; }


        /// <summary>
        /// Sets the device to the Device Type
        /// and the Id to System.Guid.NewGuid()
        /// </summary>
        /// <param name="type">The Device type of the device</param>
        protected Device(DeviceType type)
        {
            Type = type;
            IsConnected = false;
            Id = Guid.NewGuid().ToString();
        }
    }
}
