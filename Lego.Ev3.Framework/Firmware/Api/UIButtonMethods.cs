using Lego.Ev3.Framework.Core;
using System;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework.Firmware
{
    /// <summary>
    /// All UIButton Methods
    /// </summary>
    internal static class UIButtonMethods
    {

        /// <summary>
        /// Method called from autopoll to build batch command
        /// </summary>
        internal static DataType Clicked_BatchCommand(PayLoadBuilder payLoadBuilder, int button, int index)
        {
            payLoadBuilder.Raw((byte)OP.opUI_BUTTON);
            payLoadBuilder.Raw((byte)UI_BUTTON_SUBCODE.GET_BUMBED);
            payLoadBuilder.PAR8((byte)button);
            payLoadBuilder.GlobalIndex(index);
            return DataType.DATA8;
        }

        /// <summary>
        /// Enable verifying if a button has been pressed
        /// </summary>
        /// <param name="socket">Socket for executing command to brick</param>
        /// <param name="button">
        /// 0x00 : No button, 0x01 : Up button, 0x02 : Enter button, 0x03 : Down button, 0x04 : Right button, 0x05 : Left button, 0x06 : Back button, 0x07 : Any button
        /// </param>
        /// <remarks>
        /// Instruction opUI_BUTTON (CMD, …)
        /// Opcode 0x83
        /// Arguments (Data8) CMD => Specific command parameter documented below
        /// Dispatch status Unchanged
        /// Description User interface button entry
        /// CMD: PRESSED = 0x09
        /// </remarks>
        /// <returns>true if pressed</returns>
        internal static async Task<bool> Clicked(Socket socket, int button)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, 1, 0))
            {
                Clicked_BatchCommand(cb, button, 0);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            bool isPressed = false;
            if (response.Type == ResponseType.OK)
            {
                byte[] data = response.PayLoad;
                isPressed = BitConverter.ToBoolean(data, 0);
            }
            return isPressed;
        }


        internal static async Task Flush(Socket socket)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.Raw((byte)OP.opUI_BUTTON);
                cb.Raw((byte)UI_BUTTON_SUBCODE.FLUSH);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }


        //        Instruction opUI_BUTTON(CMD, …) LEGO® MINDSTORMS® EV3 Firmware Developer Kit
        //LEGO, the LEGO logo and MINDSTORMS are trademarks of the/sont des marques de
        //commerce de/son marcas registradas de LEGO Group. ©2013 The LEGO Group.
        //Page 96 of 109
        //Opcode 0x83
        //Arguments (Data8) CMD => Specific command parameter documented below
        //Dispatch status Unchanged
        //Description User interface button entry
        //CMD: SHORTPRESS = 0x01
        //Arguments
        //(Data8) BUTTON – Button to evaluate, see below documentation
        //Return
        //(Data8) STATE – Button status, [0: No, 1: Yes]
        //Description
        //Enable verifying if a short button press has happen.
        //LED Patterns:
        //0x00 : No button
        //0x01 : Up button
        //0x02 : Enter button
        //0x03 : Down button
        //0x04 : Right button
        //0x05 : Left button
        //0x06 : Back button
        //0x07 : Any button
        //CMD: LONGPRESS = 0x02
        //Arguments
        //(Data8) BUTTON – Button to evaluate, reference SHORTPRESS command
        //Return
        //(Data8) STATE – Button status, [0: No, 1: Yes]
        //Description
        //Enable verifying if a button has been pressed for long period.
        //CMD: WAIT_FOR_PRESS = 0x03
        //Description
        //Enable waiting for any button press.
        //CMD: FLUSH = 0x04
        //Description
        //Enable removing all previous button status..
        //CMD: PRESS = 0x05
        //Arguments
        //(Data8) BUTTON – Button to evaluate, reference SHORTPRESS command
        //Description
        //Enable activating a button press from software.

        //CMD: RELEASE = 0x06
        //Arguments
        //(Data8) BUTTON – Button to evaluate, reference SHORTPRESS command
        //Description
        //Enable activating a button release from software.
        //CMD: GET_HORZ = 0x07
        //Return
        //(Data16) VALUE – Horizontal arrows data(-1: Left, +1: Right, 0: Not pressed)
        //Description
        //Enable reading current arrow position within the horizontal plane.
        //CMD: GET_VERT = 0x08
        //Return
        //(Data16) VALUE – Vertical arrows data(-1: Left, +1: Right, 0: Not pressed)
        //Description
        //Enable reading current arrow position within the vertical plane.

        //CMD: SET_BACK_BLOCK = 0x0A
        //Argument
        //(Data8) BLOCKED – Set UI back button blocked flag
        //(0: Not blocked, 1: Blocked)
        //Description
        //Enable blocking the back button functionality.
        //CMD: GET_BACK_BLOCK = 0x0B
        //Return
        //(Data8) BLOCKED – Get UI back button blocked flag
        //(0: Not blocked, 1: Blocked)
        //Description
        //Enable reading the back button block flag.

        //CMD: TESTSHORTPRESS = 0x0C
        //Arguments
        //(Data8) BUTTON – Button to evaluate, reference SHORTPRESS command
        //Return
        //(Data8) STATE – Button status, [0: No, 1: Yes]
        //Description
        //Enable verifying if a button has been/are being pressed for short duration.
        //CMD: TESTLONGPRESS = 0x0D
        //Arguments
        //(Data8) BUTTON – Button to evaluate, reference SHORTPRESS command
        //Return
        //(Data8) STATE – Button status, [0: No, 1: Yes]
        //Description
        //Enable verifying if a button has been/are being pressed for long duration.
        //CMD: GET_BUMBED = 0x0E
        //Arguments
        //(Data8) BUTTON – Button to evaluate, reference SHORTPRESS command
        //Return
        //(Data8) STATE – Button status, [0: No, 1: Yes]
        //Description
        //Enable verifying if a button has been bumped = Pressed and released.
        //CMD: GET_CLICK = 0x0F
        //Return
        //(Data8) CLICK – Click sound requested, [0: No, 1: Yes]
        //        Description
        //Get and clear click sound request.


    }
}
