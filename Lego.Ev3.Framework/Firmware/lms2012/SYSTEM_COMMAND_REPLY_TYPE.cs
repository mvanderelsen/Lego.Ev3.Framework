
namespace Lego.Ev3.Framework.Firmware
{
    /// <summary>
    /// System command reply type
    /// </summary>
    internal enum SYSTEM_COMMAND_REPLY_TYPE
    {
        /// <summary>
        /// OK
        /// </summary>
        SYSTEM_REPLY = 0x03, // System command reply OK
        /// <summary>
        /// Error
        /// </summary>
        SYSTEM_REPLY_ERROR = 0x05
    }
}
