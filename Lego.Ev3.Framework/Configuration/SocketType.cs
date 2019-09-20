namespace Lego.Ev3.Framework.Configuration
{
    /// <summary>
    /// The type of the socket
    /// </summary>
    public enum SocketType
    {
        /// <summary>
        /// Usb Socket
        /// </summary>
        Usb,
        /// <summary>
        /// Bluetooth socket requires COM address
        /// </summary>
        Bluetooth,
        /// <summary>
        /// Network socket requires IP address
        /// </summary>
        Network
    }
}
