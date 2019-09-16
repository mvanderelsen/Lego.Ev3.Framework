namespace Lego.Ev3.Framework.Configuration
{
    /// <summary>
    /// Socket info used to connect with brick
    /// </summary>
    public class SocketOptions : Options
    {

        /// <summary>
        /// The Socket Usb, BlueTooth or Network. Default: <c>Usb</c>
        /// </summary>
        public SocketType Type { get; set; } = SocketType.Usb;

        /// <summary>
        /// if Bluetooth or Network specify COM Port "COM5" or Ip Address "192.168.1.98"
        /// </summary>
        public string Address { get; set; }


    }
}
