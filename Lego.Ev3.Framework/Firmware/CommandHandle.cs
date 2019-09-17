namespace Lego.Ev3.Framework.Firmware
{
    internal abstract class CommandHandle
    {
        public ushort Id { get; }

        public CommandType Type { get; }

        public CommandHandle(ushort id, CommandType type)
        {
            Id = id;
            Type = type;
        }

        private static ushort _id = 0x0001;

        private static readonly object @lock = new object();

        internal static ushort NewId()
        {
            lock (@lock)
            {
                //skip fixed id's reserve 100
                if (_id > FREE) _id = 0x0000;
                _id += 1;
            }
            return _id;
        }


        //use fixed id for event polling
        internal const ushort EVENT_ID = ushort.MaxValue;

        //free id ushort slots
        private const ushort FREE = ushort.MaxValue - 100;

    }
}
