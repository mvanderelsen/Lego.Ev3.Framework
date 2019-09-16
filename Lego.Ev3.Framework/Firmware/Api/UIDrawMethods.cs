using System.Threading.Tasks;
namespace Lego.Ev3.Framework.Firmware
{
    /// <summary>
    /// All UI draw methods
    /// </summary>
    internal static class UIDrawMethods
    {
        //CMD: BMPFILE = 0x1C
        //Arguments
        //(DATA8) Color – Specify either black or white, [0: White, 1: Black]
        //(Data16) X0 – Specify X start point, [0 - 177]
        //(Data16) Y0 – Specify Y start point, [0 - 127]
        //(Data8) NAME – First character in filename(Character string)
        //Description
        //Enable displaying BMP file from icon file within running project.
        internal static async Task BMPFile(Socket socket, string path, int x = 0, int y = 0, UIColor color = UIColor.Black)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.OpCode(OP.opUI_DRAW);
                cb.Raw((byte)UI_DRAW_SUBCODE.BMPFILE);
                cb.PAR8((int)color);
                cb.PAR16(x);
                cb.PAR16(y);
                cb.PARS(path);
                cb.OpCode(OP.opUI_DRAW);
                cb.Raw((byte)UI_DRAW_SUBCODE.UPDATE);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);

        }

        //CMD: UPDATE = 0x00
        //Description
        //Automatically triggers a refreshes of the display.
        internal static async Task Update(Socket socket)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.OpCode(OP.opUI_DRAW);
                cb.Raw((byte)UI_DRAW_SUBCODE.UPDATE);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }

        //CMD: UPDATE = 0x00
        //Description
        //Automatically triggers a refreshes of the display.
        internal static async Task Flush(Socket socket)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.OpCode(OP.opUI_FLUSH);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }

//        Instruction opUI_DRAW(CMD, …)
//Opcode 0x84
//Arguments(Data8) CMD => Specific command parameter documented below
//Dispatch status Can change to BUSYBREAK
//Description User interface draw entry


//CMD: PIXEL = 0x02
//Arguments
//(DATA8) Color – Specify either black or white, [0: White, 1: Black]
//(Data16) X0 – Specify X point, [0 - 177]
//(Data16) Y0 – Specify Y point, [0 - 127]
//        Description
//Enable drawing a single pixel.

        //CMD: LINE = 0x03
//Arguments
//(DATA8) Color – Specify either black or white, [0: White, 1: Black]
//(Data16) X0 – Specify X start point, [0 - 177]
//(Data16) Y0 – Specify Y start point, [0 - 127]
//(Data16) X1 – Specify X end point
//(Data16) Y1 – Specify Y end point
//Description
//Enable drawing a line between above coordinates

        //CMD: CIRCLE = 0x04
//Arguments
//(DATA8) Color – Specify either black or white, [0: White, 1: Black]
//(Data16) X0 – Specify X start point, [0 - 177]
//(Data16) Y0 – Specify Y start point, [0 - 127]
//(Data16) R – Radius for circle
//Description
//Enable drawing a circle

        //CMD: TEXT = 0x05
//Arguments
//(DATA8) Color – Specify either black or white, [0: White, 1: Black]
//(Data16) X0 – Specify X start point, [0 - 177]
//(Data16) Y0 – Specify Y start point, [0 - 127]
//(Data8) STRING – First character in string to draw(Zero terminated)
//Description
//Enable displaying text

        //CMD: ICON = 0x06
//Arguments
//(DATA8) Color – Specify either black or white, [0: White, 1: Black]
//(Data16) X0 – Specify X start point, [0 - 177]
//(Data16) Y0 – Specify Y start point, [0 - 127]
//(Data8) TYPE – Icon type(Selection of internal available icons)
//(Data) NO – Icon number
//Description
//Enable displaying predefined icons

        //CMD: PICTURE = 0x07
//Arguments
//(DATA8) Color – Specify either black or white, [0: White, 1: Black]
//(Data16) X0 – Specify X start point, [0 - 177]
//(Data16) Y0 – Specify Y start point, [0 - 127]
//(Data32) *IP – Address on picture
//Description
//Enable displaying picture from internal memory

        //CMD: VALUE = 0x08
//Arguments
//(DATA8) Color – Specify either black or white, [0: White, 1: Black]
//(Data16) X0 – Specify X start point, [0 - 177]
//(Data16) Y0 – Specify Y start point, [0 - 127]
//(DataF) VALUE – Floating point value to display
//(Data8) FIGURES – Total number of figures inclusive decimal point
//(Data8) DECIMALS – Number of decimals
//Description
//Enable displaying floating point value with specified resolution

        //CMD: FILLRECT = 0x09
