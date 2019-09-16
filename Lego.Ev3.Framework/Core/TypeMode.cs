namespace Lego.Ev3.Framework.Core
{
    /// <summary>
    /// The device Type and Mode
    /// </summary>
    public struct DeviceTypeMode
    {
        /// <summary>
        /// Type of the device
        /// </summary>
        public DeviceType Type { get; }

        /// <summary>
        /// Mode of the device
        /// </summary>
        public DeviceMode Mode { get; }

        internal DeviceTypeMode(DeviceType type, DeviceMode mode)
        {
            Type = type;
            Mode = mode;
        }
    }
}
