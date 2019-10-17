using Lego.Ev3.Framework.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework.Firmware
{
    /// <summary>
    /// All UI Read Methods
    /// Instruction opUI_Read(CMD, …)
    /// Opcode 0x81
    /// </summary>
    internal static class UIReadMethods
    {


        #region battery

        /// <summary>
        /// Builds battery command and returns bytelength
        /// </summary>
        /// <param name="payLoadBuilder"></param>
        /// <param name="index"></param>
        /// <returns>Bytelength of the command</returns>
        internal static ushort GetBatteryValue_BatchCommand(PayLoadBuilder payLoadBuilder, int index)
        {
            int startIndex = index;
            payLoadBuilder.Raw((byte)OP.opUI_READ);
            payLoadBuilder.Raw((byte)UI_READ_SUBCODE.GET_VBATT);
            payLoadBuilder.GlobalIndex(index);// datatype = dataF
            index += DataType.DATAF.ByteLength();
            payLoadBuilder.Raw((byte)OP.opUI_READ);
            payLoadBuilder.Raw((byte)UI_READ_SUBCODE.GET_IBATT);
            payLoadBuilder.GlobalIndex(index); // datatype = dataF
            index += DataType.DATAF.ByteLength();
            payLoadBuilder.Raw((byte)OP.opUI_READ);
            payLoadBuilder.Raw((byte)UI_READ_SUBCODE.GET_TBATT);
            payLoadBuilder.GlobalIndex(index); // datatype = dataF
            index += DataType.DATAF.ByteLength();
            payLoadBuilder.Raw((byte)OP.opUI_READ);
            payLoadBuilder.Raw((byte)UI_READ_SUBCODE.GET_LBATT);
            payLoadBuilder.GlobalIndex(index);// datatype = data8
            index += DataType.DATA8.ByteLength();
            return (ushort)(index - startIndex);
        }

        internal static async Task<BatteryValue> GetBatteryValue(ISocket socket)
        {

            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, 13, 0))
            {
                GetBatteryValue_BatchCommand(cb, 0);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            return BatteryValue(response.PayLoad, 0);
        }

        internal static BatteryValue BatteryValue(byte[] data, int index)
        {
            BatteryValue battery = new BatteryValue(BatteryMode.All);
            battery.Voltage = BitConverter.ToSingle(data, index);
            index += DataType.DATAF.ByteLength();
            battery.Ampere = BitConverter.ToSingle(data, index);
            index += DataType.DATAF.ByteLength();
            battery.Temperature = BitConverter.ToSingle(data, index);
            index += DataType.DATAF.ByteLength();
            battery.Level = data[index];
            return battery;
        }

        internal static int BatteryValueLevel(byte[] data, int index)
        {
            return data[index];
        }

        internal static DataType GetBatteryLevel_BatchCommand(PayLoadBuilder payLoadBuilder, int index)
        {
            payLoadBuilder.Raw((byte)OP.opUI_READ);
            payLoadBuilder.Raw((byte)UI_READ_SUBCODE.GET_LBATT);
            payLoadBuilder.GlobalIndex(index);
            return DataType.DATA8;
        }

        //CMD: GET_VBATT = 0x01
        //Return
        //(DataF) Value – Battery voltage[V]

        //CMD: GET_IBATT = 0x02
        //Return 
        //(DataF) Value – Battery current[A]

        //CMD: GET_TBATT = 0x05
        //Return
        //(DataF) Value – Battery temperature rise[C]

        //CMD: GET_LBATT = 0x12
        //Return
        //(Data8) PCT – Battery level in percentage[0 - 100]
        #endregion

        #region USB

        // CAN NOT GET USB WORKING = TODO

        /// <summary>
        /// Builds Usb command and returns bytelength
        /// </summary>
        /// <param name="payLoadBuilder"></param>
        /// <param name="index"></param>
        /// <returns>Bytelength of the command</returns>
        internal static int GetUSB_BatchCommand(PayLoadBuilder payLoadBuilder, int index)
        {
            int startIndex = index;
            payLoadBuilder.Raw((byte)OP.opUI_READ);
            payLoadBuilder.Raw((byte)UI_READ_SUBCODE.GET_USBSTICK);
            payLoadBuilder.GlobalIndex(0);
            //cb.GlobalIndex(3);
            //cb.GlobalIndex(8);
            //index += DataType.DATA8.ByteLength();
            //cb.GlobalIndex(1);
            //index += DataType.DATA32.ByteLength();
            // cb.GlobalIndex(3);
            //index += DataType.DATA32.ByteLength();
            return index - startIndex;
        }

        //internal static async Task<USBStickDrive> GetUSBDrive(ISocket socket)
        //{

        //    Command cmd = null;
        //    using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, 9, 0))
        //    {
        //        GetUSB_BatchCommand(cb, 0);
        //        cmd = cb.ToCommand();
        //    }
        //    Response response = await socket.Execute(cmd);


        //    USBStickDrive drive = null;

        //    if (response.Type == ResponseType.OK)
        //    {
        //        byte[] data = response.PayLoad;
        //        drive = USBDriveFromReplyData(data, 0);
        //    }
        //    return drive;
        //}

        //internal static USBStickDrive USBDriveFromReplyData(byte[] data, int index)
        //{
        //    USBStickDrive drive = new USBStickDrive();
        //    drive.State = (DriveState)data[index];
        //    if(drive.State == DriveState.OK)
        //    {
        //        index += DataType.DATA8.ByteLength();
        //        drive.USBStick = new USBStick();
        //        drive.USBStick.Total = BitConverter.ToInt32(data, index);
        //        index += DataType.DATA32.ByteLength();
        //        drive.USBStick.Free = BitConverter.ToInt32(data, index);
        //    }
        //    return drive;
        //}

        //CMD: GET_USBSTICK = 0x1E
        //Return
        //(Data8) STATE – USB memory stick present, [0: No, 1: Present]
        //(Data32) TOTAL – USB memory size[KB]
        //(Data32) FREE – Amount of free memory[KB]



        #endregion

        #region SD Card
            // CAN NOT GET SD CARD WORKING = TODO



        /// <summary>
        /// Builds Usb command and returns bytelength
        /// </summary>
        /// <param name="payLoadBuilder"></param>
        /// <param name="index"></param>
        /// <returns>Bytelength of the command</returns>
        internal static int GetSDCard_BatchCommand(PayLoadBuilder payLoadBuilder, int index)
        {
            int startIndex = index;
            payLoadBuilder.Raw((byte)OP.opUI_READ);
            payLoadBuilder.Raw((byte)UI_READ_SUBCODE.GET_SDCARD);
            payLoadBuilder.GlobalIndex(index);
            index += DataType.DATA8.ByteLength();
            payLoadBuilder.GlobalIndex(index);
            index += DataType.DATA32.ByteLength();
            payLoadBuilder.GlobalIndex(index);
            index += DataType.DATA32.ByteLength();
            return index - startIndex;
        }

        //TODO
        //internal static async Task<SDCardDrive> GetSDCardDrive(ISocket socket)
        //{

        //    Command cmd = null;
        //    using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, 9, 0))
        //    {
        //        //GetSDCard_BatchCommand(cb, 0);
        //        cb.OpCode(OP.opUI_READ);
        //        cb.Raw((byte)UI_READ_SUBCODE.GET_SDCARD);
        //        cb.GlobalIndex(0);
        //        cb.GlobalIndex(1);
        //        cb.GlobalIndex(5);
        //        cmd = cb.ToCommand();
        //    }


        //    Response response = await socket.Execute(cmd);

        //    SDCardDrive drive = null;

        //    if (response.Type == ResponseType.OK)
        //    {
        //        byte[] data = response.PayLoad;
        //        drive = SDCardDriveFromReplyData(data, 0);
        //    }
        //    return drive;
        //}

        //internal static SDCardDrive SDCardDriveFromReplyData(byte[] data, int index)
        //{
        //    SDCardDrive drive = new SDCardDrive();
        //    drive.State = (DriveState)data[index];
        //    if (drive.State == DriveState.OK)
        //    {
        //        index += DataType.DATA8.ByteLength();
        //        drive.SDCard = new SDCard();
        //        drive.SDCard.Total = BitConverter.ToInt32(data, index);
        //        index += DataType.DATA32.ByteLength();
        //        drive.SDCard.Free = BitConverter.ToInt32(data, index);
        //    }
        //    return drive;
        //}

        //CMD: GET_SDCARD= 0x1D 
        //Return
        //(Data8) STATE – SD Card present, [0: No, 1: Present]
        //(Data32) TOTAL – SD Card memory size[KB]
        //(Data32) FREE – Amount of free memory[KB]
        #endregion

        #region SystemInfo
        //CMD: GET_FW_VERS = 0x0A
        //Arguments
        //(Data32) LENGTH – Maximal length of string returned(-1 = No check)
        //Return
        //(Data8) DESTINATION – String variable or handle to string
        //Description
        //Enable reading the firmware version currently on the EV3 brick
        internal static async Task<string> GetFirmwareVersion(ISocket socket, int stringLength = -1)
        {

            if (stringLength < 1 || stringLength > 255) stringLength = 255;
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, (ushort)stringLength, 0))
            {
                cb.OpCode(OP.opUI_READ);
                cb.Raw((byte)UI_READ_SUBCODE.GET_FW_VERS);
                cb.PAR32(stringLength);
                cb.GlobalIndex(0);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            byte[] data = response.PayLoad;
            int index = Array.IndexOf(data, (byte)0);
            return Encoding.UTF8.GetString(data, 0, index);
        }


        //CMD: GET_FW_BUILD = 0x0B
        //Arguments 
        //(Data32) LENGTH – Maximal length of string returned(-1 = No check)
        //Return
        //(Data8) DESTINATION – String variable or handle to string
        //Description
        //Enable reading the OS version currently on the EV3 brick
        internal static async Task<string> GetFirmwareBuild(ISocket socket, int stringLength = -1)
        {

            if (stringLength < 1 || stringLength > 255) stringLength = 255;
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, (ushort)stringLength, 0))
            {
                cb.OpCode(OP.opUI_READ);
                cb.Raw((byte)UI_READ_SUBCODE.GET_FW_BUILD);
                cb.PAR32(stringLength);
                cb.GlobalIndex(0);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            byte[] data = response.PayLoad;
            int index = Array.IndexOf(data, (byte)0);
            return Encoding.UTF8.GetString(data, 0, index);
        }

        //CMD: GET_OS_VERS = 0x03
        //Arguments
        //(Data8) LENGTH – Maximal length of string to be returned
        //Return
        //(Data) DESTINATION – String variable or handle to string
        //Description
        //Enable getting OS version string
        internal static async Task<string> GetOSVersion(ISocket socket, int stringLength = -1)
        {

            if (stringLength < 1 || stringLength > 255) stringLength = 255;
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, (ushort)stringLength, 0))
            {
                cb.OpCode(OP.opUI_READ);
                cb.Raw((byte)UI_READ_SUBCODE.GET_OS_VERS);
                cb.PAR32(stringLength);
                cb.GlobalIndex(0);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            byte[] data = response.PayLoad;
            int index = Array.IndexOf(data, (byte)0);
            return Encoding.UTF8.GetString(data, 0, index);
        }

        //CMD: GET_OS_BUILD = 0x0C
        //Arguments
        //(Data32) LENGTH – Maximal length of string returned(-1 = No check)
        //Return
        //(Data8) DESTINATION – String variable or handle to string
        //Description
        //Enable reading the OS build info currently on the EV3 brick
        internal static async Task<string> GetOSBuild(ISocket socket, int stringLength = -1)
        {

            if (stringLength < 1 || stringLength > 255) stringLength = 255;
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, (ushort)stringLength, 0))
            {
                cb.OpCode(OP.opUI_READ);
                cb.Raw((byte)UI_READ_SUBCODE.GET_OS_BUILD);
                cb.PAR32(stringLength);
                cb.GlobalIndex(0);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            byte[] data = response.PayLoad;
            int index = Array.IndexOf(data, (byte)0);
            return Encoding.UTF8.GetString(data, 0, index);
        }

        //CMD: GET_HW_VERS = 0x09
        //Arguments
        //(Data32) LENGTH – Maximal length of string returned(-1 = No check)
        //Return
        //(Data8) DESTINATION – String variable or handle to string
        //Description
        //Enable reading the hardware version on the given hardware
        internal static async Task<string> GetHardwareVersion(ISocket socket, int stringLength = -1)
        {

            if (stringLength < 1 || stringLength > 255) stringLength = 255;
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, (ushort)stringLength, 0))
            {
                cb.OpCode(OP.opUI_READ);
                cb.Raw((byte)UI_READ_SUBCODE.GET_HW_VERS);
                cb.PAR32(stringLength);
                cb.GlobalIndex(0);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            byte[] data = response.PayLoad;
            int index = Array.IndexOf(data, (byte)0);
            return Encoding.UTF8.GetString(data, 0, index);

        }
        #endregion

        //CMD: GET_WARNING = 0x11
        //Return 
        //(Data8) WARNING – Bit field containing various warnings
        //Description
        //Read warning bit field.
        //Warnings:
        //0x01 : Warning temperature
        //0x02 : Warning current
        //0x04 : Warning voltage
        //0x08 : Warning memory
        //0x10 : Warning DSPSTAT
        //0x20 : Warning RAM
        //0x40 : Warning battery low
        //0x80 : Warning busy
        //0x3F : Warnings
        internal static async Task<IEnumerable<Warning>> GetWarnings(ISocket socket)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, 1, 0))
            {
                GetWarning_BatchCommand(cb, 0);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            byte bitField = response.PayLoad[0];
            if (bitField == 0) return new List<Warning> { Warning.None };
            Warning warning = (Warning)bitField;
            return warning.GetFlags();
        }

        internal static ushort GetWarning_BatchCommand(PayLoadBuilder payLoadBuilder, int index)
        {
            payLoadBuilder.Raw((byte)OP.opUI_READ);
            payLoadBuilder.Raw((byte)UI_READ_SUBCODE.GET_WARNING);
            payLoadBuilder.GlobalIndex(0);
            return DataType.DATA8.ByteLength();
        }

        internal static IEnumerable<Warning> GetFlags(this Warning input)
        {
            foreach (Warning value in Enum.GetValues(input.GetType()))
            {
                if (value == Warning.None || value == Warning.All) continue;
                if (input.HasFlag(value)) yield return value;
            }
        }

        //CMD: GET_VERSION = 0x1A
        //Arguments
        //(Data32) LENGTH – Maximal length of string returned(-1 = No check)
        //Return
        //(Data8) DESTINATION – String variable or handle to string
        //Description
        //Enable reading the hardware version on the given hardware

        internal static async Task<string> GetVersion(ISocket socket, int stringLength = -1)
        {

            if (stringLength < 1 || stringLength > 255) stringLength = 255;
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, (ushort)stringLength, 0))
            {
                cb.OpCode(OP.opUI_READ);
                cb.Raw((byte)UI_READ_SUBCODE.GET_VERSION);
                cb.PAR32(stringLength);
                cb.GlobalIndex(0);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            byte[] data = response.PayLoad;
            int index = Array.IndexOf(data, (byte)0);
            return Encoding.UTF8.GetString(data, 0, index);
        }

        //CMD: GET_IP = 0x1B
        //Arguments
        //(Data32) LENGTH – Maximal length of string returned(-1 = No check)
        //Return
        //(Data8) DESTINATION – String variable or handle to string
        //Description
        //Enable reading the IP address when available
        internal static async Task<string> GetIP(ISocket socket, int stringLength = -1)
        {

            if (stringLength < 1 || stringLength > 255) stringLength = 255;
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, (ushort)stringLength, 0))
            {
                cb.OpCode(OP.opUI_READ);
                cb.Raw((byte)UI_READ_SUBCODE.GET_IP);
                cb.PAR32(stringLength);
                cb.GlobalIndex(0);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            byte[] data = response.PayLoad;
            int index = Array.IndexOf(data, (byte)0);
            return Encoding.UTF8.GetString(data, 0, index);
        }



    







        //        CMD: GET_IMOTOR = 0x07
        //Return
        //(DataF) Value – Motor current[A]

        //        Instruction opUI_Read(CMD, …)
        //Opcode 0x81
        //Arguments(Data8) CMD => Specific command parameter documented below
        //Dispatch status Can change to BUSYBREAK or FAILBREAK
        //Description User interface read entry



       


        //CMD: GET_EVENT = 0x04
        //Return
        //(Data8) EVENT– Event, [1,2 = Bluetooth event], Internal use



        //        CMD: GET_STRING = 0x08
        //Arguments
        //(Data8) LENGTH – Maximal length of string to be returned
        //Return
        //(Data) DESTINATION – String variable or handle to string
        //Description
        //Enable getting a string from the terminal





   



        //CMD: GET_ADDRESS = 0x0D
        //Arguments
        //(Data32) VALUE – Address from lms_cmdin
        //Description
        //Get address from terminal, Command used for advanced debugging.

        //CMD: GET_CODE = 0x0E
        //Arguments
        //(Data32) LENGTH – Maximal code stream length
        //Return
        //(Data32) *IMAGE – Address of image
        //(Data32) *GLOBAL – Address of global variables
        //(Data8) FLAG – Flag tells if image is ready to execute, [1: Ready]
        //        Description
        //Get code from terminal, Command used for advanced debugging.

        //CMD: KEY = 0x0F
        //Arguments
        //(Data8) VALUE – Key from lms_cmdin(0 = no key)
        //Description
        //Get key from terminal.Command used for advanced debugging.

        //CMD: GET_SHUTDOWN= 0x10
        //Return
        //(Data8) FLAG – Flag[Want to shutdown]
        //Description
        //Read warning bit Get and clear shutdown flag(Internal use).





        //CMD: TEXTBOX_READ = 0x15
        //Arguments
        //(Data8) TEXT – First character in text box text(Must be zero terminated)
        //(Data32) SIZE – Maximal text size
        //(Data8) DEL – Delimiter code
        //(Data8) LENGTH- Maximal length of string returned(-1 = No Check)
        //(Data16) LINE – Selected line number
        //Return
        //(Data8) DESTINATION – String variable or handle to string
        //Description
        //Enable reading line from text box


    }
}
