using System;
using System.IO;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework.Firmware
{
    /// <summary>
    /// All methods that interact with brick soundport
    /// </summary>
    internal static class SoundMethods
    {
        /// <summary>
        /// Stops current sound playback.
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <remarks>
        /// Instruction opSound (CMD, …)
        /// Opcode 0x94
        /// Arguments (Data8) CMD => Specific command parameter documented below
        /// Dispatch status Unchanged
        /// Description Sound control entry
        /// CMD: BREAK = 0x00 (Stop current sound playback)
        /// </remarks>
        internal static async Task Break(ISocket socket)
        {
            Command cmd = null;
            using (CommandBuilder cb = new(CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.OpCode(OP.opSOUND);
                cb.Raw((byte)SOUND_SUBCODE.BREAK);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }

        /// <summary>
        /// Plays a tone based on frequency for given duration and at a given volume.
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <param name="volume">Specify volume for playback, [0 - 100]</param>
        /// <param name="frequency">Specify frequency, [250 - 10000]</param>
        /// <param name="duration">Specify duration in milliseconds [1 - n]</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <remarks>
        /// Instruction opSound (CMD, …)
        /// Opcode 0x94
        /// Arguments (Data8) CMD => Specific command parameter documented below
        /// Dispatch status Unchanged
        /// Description Sound control entry
        /// CMD: TONE = 0x01
        /// Arguments
        /// (Data8) VOLUME – Specify volume for playback, [0 - 100]
        /// (Data16) FREQUENCY – Specify frequency, [250 - 10000]
        /// (Data16) DURATION – Specify duration in millisecond
        /// </remarks>
        internal static async Task Tone(ISocket socket, int volume, int frequency, int duration)
        {
            if (volume < 0 || volume > 100) throw new ArgumentOutOfRangeException("Volume must be between 0 and 100", "volume");
            if (frequency < 250 || frequency > 10000) throw new ArgumentOutOfRangeException("Frequency must be between 250 and 10000", "frequency");
            if (duration < 1) throw new ArgumentOutOfRangeException("Duration must be longer than 0", "duration");

            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.OpCode(OP.opSOUND);
                cb.Raw((byte)SOUND_SUBCODE.TONE);
                cb.PAR8(volume);
                cb.PAR16(frequency);
                cb.PAR16(duration);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }

        /// <summary>
        /// Plays a sound file on the brick at a given volume.
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <param name="volume">Specify volume for playback, [0 - 100]</param>
        /// <param name="filePath">FilePath to sound file: ../prjs/myproject/file.rsf</param>
        /// <remarks>
        /// Instruction opSound (CMD, …)
        /// Opcode 0x94
        /// Arguments (Data8) CMD => Specific command parameter documented below
        /// Dispatch status Unchanged
        /// Description Sound control entry
        /// CMD: PLAY = 0x02
        /// Arguments
        /// (Data8) VOLUME – Specify volume for playback, [0 - 100]
        /// (Data8) NAME – First character in filename (Character string)
        /// </remarks>
        internal static async Task Play(ISocket socket, int volume, string filePath)
        {
            if (volume < 0 || volume > 100) throw new ArgumentOutOfRangeException("Volume must be between 0 and 100", "volume");

            //soundfiles will not play when extension in path
            filePath = Path.ChangeExtension(filePath, null);

            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.OpCode(OP.opSOUND);
                cb.Raw((byte)SOUND_SUBCODE.PLAY);
                cb.PAR8(volume);
                cb.PARS(filePath); //?? should be data8 ??
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }

        /// <summary>
        /// Repeats playing a sound file on the brick at a given volume.
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <param name="volume">Specify volume for playback, [0 - 100]</param>
        /// <param name="filePath">FilePath to sound file: ../prjs/myproject/file.rsf</param>
        /// <remarks>
        /// Instruction opSound (CMD, …)
        /// Opcode 0x94
        /// Arguments (Data8) CMD => Specific command parameter documented below
        /// Dispatch status Unchanged
        /// Description Sound control entry
        /// CMD: REPEAT = 0x03
        /// Arguments
        /// (Data8) VOLUME – Specify volume for playback, [0 - 100]
        /// (Data8) NAME – First character in filename (Character string)
        /// </remarks>
        internal static async Task Repeat(ISocket socket, int volume, string filePath)
        {
            if (volume < 0 || volume > 100) throw new ArgumentOutOfRangeException("Volume must be between 0 and 100", "volume");

            //soundfiles will not play when extension in path
            filePath = Path.ChangeExtension(filePath, null);

            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.OpCode(OP.opSOUND);
                cb.Raw((byte)SOUND_SUBCODE.REPEAT);
                cb.PAR8(volume); // (Data8) VOLUME
                cb.PARS(filePath); // (Data8) NAME – First character in filename (Character string) ?
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }

        /// <summary>
        /// This function enables the program to test if sound is busy (Playing sound or tone)
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <returns>Output busy flag, [0 = Ready, 1 = Busy]</returns>
        /// <remarks>
        /// Instruction opSound_Test (BUSY)
        /// Opcode 0x95
        /// Arguments
        /// Return (Data8) BUSY – Output busy flag, [0 = Ready, 1 = Busy]
        /// Dispatch status Unchanged
        /// Description This function enables the program to test if sound is busy (Playing sound or tone)
        /// </remarks>
        internal static async Task<bool> Test(ISocket socket)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, 1, 0))
            {
                cb.OpCode(OP.opSOUND_TEST);
                cb.GlobalIndex(0);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            bool isBusy = false;
            if (response.Type == ResponseType.OK)
            {
                byte[] data = response.PayLoad;
                isBusy = BitConverter.ToBoolean(data, 0);
            }
            return isBusy;
        }

        /// <summary>
        /// Enables program execution to wait for sound ready. (Wait for completion)
        /// Dispatch status Can changed to BUSYBREAK
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <remarks>
        /// Instruction opSound_Ready ()
        /// Opcode 0x96
        /// Arguments
        /// Dispatch status Can changed to BUSYBREAK
        /// Description Enables program execution to wait for sound ready. (Wait for completion)
        /// </remarks>
        internal static async Task Ready(ISocket socket)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.OpCode(OP.opSOUND_READY);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }

        /// <summary>
        /// ?? Must be tested
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        internal static async Task Service(ISocket socket)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.OpCode(OP.opSOUND);
                cb.Raw((byte)SOUND_SUBCODE.SERVICE);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }
    }
}