//Arguments
//(DATA8) Color – Specify either black or white, [0: White, 1: Black]
//(Data16) X0 – Specify X start point, [0 - 177]
//(Data16) Y0 – Specify Y start point, [0 - 127]
//(Data16) X1 – Specify X size
//(Data16) Y1 – Specify Y size
//Description
//Enable drawing a filled rectangle

        //CMD: RECT = 0x0A
//Arguments
//(DATA8) Color – Specify either black or white, [0: White, 1: Black]
//(Data16) X0 – Specify X start point, [0 - 177]
//(Data16) Y0 – Specify Y start point, [0 - 127]
//(Data16) X1 – Specify X size
//(Data16) Y1 – Specify Y size
//Description
//Enable drawing a rectangle

        //CMD: NOTIFICATION = 0x0B
//Arguments
//(DATA8) Color – Specify either black or white, [0: White, 1: Black]
//(Data16) X0 – Specify X start point, [0 - 177]
//(Data16) Y0 – Specify Y start point, [0 - 127]
//(Data8) ICON1 – First icon
//(Data8) ICON2 – Second icon
//(Data8) ICON3 – Third icon
//(Data8) STRING – First character in notification string
//(Data8) *STATE – State, 0 = INIT
//Description
//Enable displaying multiple notification options to the user

        //CMD: QUESTION = 0x0C
//Arguments
//(DATA8) Color – Specify either black or white, [0: White, 1: Black]
//(Data16) X0 – Specify X start point, [0 - 177]
//(Data16) Y0 – Specify Y start point, [0 - 127]
//(Data8) ICON1 – First icon
//(Data8) ICON2 – Second icon
//(Data8) STRING – First character in question string
//(Data8) *STATE – State, 0 = No, 1 = OK
//Return
//(Data8) OK – Answer, 0 = No, 1 = OK, -1 = SKIP
//Description
//Enable displaying question to the user and returning the users selection.

        //CMD: KEYBOARD = 0x0D
//Arguments 
//(DATA8) Color – Specify either black or white, [0: White, 1: Black]
//(Data16) X0 – Specify X start point, [0 - 177]
//(Data16) Y0 – Specify Y start point, [0 - 127]
//(Data8) LENGTH – Maximum string length
//(Data8) DEFAULT – Default string, (0 = None)
//(Data8) *CHARSET – Internal use
//(Must be a variable initialised by a “valid character set”)
//Return
//(Data8) STRING – First character in string received from keyboard input
//Description
//Enable displaying a keyboard to the user, either with or without a pre-defined text.The function will return the input from keyboard.

        //CMD: BROWSE = 0x0E
//Arguments
//(DATA8) TYPE – See further specification below in type
//(Data16) X0 – Specify X start point, [0 - 177]
//(Data16) Y0 – Specify Y start point, [0 - 127]
//(Data16) X1– X size
//(Data16) Y1 – Y size
//(Data8) LENGTH – Maximum string length
//Return
//(Data8) TYPE – Item type(Folder, byte code file, sound file,…)
//(Must be a zero initialized variable)
//(Data8) STRING – First character in string receiving selected item name
//Description
//Enable displaying different browsers and content.
//TYPE available:
//0x00 : Browser for folders
//0x01 : Browser for folders and files
//0x02 : Browser for cached / recent files
//0x03 : Browser for files

        //CMD: VERTBAR = 0x0F
//Arguments 
//(DATA8) Color – Specify either black or white, [0: White, 1: Black]
//(Data16) X0 – Specify X start point, [0 - 177]
//(Data16) Y0 – Specify Y start point, [0 - 127]
//(Data16) X1 – Specify X end point
//(Data16) Y1 – Specify Y end point
//(Data16) MIN – Minimum value
//(Data16) MAX – Maximum value
//(Datat16) ACT – Actual value
//Description
//Enable display a vertical bar including a fill portion depending on the actual value in relation to minimum and maximum limits specified.

        //CMD: INVERSERECT = 0x10
//Arguments
//(Data16) X0 – Specify X start point, [0 - 177]
//(Data16) Y0 – Specify Y start point, [0 - 127]
//(Data16) X1 – Specify X size
//(Data16) Y1 – Specify Y size
//Description
//Enable inverting a already drawn filled rectangle.

        //CMD: SELECT_FRONT = 0x11
//Arguments
//(DATA8) TYPE – Specify font type, [0 - 2]
//        Description
//Enable specifying the font to be used.Font will change to 0 when UPDATE is called.

        //CMD: TOPLINE = 0x12
//Arguments
//(DATA8) ENABLE – Enable or disable top status line, [0: Disable, 1: Enable]
//Description
//Enable specifying the font to be used.Font will change to 0 when UPDATE is called.

        //CMD: FILLWINDOW = 0x13
