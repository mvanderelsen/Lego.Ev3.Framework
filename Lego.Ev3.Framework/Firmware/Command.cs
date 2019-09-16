namespace Lego.Ev3.Framework.Firmware
{
    internal class Command : CommandHandle
    {
        public byte[] PayLoad { get; }

        public Command(ushort id, CommandType type, byte[] payLoad) : base(id, type)
        {
            PayLoad = payLoad;
        }
    }
}
