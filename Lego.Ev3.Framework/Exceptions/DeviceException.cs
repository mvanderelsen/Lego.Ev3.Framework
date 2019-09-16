using System;

namespace Lego.Ev3.Framework
{
    /// <summary>
    /// Device exception class thrown if device is not connected to brick
    /// </summary>
    public class DeviceException : Exception
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="message">message</param>
        public DeviceException(string message) : base(message) { }
    }
}
