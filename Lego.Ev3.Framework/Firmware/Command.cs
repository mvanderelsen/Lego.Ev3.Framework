namespace Lego.Ev3.Framework.Firmware
{
    public class Command
    {
        public ushort Id { get; }

        public CommandType Type { get; }

        public byte[] PayLoad { get; }

        public Command(ushort id, CommandType type, byte[] payLoad)
        {
            Id = id;
            Type = type;
            PayLoad = payLoad;
        }
    }
}
