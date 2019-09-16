
namespace Lego.Ev3.Framework
{

    /// <summary>
    /// The status of a port
    /// </summary>
    public enum PortStatus
    {
        /// <summary>
        /// A device is connected to the brick
        /// </summary>
        OK = 0,
        /// <summary>
        /// Device is initializing
        /// </summary>
        Initializing = 0x7d,
        /// <summary>
        /// Port is empty
        /// </summary>
        Empty = 0x7e,
        /// <summary>
        /// Sensor is plugged into a motor port, or vice-versa
        /// </summary>
        Error = 0x7f,
        /// <summary>
        /// Unknown sensor/status
        /// </summary>
        Unknown = 0xff
    }
}
