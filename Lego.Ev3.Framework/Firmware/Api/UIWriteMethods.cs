using System.Threading.Tasks;

namespace Lego.Ev3.Framework.Firmware
{
    /// <summary>
    /// All user interface write methods
    /// </summary>
    /// <remarks>
    /// See: LEGO® MINDSTORMS® EV3 Firmware Developer Kit 4.15 User interface operations
    /// </remarks>
    public static class UIWriteMethods
    {
        /// <summary>
        /// Set Led mode
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <param name="ledMode">Pattern of the led</param>
        /// <param name="requireReply">leave false</param>
        public static async Task Led(ISocket socket, LedMode ledMode, bool requireReply = false)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(requireReply ? CommandType.DIRECT_COMMAND_REPLY : CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.OpCode(OP.opUI_WRITE);
                cb.Raw((byte)UI_WRITE_SUBCODE.LED);
                cb.PAR8((byte)ledMode);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }
    }
}
