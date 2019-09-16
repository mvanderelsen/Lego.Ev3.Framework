using System.Threading.Tasks;

namespace Lego.Ev3.Framework.Firmware
{
    /// <summary>
    /// All UI Write Methods
    /// </summary>
    internal static class UIWriteMethods
    {



        /// <summary>
        /// Set Led pattern
        /// </summary>
        /// <param name="socket">Socket for executing command to brick</param>
        /// <param name="pattern">Pattern of the led default = 0 Off</param>
        /// <remarks>
        ///CMD: LED = 0x1B
        ///Arguments
        ///(Data8) PATTERN – LED pattern, see below for more documentation
        ///Description
        ///Enable controlling the LED light around the button on EV3
        ///LED Patterns:
        ///0x00 : Led off
        ///0x01 : Led green
        ///0x02 : Led red
        ///0x03 : Led orange
        ///0x04 : Led green flashing
        ///0x05 : Led red flashing
        ///0x06 : Led orange flashing
        ///0x07 : Led green pulse
        ///0x08 : Led red pulse
        ///0x09 : Led orange pulse
        /// </remarks>
        internal static async Task Led(Socket socket, int pattern = 0)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.OpCode(OP.opUI_WRITE);
                cb.Raw((byte)UI_WRITE_SUBCODE.LED);
                cb.PAR8(pattern);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }





        ////        Instruction opKeep_Alive(MINUTES)
        ////Opcode 0x90
        ////Arguments(Data8) MINUTES – Number of minutes before entering sleep mode
        ////Dispatch status Unchanged
        ////Description This function enables controlling the power down time.


        //        Instruction opUI_WRITE(CMD, …)
        //Opcode 0x82
        //Arguments(Data8) CMD => Specific command parameter documented below
        //Dispatch status Can change to BUSYBREAK or FAILBREAK
        //Description User interface write entry
        //CMD: WRITE_FLUSH = 0x01
        //Description
        //Enable updating the terminal with all latest data.
        //CMD: FLOATVALUE = 0x02
        //Arguments
        //(DataF) VALUE – Value to write
        //(Data8) FIGURES – Total number figures inclusive decimal point
        //(Data8) DECIMALS – Number of decimals
        //Description
        //Enable writing a floating point value
        //CMD: PUT_STRING = 0x08
        //Arguments
        //(Data8) STRING – First character in string to write
        //Description
        //Enable writing a string
        //CMD: VALUE8 = 0x09
        //Arguments
        //(Data8) VALUE – Value to write
        //Description
        //Enable writing a 8-bit integer
        //CMD: VALUE16 = 0x0A LEGO® MINDSTORMS® EV3 Firmware Developer Kit
        //LEGO, the LEGO logo and MINDSTORMS are trademarks of the/sont des marques de
        //commerce de/son marcas registradas de LEGO Group. ©2013 The LEGO Group.
        //Page 94 of 109
        //Arguments
        //(Data8) VALUE – Value to write
        //Description
        //Enable writing a 16-bit integer
        //CMD: VALUE32 = 0x0B
        //Arguments
        //(Data8) VALUE – Value to write
        //Description
        //Enable writing a 32-bit integer
        //CMD: VALUEF= 0x0C
        //Arguments
        //(Data8) VALUE – Value to write
        //Description
        //Enable writing a floating point
        //CMD: DOWNLOAD_END = 0x0F
        //Description
        //Enables updating the user interface browser when a file download is done.A small sound will also be played to indicate an update has happened.
        //CMD: SCREEN_BLOCK = 0x10
        //Arguments
        //(Data8) STATUS – Value[o: Normal, 1: Blocked]
        //Description
        //Enable setting or clearing screen block status(With screen blocked – all graphics action are disabled.
        //CMD: TEXTBOX_APPEND = 0x15
        //Arguments
        //(Data8) TEXT – First character in text box text(Must be zero terminated)
        //(Data32) SIZE – Maximal text size(Including zero termination)
        //(Data8) DEL – Delimiter code
        //(Data8) SOURCE – String variable or handle to string append
        //Description
        //Enable appending line of text at the bottom of a text box
        //CMD: SET_BUSY = 0x16
        //Arguments
        //(Data8) VALUE – Set busy value, [0, 1]
        //        Description
        //Enable setting the busy flag.
        //CMD: SET_TESTPIN = 0x18
        //Arguments LEGO® MINDSTORMS® EV3 Firmware Developer Kit
        //LEGO, the LEGO logo and MINDSTORMS are trademarks of the/sont des marques de
        //commerce de/son marcas registradas de LEGO Group. ©2013 The LEGO Group.
        //Page 95 of 109
        //(Data8) STATE – Set test-pin, [0, 1]
        //        Description
        //Enable controlling the test-pin.Is only possible within test mode.
        //CMD: INIT_RUN= 0x19
        //Description
        //Start the “MINDSTORMS” “Run” screen

        //CMD: POWER = 0x1D
        //Arguments
        //(Data8) VALUE – Set power value, [0, 1]
        //        Description
        //Enable setting the power flag.
        //CMD: GRAPH_SAMPLE = 0x1E
        //Description
        //Update tick to scroll graph horizontal in memory when drawing graph in “Scope” mode
        //CMD: TERMINAL = 0x1F
        //Arguments
        //(Data8) STATE – Enable / disable terminal, [0, 1]
        //Description
        //Enable or disable terminal port.


        //Instruction opUI_Flush ()
        //Opcode 0x80
        //Arguments
        //Dispatch status Unchanged
        //Description This function flushes user interface buffers.
    }
}
