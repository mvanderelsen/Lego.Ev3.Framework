namespace Lego.Ev3.Framework.Firmware
{

    /// <summary>
    /// Grouped system commands and direct commands
    /// </summary>
    public enum CommandType
    {
        /// <summary>
        /// Commmand with reply expected
        /// </summary>
        DIRECT_COMMAND_REPLY = 0x00,
        /// <summary>
        /// Commmand without reply
        /// </summary>
        DIRECT_COMMAND_NO_REPLY = 0x80,

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
