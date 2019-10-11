using System;

namespace Lego.Ev3.Framework.Firmware
{
    internal class CommandBuilder : PayLoadBuilder
    {
        public ushort Id { get; }

        public CommandType Type { get; }

        /// <summary>
        /// DirectCommmand
        /// Byte 0 – 1: Command size, Little Endian. Command size not including these 2 bytes
        /// Byte 2 – 3: Message counter, Little Endian. Forth running counter
        /// Byte 4: Command type. See defines above
        /// Byte 5 - 6: Reservation (allocation) of global and local variables using a compressed format (globals reserved in byte 5 and the 2 lsb of byte 6, locals reserved in the upper 6 bits of byte 6) – see below:
        /// Byte 7 – n: Byte codes as a single command or compound commands (I.e. more commands composed as a small program)
        /// </summary>
        /// <param name="type">Type of the command</param>
        /// <param name="globalAllocation">Maximum of 1024 bytes. Reservation (allocation) of global variables using a compressed format reserved in byte 5 and the 2 lsb of byte 6 (DirectCommmand Only).</param>
        /// <param name="localAllocation">Maximum of 64 bytes. Reservation (allocation) of local variables using a compressed format reserved in the upper 6 bits of byte 6 (DirectCommmand Only).</param>
        /// <param name="useEventId">Use fixed allocated id as command id</param>
        public CommandBuilder(CommandType type, ushort globalAllocation = 0, ushort localAllocation = 0, bool useEventId = false)
        {
            if (globalAllocation > 1024) throw new ArgumentException("Global buffer must be less than 1024 bytes", nameof(globalAllocation));
            if (localAllocation > 64) throw new ArgumentException("Local buffer must be less than 64 bytes", nameof(localAllocation));

            Id = useEventId ? CommandHandle.EVENT_ID : CommandHandle.NewId();
            Type = type;

            LittleEndian(0); // Command size, Little Endian. Command size not including these 2 bytes. Blank for setting at ToCommand() method

            // write message counter or unique message Id little-endian
            LittleEndian(Id);

            Raw((byte)type); // write command type

            switch(Type)
            {
                case CommandType.DIRECT_COMMAND_NO_REPLY:
                case CommandType.DIRECT_COMMAND_REPLY:
                    {
                        Raw((byte)globalAllocation); // lower bits of globalAllocation
                        Raw((byte)((localAllocation << 2) | (globalAllocation >> 8) & 0x03)); // upper bits of globalAllocation + localAllocation
                        break;
                    }
            }
        }

        public void OpCode(SYSTEM_OP code)
        {
            Raw((byte)code);
        }

        public void OpCode(OP code)
        {
            Raw((byte)code);
        }


        public Command ToCommand()
        {
            byte[] payLoad = ToBytes();

            // size of data, not including the 2 size bytes
            ushort size = (ushort)(payLoad.Length - 2);

            // little-endian
            payLoad[0] = (byte)size;
            payLoad[1] = (byte)(size >> 8);

            return new Command(Id, Type, payLoad);
        }
    }
}
