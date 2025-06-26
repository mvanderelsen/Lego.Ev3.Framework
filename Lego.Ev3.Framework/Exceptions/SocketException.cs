using System;

namespace Lego.Ev3.Framework
{
    /// <summary>
    /// Socket exception class thrown on socket unexpected disconnect or on initializing.
    /// </summary>
    public class SocketException : Exception
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="message">message</param>
        public SocketException(string message) : base(message) { }
    }
}
