using Lego.Ev3.Framework.Core;
using System;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework.Firmware
{
    /// <summary>
    /// Methods to interact with the brick buttons
    /// </summary>
    /// <remarks>
    /// See: LEGO® MINDSTORMS® EV3 Firmware Developer Kit 4.15 User interface operations
    /// </remarks>
    internal static class UIButtonMethods
    {

        /// <summary>
        /// Method called from autopoll to build batch command
        /// </summary>
        internal static ushort BatchCommand(PayLoadBuilder payLoadBuilder, ButtonType button, ButtonMode mode, int index)
        {
            payLoadBuilder.Raw((byte)OP.opUI_BUTTON);
            payLoadBuilder.Raw((byte)mode);
            payLoadBuilder.PAR8((byte)button);
            payLoadBuilder.GlobalIndex(index);
            return DataType.DATA8.ByteLength();
        }

        /// <summary>
        /// Verifies if a button has been clicked, or depending on mode pressed.
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <param name="button">type of the button</param>
        /// <param name="mode">mode of click</param>
        /// <returns><c>true</c> if clicked, otherwise <c>false</c></returns>
        public static async Task<bool> GetClick(ISocket socket, ButtonType button, ButtonMode mode = ButtonMode.Click)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, 1, 0))
            {
                BatchCommand(cb, button, mode, 0);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);
            return BitConverter.ToBoolean(response.PayLoad, 0);
        }


        /// <summary>
        /// Flushes all button states
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <param name="requireReply">indicate if the brick should reply</param>
        /// <returns></returns>
        public static async Task Flush(ISocket socket, bool requireReply = true)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(requireReply ? CommandType.DIRECT_COMMAND_REPLY : CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.Raw((byte)OP.opUI_BUTTON);
                cb.Raw((byte)UI_BUTTON_SUBCODE.FLUSH);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }
    }
}
