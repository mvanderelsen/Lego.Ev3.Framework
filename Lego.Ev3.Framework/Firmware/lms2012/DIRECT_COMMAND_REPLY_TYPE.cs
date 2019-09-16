
namespace Lego.Ev3.Framework.Firmware
{
    /// <summary>
    /// Direct command reply
    /// </summary>
    internal enum DIRECT_COMMAND_REPLY_TYPE
    {
        /// <summary>
        /// OK
        /// </summary>
        DIRECT_REPLY = 0x02, // Direct command reply OK
        /// <summary>
        /// Error
        /// </summary>
        DIRECT_REPLY_ERROR = 0x04, // Direct command reply ERROR
    }
}