//Arguments
//(DATA8) Color – Specify either black or white, [0: White, 1: Black]
//(Data16) Y0 – Specify Y start point, [0 - 127]
//(Data16) Y1 – Specify Y size
//Description
//Enable filling part of the display within the horizontal plane.

        //CMD: DOTLINE = 0x15
//Arguments 
//(DATA8) Color – Specify either black or white, [0: White, 1: Black]
//(Data16) X0 – Specify X start point, [0 - 177]
//(Data16) Y0 – Specify Y start point, [0 - 127]
//(Data16) X1 – Specify X end point
//(Data16) Y1 – Specify Y end point
//(Data16) ON – On pixels
//(Data16) OFF – Off pixels
//Description
//Enable drawing a dotted line between two coordinates

        //CMD: VIEW_VALUE = 0x16
//Arguments
//(DATA8) Color – Specify either black or white, [0: White, 1: Black]
//(Data16) X0 – Specify X start point, [0 - 177]
//(Data16) Y0 – Specify Y start point, [0 - 127]
//(DataF) VALUE – Floating point value to display
//(Data8) FIGURES – Total number of figures inclusive decimal point
//(Data8) DECIMALS – Number of decimals
//Description
//Enable displaying floating point value with specified resolution

        //CMD: VIEW_UNIT = 0x17
//Arguments
//(DATA8) Color – Specify either black or white, [0: White, 1: Black]
//(Data16) X0 – Specify X start point, [0 - 177]
//(Data16) Y0 – Specify Y start point, [0 - 127]
//(DataF) VALUE – Floating point value to display
//(Data8) FIGURES – Total number of figures inclusive decimal point
//(Data8) DECIMALS – Number of decimals
//(Data8) LENGTH – Maximum string length
//(Data8) STRING – First character in string to draw
//Description
//Enable displaying floating point value with specified resolution plus with specified units attached.

        //CMD: FILLCIRCLE = 0x18
//Arguments
//(DATA8) Color – Specify either black or white, [0: White, 1: Black]
//(Data16) X0 – Specify X start point, [0 - 177]
//(Data16) Y0 – Specify Y start point, [0 - 127]
//(Data16) R – Radius for circle
//Description
//Enable drawing a circle

        //CMD: STORE = 0x19
//Arguments 
//(DATA8) NO – Level number
//Description
//Enable storing current display image in memory.
//CMD: RESTORE = 0x1A
//Arguments
//(DATA8) NO – Level number(N= 0 => Saved screen just before run)
//Description
//Enable restoring previous display window.

        //CMD: ICON_QUESTION = 0x1B
//Arguments
//(DATA8) Color – Specify either black or white, [0: White, 1: Black]
//(Data16) X0 – Specify X start point, [0 - 177]
//(Data16) Y0 – Specify Y start point, [0 - 127]
//(Data8) *STATE – State, 0 = INIT
//(Data32) ICONS – Bitfield with icons
//Description
//Enable displaying a question window using different icon



        //CMD: GRAPH_SETUP = 0x1E
//Arguments
//(Data16) X0 – Specify X start point, [0 - 177]
//(Data16) Y0 – Specify Y start point, [0 - 127]
//(Data8) ITEMS – Number of datasets in arrayes
//(Data16) OFFSET – Data16 array(handle) containing Y point
//(Data16) SPAN – Data16 array(handle) containing Y size
//(DataF) MIN – DataF array(handle) containing min values
//(DataF) MAX – DataF array(handle) containing max values
//(DataF) SAMPLE – DataF array(handle) containing sample values
//Description
//Enable continuously drawing a graph including updating the graph according to new data value.

        //CMD: GRAPH_DRAW = 0x1F
//Arguments 
//(Data8) VIEW – Dataset number to view(0 = All)
//Description
//Enable live updating graph window including scroll.

        //CMD: TEXTBOX = 0x20
//Arguments
//(Data16) X0 – Specify X start point, [0 - 177]
//(Data16) Y0 – Specify Y start point, [0 - 127]
//(Data16) X1 – Specify X end point
//(Data16) Y1 – Specify Y end point
//(Data8) TEXT – First character in text box(Must be zero terminated)
//(Data32) SIZE – Maximal text size(Including zero termination)
//(Data8) DEL – Delimiter code, see below documentation
//Return
//(Data16) LINE – Selected line number
//Description
//Draws and controls a text box(one long string containing characters and line delimiters) on the screen.
//Delimiter code supported:
//0x00 : No delimiter used
//0x01 : Tab used as delimiter
//0x02 : Space used as delimiter
//0x03 : Return used as delimiter
//0x04 : Colon used as delimiter
//0x05 : Comma used as delimiter
//0x06 : Line feed used as delimiter
//0x07 : Return and line feed as delimiter

        
        //Instruction opUI_Flush()
//Opcode 0x80
//Arguments
//Dispatch status Unchanged
//Description This function flushes user interface buffers.
//Instruction

}
}
