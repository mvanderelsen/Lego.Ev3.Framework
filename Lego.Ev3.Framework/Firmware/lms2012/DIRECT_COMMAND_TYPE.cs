
namespace Lego.Ev3.Framework.Firmware
{

    /// <summary>
    /// Direct command
    /// </summary>
    internal enum DIRECT_COMMAND_TYPE
    {

        /// <summary>
        /// Commmand with reply expected
        /// </summary>
        DIRECT_COMMAND_REPLY = 0x00,
        /// <summary>
        /// Commmand without reply
        /// </summary>
        DIRECT_COMMAND_NO_REPLY = 0x80
    }
}
