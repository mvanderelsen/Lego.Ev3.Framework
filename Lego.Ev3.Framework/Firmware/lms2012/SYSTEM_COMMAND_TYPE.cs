
namespace Lego.Ev3.Framework.Firmware
{
    /// <summary>
    /// System command type
    /// </summary>
    internal enum SYSTEM_COMMAND_TYPE
    {
        /// <summary>
        /// Commmand with reply expected
        /// </summary>
        SYSTEM_COMMAND_REPLY = 0x01,
        /// <summary>
        /// Commmand without reply
        /// </summary>
        SYSTEM_COMMAND_NO_REPLY = 0x81
    }
}
