namespace Lego.Ev3.Framework.Firmware
{
    /// <summary>
    /// Firmware command
    /// </summary>
    public class Command
    {
        /// <summary>
        /// Id
        /// </summary>
        public ushort Id { get; }

        /// <summary>
        /// Type
        /// </summary>
        public CommandType Type { get; }

        /// <summary>
        /// Payload
        /// </summary>
        public byte[] PayLoad { get; }

        /// <summary>
        /// NoReply true if Type == CommandType.DIRECT_COMMAND_NO_REPLY || Type == CommandType.SYSTEM_COMMAND_NO_REPLY;
        /// </summary>
        public bool NoReply => Type == CommandType.DIRECT_COMMAND_NO_REPLY || Type == CommandType.SYSTEM_COMMAND_NO_REPLY;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="payLoad"></param>
        public Command(ushort id, CommandType type, byte[] payLoad)
        {
            Id = id;
            Type = type;
            PayLoad = payLoad;
        }
    }
}
