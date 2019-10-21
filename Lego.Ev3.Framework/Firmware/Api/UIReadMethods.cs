using Lego.Ev3.Framework.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework.Firmware
{
    /// <summary>
    /// All user interface read methods
    /// </summary>
    /// <remarks>
    /// See: LEGO® MINDSTORMS® EV3 Firmware Developer Kit 4.15 User interface operations
    /// </remarks>
    public static class UIReadMethods
    {
        /// <summary>
        /// Gets battery values : voltage, current, temperature and level
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <returns></returns>
        public static async Task<BatteryValue> GetBatteryValue(ISocket socket)
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

        #region battery

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

        #endregion

        /// <summary>
        /// Gets the Operating System version
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <returns></returns>
        public static async Task<string> GetOSVersion(ISocket socket)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, 255, 0))
            {
                cb.OpCode(OP.opUI_READ);
                cb.Raw((byte)UI_READ_SUBCODE.GET_OS_VERS);
                cb.PAR32(255);
                cb.GlobalIndex(0);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            byte[] data = response.PayLoad;
            int index = Array.IndexOf(data, (byte)0);
            return Encoding.UTF8.GetString(data, 0, index);
        }

        /// <summary>
        /// Gets the Operating System version build
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <returns></returns>
        public static async Task<string> GetOSBuild(ISocket socket)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, 255, 0))
            {
                cb.OpCode(OP.opUI_READ);
                cb.Raw((byte)UI_READ_SUBCODE.GET_OS_BUILD);
                cb.PAR32(255);
                cb.GlobalIndex(0);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            byte[] data = response.PayLoad;
            int index = Array.IndexOf(data, (byte)0);
            return Encoding.UTF8.GetString(data, 0, index);
        }

        /// <summary>
        /// Gets the hardware version
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <returns></returns>
        public static async Task<string> GetHardwareVersion(ISocket socket)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, 255, 0))
            {
                cb.OpCode(OP.opUI_READ);
                cb.Raw((byte)UI_READ_SUBCODE.GET_HW_VERS);
                cb.PAR32(255);
                cb.GlobalIndex(0);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            byte[] data = response.PayLoad;
            int index = Array.IndexOf(data, (byte)0);
            return Encoding.UTF8.GetString(data, 0, index);

        }

        /// <summary>
        /// Gets the firmware version
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <returns></returns>
        public static async Task<string> GetFirmwareVersion(ISocket socket)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, 255, 0))
            {
                cb.OpCode(OP.opUI_READ);
                cb.Raw((byte)UI_READ_SUBCODE.GET_FW_VERS);
                cb.PAR32(255);
                cb.GlobalIndex(0);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            byte[] data = response.PayLoad;
            int index = Array.IndexOf(data, (byte)0);
            return Encoding.UTF8.GetString(data, 0, index);
        }

        /// <summary>
        /// Gets the firmware build
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <returns></returns>
        public static async Task<string> GetFirmwareBuild(ISocket socket)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, 255, 0))
            {
                cb.OpCode(OP.opUI_READ);
                cb.Raw((byte)UI_READ_SUBCODE.GET_FW_BUILD);
                cb.PAR32(255);
                cb.GlobalIndex(0);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            byte[] data = response.PayLoad;
            int index = Array.IndexOf(data, (byte)0);
            return Encoding.UTF8.GetString(data, 0, index);
        }

        /// <summary>
        /// Gets any warnings <see cref="Warning"/>
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <returns></returns>
        public static async Task<IEnumerable<Warning>> GetWarnings(ISocket socket)
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

        #region warning
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
        #endregion

        /// <summary>
        /// Gets the current brick version
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <returns></returns>
        public static async Task<string> GetVersion(ISocket socket)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, 255, 0))
            {
                cb.OpCode(OP.opUI_READ);
                cb.Raw((byte)UI_READ_SUBCODE.GET_VERSION);
                cb.PAR32(255);
                cb.GlobalIndex(0);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            byte[] data = response.PayLoad;
            int index = Array.IndexOf(data, (byte)0);
            return Encoding.UTF8.GetString(data, 0, index);
        }

        /// <summary>
        /// Gets the IP address when available
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <returns></returns>
        public static async Task<string> GetIPAddress(ISocket socket)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, 255, 0))
            {
                cb.OpCode(OP.opUI_READ);
                cb.Raw((byte)UI_READ_SUBCODE.GET_IP);
                cb.PAR32(255);
                cb.GlobalIndex(0);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            byte[] data = response.PayLoad;
            int index = Array.IndexOf(data, (byte)0);
            return Encoding.UTF8.GetString(data, 0, index);
        }

    }
}
