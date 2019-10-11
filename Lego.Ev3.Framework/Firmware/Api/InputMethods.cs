using Lego.Ev3.Framework.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework.Firmware
{

    /*
     * 
     *  //    Input
  OC(   opINPUT_SAMPLE,         PAR32,PAR16,PAR16,PAR8,PAR8,PAR8,PAR8,PARF                            ),
  OC(   opINPUT_DEVICE_LIST,    PAR8,PAR8,PAR8,                                 0,0,0,0,0             ),
  OC(   opINPUT_DEVICE,         PAR8,SUBP,INPUT_SUBP,                           0,0,0,0,0             ),
  OC(   opINPUT_READ,           PAR8,PAR8,PAR8,PAR8,PAR8,                       0,0,0                 ),
  OC(   opINPUT_READSI,         PAR8,PAR8,PAR8,PAR8,PARF,                       0,0,0                 ),
  OC(   opINPUT_TEST,           PAR8,                                           0,0,0,0,0,0,0         ),
  OC(   opINPUT_TEST,           PAR8,PAR8,PAR8,                                 0,0,0,0,0             ),
  OC(   opINPUT_READY,          PAR8,PAR8,                                      0,0,0,0,0,0           ),
  OC(   opINPUT_READEXT,        PAR8,PAR8,PAR8,PAR8,PAR8,PARNO,                 0,0                   ),
  OC(   opINPUT_WRITE,          PAR8,PAR8,PAR8,PAR8,                            0,0,0,0               ),
     * 
     * 
     *   //    Input
  SC(   INPUT_SUBP,             GET_TYPEMODE,           PAR8,PAR8,PAR8,PAR8,                            0,0,0,0               ),
  SC(   INPUT_SUBP,             GET_CONNECTION,         PAR8,PAR8,PAR8,                                 0,0,0,0,0             ),
  SC(   INPUT_SUBP,             GET_NAME,               PAR8,PAR8,PAR8,PAR8,                            0,0,0,0               ),
  SC(   INPUT_SUBP,             GET_SYMBOL,             PAR8,PAR8,PAR8,PAR8,                            0,0,0,0               ),
  SC(   INPUT_SUBP,             GET_FORMAT,             PAR8,PAR8,PAR8,PAR8,PAR8,PAR8,                  0,0                   ),
  SC(   INPUT_SUBP,             GET_RAW,                PAR8,PAR8,PAR32,                                0,0,0,0,0             ),
  SC(   INPUT_SUBP,             GET_MODENAME,           PAR8,PAR8,PAR8,PAR8,PAR8,                       0,0,0                 ),
  SC(   INPUT_SUBP,             SET_RAW,                PAR8,PAR8,PAR8,PAR32,                           0,0,0,0               ),
  SC(   INPUT_SUBP,             GET_FIGURES,            PAR8,PAR8,PAR8,PAR8,                            0,0,0,0               ),
  SC(   INPUT_SUBP,             GET_CHANGES,            PAR8,PAR8,PARF,                                 0,0,0,0,0             ),
  SC(   INPUT_SUBP,             CLR_CHANGES,            PAR8,PAR8,0,                                    0,0,0,0,0             ),
  SC(   INPUT_SUBP,             READY_PCT,              PAR8,PAR8,PAR8,PAR8,PARNO,                      0,0,0                 ),
  SC(   INPUT_SUBP,             READY_RAW,              PAR8,PAR8,PAR8,PAR8,PARNO,                      0,0,0                 ),
  SC(   INPUT_SUBP,             READY_SI,               PAR8,PAR8,PAR8,PAR8,PARNO,                      0,0,0                 ),
  SC(   INPUT_SUBP,             GET_MINMAX,             PAR8,PAR8,PARF,PARF,                            0,0,0,0               ),
  SC(   INPUT_SUBP,             CAL_MINMAX,             PAR8,PAR8,PAR32,PAR32,                          0,0,0,0               ),
  SC(   INPUT_SUBP,             CAL_DEFAULT,            PAR8,PAR8,0,                                    0,0,0,0,0             ),
  SC(   INPUT_SUBP,             CAL_MIN,                PAR8,PAR8,PAR32,                                0,0,0,0,0             ),
  SC(   INPUT_SUBP,             CAL_MAX,                PAR8,PAR8,PAR32,                                0,0,0,0,0             ),
  SC(   INPUT_SUBP,             GET_BUMPS,              PAR8,PAR8,PARF,                                 0,0,0,0,0             ),
  SC(   INPUT_SUBP,             SETUP,                  PAR8,PAR8,PAR8,PAR16,PAR8,PAR8,PAR8,PAR8                              ),
  SC(   INPUT_SUBP,             CLR_ALL,                PAR8,                                           0,0,0,0,0,0,0         ),
  SC(   INPUT_SUBP,             STOP_ALL,               PAR8,                                           0,0,0,0,0,0,0         ),
     * */
    /// <summary>
    /// All methods that interact with brick input ports
    /// </summary>
    internal static class InputMethods
    {
        /// <summary>
        /// Port helper method to get proper layer for port number eg. port 16 is output port on layer One
        /// </summary>
        /// <param name="port">Port 0-31</param>
        /// <returns>Chainlayer</returns>
        private static ChainLayer GetLayer(int port)
        {
            if (port >= 16)
            {
                return (ChainLayer)(int)Math.Floor((port - 16d) / 4d);
            }
            return (ChainLayer)(int)Math.Floor(((double)port) / 4d);
        }

        /// <summary>
        /// Enables a program to read all available devices on input chain
        /// </summary>
        /// <param name="socket">Socket for executing command to brick</param>
        /// <param name="maximum">Maximum number of devices types (Normally 32)</param>
        /// <returns>List of devices on chain and ports</returns>
        /// <remarks>
        /// Instruction opInput_Device_List (LENGTH, ARRAY, CHANGED)
        /// Opcode 0x98
        /// Arguments (Data8) LENGTH – Maximum number of devices types (Normally 32)
        /// Return (Data8) ARRAY – First element of data8 array of types (Normally 32)
        /// (Data8) CHANGED – Changed status
        /// Dispatch status Unchanged
        /// Description Enables a program to read all available devices on input chain
        /// </remarks>
        internal static async Task<IEnumerable<PortInfo>> PortScan(Socket socket, ushort maximum = 32)
        {
            if (maximum < 1 || maximum > 32) throw new ArgumentException("Maximum number of devices must be between 1 and 32", "maximum");
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, maximum, 0))
            {
                cb.OpCode(OP.opINPUT_DEVICE_LIST);
                cb.PAR8(maximum);
                cb.GlobalIndex(0);
                cb.GlobalIndex(1);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            List<PortInfo> list = new List<PortInfo>();
            if (response.Type == ResponseType.OK)
            {
                byte[] data = response.PayLoad;
                for (int i = 0; i < maximum; i++) //32 = 4 bricks * 8 ports.
                {
                    int d = data[i];
                    if (data[i] <= 120 && Enum.IsDefined(typeof(DeviceType), d))
                    {
                        list.Add(new PortInfo(i, (DeviceType)d));
                    }
                    else list.Add(new PortInfo(i, d));

                }

                //TODO full return includes change status
                //(Data8) ARRAY – First element of data8 array of types (Normally 32) (Data8) CHANGED – Changed status
            }
            return list;
        }

        /// <summary>
        /// Read information about external device
        /// </summary>
        /// <param name="socket">Socket for executing command to brick</param>
        /// <param name="port">Port number [0-31]</param>
        /// <returns>Format</returns>
        /// <remarks>
        /// Instruction opInput_Device (CMD, …)
        /// Opcode 0x99
        /// CMD: GET_FORMAT = 0x02
        /// Arguments
        /// (Data8) LAYER – Specify chain layer number [0-3]
        /// (Data8) NO – Port number
        /// Returns
        /// (Data8) Datasets – Number of data sets
        /// (Data8) FORMAT – Format [0-3], (0: 8-bit, 1: 16-bit, 2: 32-bit, 3: Float point)
        /// (Data8) MODES – Number of modes [1-8]
        /// (Data8) VIEW – Number of modes visible within port view app [1-8]
        /// </remarks>
        internal static async Task<Format> GetFormat(Socket socket, int port)
        {
            if (port < 0 || port > 31) throw new ArgumentException("Number of port must be between 0 and 31", "port");
            ChainLayer layer = GetLayer(port);

            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, 4, 0))
            {
                cb.OpCode(OP.opINPUT_DEVICE);
                cb.Raw((byte)INPUT_DEVICE_SUBCODE.GET_FORMAT);
                cb.PAR8((byte)layer);
                cb.PAR8((byte)port);
                cb.GlobalIndex(0);
                cb.GlobalIndex(1);
                cb.GlobalIndex(2);
                cb.GlobalIndex(3);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            if (response.Type == ResponseType.OK)
            {
                byte[] data = response.PayLoad;
                return new Format(data[0], (DataType)data[1], data[2], data[3]);
            }
            return new Format(-1, DataType.NONE, -1, -1);
        }

        /// <summary>
        /// Apply new minimum and maximum raw value for device type to be used in scaling PCT and SI value
        /// </summary>
        /// <param name="socket">Socket for executing command to brick</param>
        /// <param name="type">Device type</param>
        /// <param name="mode">Device mode [0-7]</param>
        /// <param name="minimum">32 bit raw minimum value (Zero point)</param>
        /// <param name="maximum">32 bit raw maximum value (Full scale)</param>
        /// <returns></returns>
        /// <remarks>
        /// Instruction opInput_Device (CMD, …)
        /// Opcode 0x99
        /// CMD: CAL_MINMAX = 0x03
        /// Arguments
        /// (Data8) TYPE – Device type (Please reference section 0)
        /// (Data8) MODE – Device mode [0-7]
        /// (Data32) CAL_MIN – 32 bit raw minimum value (Zero point)
        /// (Data32) CAL_MAX – 32 bit raw maximum value (Full scale)
        /// Description
        /// Apply new minimum and maximum raw value for device type to be used in scaling PCT and SI value
        /// </remarks>
        internal static async Task SetMinMax(Socket socket, DeviceType type, DeviceMode mode, int minimum, int maximum)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.OpCode(OP.opINPUT_DEVICE);
                cb.Raw((byte)INPUT_DEVICE_SUBCODE.CAL_MINMAX);
                cb.PAR8((byte)type);
                cb.PAR8((byte)mode);
                cb.PAR32(minimum);
                cb.PAR32(maximum);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }

        /// <summary>
        /// Apply the default minimum and maximum raw value for device type to be used in scaling PCT and SI value
        /// </summary>
        /// <param name="socket">Socket for executing command to brick</param>
        /// <param name="type">Device type</param>
        /// <param name="mode">Device mode [0-7]</param>
        /// <returns></returns>
        /// <remarks>
        /// Instruction opInput_Device (CMD, …)
        /// Opcode 0x99
        /// CMD: CAL_DEFAULT = 0x04
        /// Arguments
        /// (Data8) TYPE – Device type (Please reference section 0)
        /// (Data8) MODE – Device mode [0-7]
        /// Description
        /// Apply the default minimum and maximum raw value for device type to be used in scaling PCT and SI value
        /// </remarks>
        internal static async Task SetDefaultMinMax(Socket socket, DeviceType type, DeviceMode mode)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.OpCode(OP.opINPUT_DEVICE);
                cb.Raw((byte)INPUT_DEVICE_SUBCODE.CAL_DEFAULT);
                cb.PAR8((byte)type);
                cb.PAR8((byte)mode);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }

        /// <summary>
        /// Reads the type and mode of the connected device
        /// </summary>
        /// <param name="socket">Socket for executing command to brick</param>
        /// <param name="port">Port [0-31]</param>
        /// <returns>TypeMode</returns>
        /// <remarks>
        /// Instruction opInput_Device (CMD, …)
        /// Opcode 0x99
        /// CMD: GET_TYPEMODE = 0x05
        /// Arguments
        /// (Data8) LAYER – Specify chain layer number [0-3]
        /// (Data8) NO – Port number
        /// Returns
        /// (Data8) TYPE – See device type list (Please reference section 0)
        /// (Data8) MODE – Device mode [0-7]
        /// </remarks>
        internal static async Task<DeviceTypeMode> GetTypeMode(Socket socket, int port)
        {
            if (port < 0 || port > 31) throw new ArgumentException("Number of port must be between 0 and 31", "port");
            ChainLayer layer = GetLayer(port);

            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, 2, 0))
            {
                cb.OpCode(OP.opINPUT_DEVICE);
                cb.Raw((byte)INPUT_DEVICE_SUBCODE.GET_TYPEMODE);
                cb.PAR8((byte)layer);
                cb.PAR8((byte)port);
                cb.GlobalIndex(0);
                cb.GlobalIndex(1);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            if (response.Type == ResponseType.OK)
            {
                byte[] data = response.PayLoad;
                return new DeviceTypeMode((DeviceType)data[0], (DeviceMode)data[1]);
            }
            return new DeviceTypeMode();
        }

        /// <summary>
        /// Apply new minimum raw value for device type to be used in scaling PCT and SI value
        /// </summary>
        /// <param name="socket">Socket for executing command to brick</param>
        /// <param name="type">Device type</param>
        /// <param name="mode">Device mode [0-7]</param>
        /// <param name="minimum">32 bit raw minimum value (Zero point)</param>
        /// <returns></returns>
        /// <remarks>
        /// Instruction opInput_Device (CMD, …)
        /// Opcode 0x99
        /// CMD: CAL_MIN = 0x07
        /// Arguments
        /// (Data8) TYPE – Device type (Please reference section 0)
        /// (Data8) MODE – Device mode [0-7]
        /// (Data32) CAL_MIN – 32 bit raw minimum value (Zero point)
        /// Description
        /// Apply new minimum raw value for device type to be used in scaling PCT and SI value
        /// </remarks>
        internal static async Task SetMin(Socket socket, DeviceType type, DeviceMode mode, int minimum)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.OpCode(OP.opINPUT_DEVICE);
                cb.Raw((byte)INPUT_DEVICE_SUBCODE.CAL_MIN);
                cb.PAR8((byte)type);
                cb.PAR8((byte)mode);
                cb.PAR32(minimum);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }

        /// <summary>
        /// Apply new minimum raw value for device type to be used in scaling PCT and SI value
        /// </summary>
        /// <param name="socket">Socket for executing command to brick</param>
        /// <param name="type">Device type</param>
        /// <param name="mode">Device mode [0-7]</param>
        /// <param name="maximum">32 bit raw maximum value (Full scale)</param>
        /// <returns></returns>
        /// <remarks>
        /// Instruction opInput_Device (CMD, …)
        /// Opcode 0x99
        /// CMD: CAL_MAX = 0x08
        /// Arguments
        /// (Data8) TYPE – Device type (Please reference section 0)
        /// (Data8) MODE – Device mode [0-7]
        /// (Data32) CAL_MAX – 32 bit raw maximum value (Full scale)
        /// Description
        /// Apply new minimum raw value for device type to be used in scaling PCT and SI value
        /// </remarks>
        internal static async Task SetMax(Socket socket, DeviceType type, DeviceMode mode, int maximum)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.OpCode(OP.opINPUT_DEVICE);
                cb.Raw((byte)INPUT_DEVICE_SUBCODE.CAL_MAX);
                cb.PAR8((byte)type);
                cb.PAR8((byte)mode);
                cb.PAR32(maximum);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }

        /// <summary>
        /// Clear all device counters and values
        /// </summary>
        /// <param name="socket">Socket for executing command to brick</param>
        /// <param name="layer">Specify chain layer number [0 - 3]</param>
        /// <returns></returns>
        /// <remarks>
        /// Instruction opInput_Device (CMD, …)
        /// Opcode 0x99
        /// CMD: CLR_ALL = 0x0A
        /// Arguments
        /// (Data8) LAYER – Specify chain layer number [0-3] (-1 = All)
        /// Description
        /// Clear all device counters and values
        /// </remarks>
        internal static async Task Reset(Socket socket, ChainLayer layer)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.OpCode(OP.opINPUT_DEVICE);
                cb.Raw((byte)INPUT_DEVICE_SUBCODE.CLR_ALL);
                cb.PAR8((byte)layer);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }

        /// <summary>
        /// Clear all device counters and values on all layers
        /// </summary>
        /// <param name="socket">Socket for executing command to brick</param>
        /// <returns></returns>
        /// <remarks>
        /// Instruction opInput_Device (CMD, …)
        /// Opcode 0x99
        /// CMD: CLR_ALL = 0x0A
        /// Arguments
        /// (Data8) LAYER – Specify chain layer number [0-3] (-1 = All)
        /// Description
        /// Clear all device counters and values
        /// </remarks>
        internal static async Task Reset(Socket socket)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                int all = -1;
                cb.OpCode(OP.opINPUT_DEVICE);
                cb.Raw((byte)INPUT_DEVICE_SUBCODE.CLR_ALL);
                cb.PAR8((byte)all);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }

        /// <summary>
        /// Method called from autopoll to build batch command
        /// </summary>
        internal static DataType GetRawValue_BatchCommand(PayLoadBuilder dataBuilder, ChainLayer layer, int port, int index)
        {
            dataBuilder.Raw((byte)OP.opINPUT_DEVICE);
            dataBuilder.Raw((byte)INPUT_DEVICE_SUBCODE.GET_RAW);
            dataBuilder.PAR8((byte)layer);
            dataBuilder.PAR8((byte)port);
            dataBuilder.GlobalIndex(index);
            return DataType.DATA32;
        }

        /// <summary>
        /// Gets the 32 bit raw value on the given sensor port
        /// </summary>
        /// <param name="socket">Socket for executing command to brick</param>
        /// <param name="port">Port</param>
        /// <returns></returns>
        /// <remarks>
        /// Instruction opInput_Device (CMD, …)
        /// Opcode 0x99
        /// CMD: GET_RAW = 0x0B
        /// Arguments
        /// (Data8) LAYER – Specify chain layer number [0-3]
        /// (Data8) NO – Port number
        /// Returns
        /// (Data32) Value – 32 bit raw value on the given sensor port
        /// </remarks>
        internal static async Task<int> GetRawValue(Socket socket, int port)
        {
            if (port < 0 || port > 31) throw new ArgumentException("Number of port must be between 0 and 31", "port");
            ChainLayer layer = GetLayer(port);

            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, 4, 0))
            {
                GetRawValue_BatchCommand(cb, layer, port, 0);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            int value = 0;
            if (response.Type == ResponseType.OK)
            {
                byte[] data = response.PayLoad;
                if (data.Length > 0)
                {
                    value = BitConverter.ToInt32(data, 0);
                }
            }
            return value;
        }

        /// <summary>
        /// Gets the connection type for the given port
        /// </summary>
        /// <param name="socket">Socket for executing command to brick</param>
        /// <param name="port">Port number [0 - 31]</param>
        /// <returns></returns>
        /// <remarks>
        /// Instruction opInput_Device (CMD, …)
        /// Opcode 0x99
        /// CMD: GET_CONNECTION = 0x0C
        /// Arguments
        /// (Data8) LAYER – Specify chain layer number [0-3]
        /// (Data8) NO – Port number
        /// Returns
        /// (Data8) CONN – Connection type
        /// </remarks>
        internal static async Task<ConnectionType> GetConnection(Socket socket, int port)
        {
            if (port < 0 || port > 31) throw new ArgumentException("Number of port must be between 0 and 31", "port");
            ChainLayer layer = GetLayer(port);

            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, 1, 0))
            {
                cb.OpCode(OP.opINPUT_DEVICE);
                cb.Raw((byte)INPUT_DEVICE_SUBCODE.GET_CONNECTION);
                cb.PAR8((byte)layer);
                cb.PAR8((ushort)port);
                cb.GlobalIndex(0);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            ConnectionType type = ConnectionType.CONN_UNKNOW;
            if (response.Type == ResponseType.OK)
            {
                byte[] data = response.PayLoad;
                if (data.Length > 0)
                {
                    type = (ConnectionType)data[0];
                }
            }
            return type;
        }

        /// <summary>
        /// Stop all input devices
        /// </summary>
        /// <param name="socket">Socket for executing command to brick</param>
        /// <param name="layer">Specify chain layer number [0 - 3]</param>
        /// <returns></returns>
        /// <remarks>
        /// Instruction opInput_Device (CMD, …)
        /// Opcode 0x99
        /// CMD: STOP_ALL= 0x0D (Stop all devices)
        /// Arguments
        /// (Data8) LAYER – Specify chain layer number [0-3] (-1 = All)
        /// </remarks>
        internal static async Task Stop(Socket socket, ChainLayer layer)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.OpCode(OP.opINPUT_DEVICE);
                cb.Raw((byte)INPUT_DEVICE_SUBCODE.STOP_ALL);
                cb.PAR8((byte)layer);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }

        /// <summary>
        /// Stop all input devices on all layers
        /// </summary>
        /// <param name="socket">Socket for executing command to brick</param>
        /// <returns></returns>
        /// <remarks>
        /// Instruction opInput_Device (CMD, …)
        /// Opcode 0x99
        /// CMD: STOP_ALL= 0x0D (Stop all devices)
        /// Arguments
        /// (Data8) LAYER – Specify chain layer number [0-3] (-1 = All)
        /// </remarks>
        internal static async Task Stop(Socket socket)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                int all = -1;
                cb.OpCode(OP.opINPUT_DEVICE);
                cb.Raw((byte)INPUT_DEVICE_SUBCODE.STOP_ALL);
                cb.PAR8((byte)all);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }




        #region TouchSensor

        /// <summary>
        /// Method called from autopoll to build batch command
        /// </summary>
        /// <returns>Value type on return after executing command</returns>
        internal static DataType GetChangesValue_BatchCommand(PayLoadBuilder dataBuilder, ChainLayer layer, int port, int index)
        {
            dataBuilder.Raw((byte)OP.opINPUT_DEVICE);
            dataBuilder.Raw((byte)INPUT_DEVICE_SUBCODE.GET_CHANGES);
            dataBuilder.PAR8((byte)layer);
            dataBuilder.PAR8((byte)port);
            dataBuilder.GlobalIndex(index);
            return DataType.DATAF;

        }

        /// <summary>
        /// Gets the Positive changes since last clear. ( Button pressed)
        /// </summary>
        /// <param name="socket">Socket for executing command to brick</param>
        /// <param name="port">Port number [0 - 31]</param>
        /// <returns></returns>
        /// <remarks>
        /// Instruction opInput_Device (CMD, …)
        /// Opcode 0x99
        /// CMD: GET_CHANGES = 0x19
        /// Arguments
        /// (Data8) LAYER – Specify chain layer number [0-3]
        /// (Data8) NO – Port number
        /// Returns
        /// (DataF) VALUE1 – Positive changes since last clear. ( Button pressed)
        /// </remarks>
        internal static async Task<int> GetChanges(Socket socket, int port)
        {
            if (port < 0 || port > 31) throw new ArgumentException("Number of port must be between 0 and 31", "port");
            ChainLayer layer = GetLayer(port);
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, 1, 0))
            {
                GetChangesValue_BatchCommand(cb, layer, port, 0);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            float value = 0;
            if (response.Type == ResponseType.OK)
            {
                byte[] data = response.PayLoad;
                if (data.Length > 1)
                {
                    value = BitConverter.ToSingle(data, 0);
                }
            }
            return (int)value;
        }

        /// <summary>
        /// Clear changes and bumps
        /// </summary>
        /// <param name="socket">Socket for executing command to brick</param>
        /// <param name="port">Port number [0 - 31]</param>
        /// <returns></returns>
        /// <remarks>
        /// Instruction opInput_Device (CMD, …)
        /// Opcode 0x99
        /// CMD: CLR_CHANGES = 0x1A
        /// Arguments
        /// (Data8) LAYER – Specify chain layer number [0-3]
        /// (Data8) NO – Port number
        /// Description
        /// Clear changes and bumps
        /// </remarks>
        internal static async Task ClearChanges(Socket socket, int port)
        {
            if (port < 0 || port > 31) throw new ArgumentException("Number of port must be between 0 and 31", "port");
            ChainLayer layer = GetLayer(port);
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.OpCode(OP.opINPUT_DEVICE);
                cb.Raw((byte)INPUT_DEVICE_SUBCODE.CLR_CHANGES);
                cb.PAR8((byte)layer);
                cb.PAR8((ushort)port);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }

        /// <summary>
        /// Method called from autopoll to build batch command
        /// </summary>
        /// <returns>Value type on return after executing command</returns>
        internal static DataType GetBumpsValue_BatchCommand(PayLoadBuilder dataBuilder, ChainLayer layer, int port, int index)
        {
            dataBuilder.Raw((byte)OP.opINPUT_DEVICE);
            dataBuilder.Raw((byte)INPUT_DEVICE_SUBCODE.GET_BUMPS);
            dataBuilder.PAR8((byte)layer);
            dataBuilder.PAR8((byte)port);
            dataBuilder.GlobalIndex(index);
            return DataType.DATAF;

        }

        /// <summary>
        /// Gets the Negative changes since last clear. (Button release)
        /// </summary>
        /// <param name="socket">Socket for executing command to brick</param>
        /// <param name="port">Port number [0 - 31]</param>
        /// <returns></returns>
        /// <remarks>
        /// Instruction opInput_Device (CMD, …)
        /// Opcode 0x99
        /// CMD: GET_BUMPS = 0x1F
        /// Arguments
        /// (Data8) LAYER – Specify chain layer number [0-3]
        /// (Data8) NO – Port number
        /// Returns
        /// (DataF) VALUE1 – Negative changes since last clear. (Button release)
        /// </remarks>
        internal static async Task<int> GetBumps(Socket socket, int port)
        {
            if (port < 0 || port > 31) throw new ArgumentException("Number of port must be between 0 and 31", "port");
            ChainLayer layer = GetLayer(port);
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, 1, 0))
            {
                GetBumpsValue_BatchCommand(cb, layer, port, 0);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            float value = 0;
            if (response.Type == ResponseType.OK)
            {
                byte[] data = response.PayLoad;
                if (data.Length > 1)
                {
                    value = BitConverter.ToSingle(data, 0);
                }
            }
            //Bumps are negative, but output them positive :)
            return -1 * (int)value;
        }

        #endregion


        /// <summary>
        /// Method called from autopoll to build batch command
        /// </summary>
        internal static DataType GetReadySIValue_BatchCommand(PayLoadBuilder dataBuilder, ChainLayer layer, int port, int type = 0, int mode = -1, int numberofValues = 1, int index = 0)
        {
            dataBuilder.Raw((byte)OP.opINPUT_DEVICE);
            dataBuilder.Raw((byte)INPUT_DEVICE_SUBCODE.READY_SI);
            dataBuilder.PAR8((byte)layer);
            dataBuilder.PAR8((byte)port);
            dataBuilder.PAR8((byte)type);
            dataBuilder.PAR8((byte)mode);
            dataBuilder.PAR8((byte)numberofValues);
            dataBuilder.GlobalIndex(index);
            return DataType.DATAF;
        }

        /// <summary>
        /// Description This function enables reading specific device and mode in SI units
        /// </summary>
        /// <param name="socket">Socket for executing command to brick</param>
        /// <param name="port">Port</param>
        /// <param name="type">Type of the device (0 = Don’t change type)</param>
        /// <param name="mode"> Device mode [0 - 7] (-1 = Don’t change mode)</param>
        /// <returns></returns>
        /// <remarks>
        /// CMD: READY_SI = 0x1D
        /// Arguments
        /// (Data8) LAYER – Specify chain layer number [0-3]
        /// (Data8) NO – Port number
        /// (Data8) TYPE – Specify device type (0 = Don’t change type)
        /// (Data8) MODE – Device mode [0-7] (-1 = Don’t change mode)
        /// (Data8) VALUES – Number of return values
        /// Returns (Depending on number of data samples requested in (VALUES))
        /// (DataF) VALUE1 – First value received from sensor in the specified mode
        /// </remarks>
        internal static async Task<float> GetReadySIValue(Socket socket, int port, int type = 0, int mode = -1)
        {
            if (port < 0 || port > 31) throw new ArgumentException("Number of port must be between 0 and 31", "port");
            ChainLayer layer = GetLayer(port);

            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, 4, 0))
            {
                GetReadySIValue_BatchCommand(cb, layer, port, type, mode);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            float value = 0;
            if (response.Type == ResponseType.OK)
            {
                byte[] data = response.PayLoad;
                if (data.Length > 0)
                {
                    value = BitConverter.ToSingle(data, 0);
                }
            }
            return value;
        }



        /// <summary>
        /// Method called from autopoll to build batch command
        /// </summary>
        internal static DataType GetSIValue_BatchCommand(PayLoadBuilder dataBuilder, ChainLayer layer, int port, ushort type = 0, int mode = -1, int index = 1)
        {
            dataBuilder.Raw((byte)OP.opINPUT_READSI);
            dataBuilder.PAR8((byte)layer);
            dataBuilder.PAR8((byte)port);
            dataBuilder.PAR8((byte)type);
            dataBuilder.PAR8((byte)mode);
            dataBuilder.GlobalIndex(index);
            return DataType.DATAF;
        }

        /// <summary>
        /// This function enables reading specific device and mode in SI units
        /// </summary>
        /// <param name="socket">Socket for executing command to brick</param>
        /// <param name="port">Port</param>
        /// <param name="type">Type of the device (0 = Don’t change type)</param>
        /// <param name="mode"> Device mode [0 - 7] (-1 = Don’t change mode)</param>
        /// <returns></returns>
        /// <remarks>
        /// Instruction opInput_ReadSI (LAYER, NO, TYPE, MODE, SI)
        /// Opcode 0x9D
        /// Arguments (Data8) LAYER – Specify chain layer number [0 - 3]
        /// (Data8) NO – Port number
        /// (Data8) TYPE – Specify device type (0 = Don’t change type)
        /// (Data8) MODE – Device mode [0 - 7] (-1 = Don’t change mode)
        /// Return (DataF) SI – SI unit value from device
        /// Dispatch status Unchanged
        /// Description This function enables reading specific device and mode in SI units
        /// </remarks>
        internal static async Task<float> GetSIValue(Socket socket, int port, ushort type = 0, int mode = -1)
        {
            if (port < 0 || port > 31) throw new ArgumentException("Number of port must be between 0 and 31", "port");
            ChainLayer layer = GetLayer(port);

            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, 4, 0))
            {
                GetSIValue_BatchCommand(cb, layer, port, type, mode, 1);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            float value = 0;
            if (response.Type == ResponseType.OK)
            {
                byte[] data = response.PayLoad;
                if (data.Length > 0)
                {
                    value = BitConverter.ToSingle(data, 0);
                }
            }
            return value;
        }


        /// <summary>
        /// Method called from autopoll to build batch command
        /// </summary>
        internal static DataType GetReadyRaw_BatchCommand(PayLoadBuilder dataBuilder, ChainLayer layer, int port, ushort type = 0, int mode = -1, int numberOfValues = 1, int index = 1)
        {
            dataBuilder.Raw((byte)OP.opINPUT_DEVICE);
            dataBuilder.Raw((byte)INPUT_DEVICE_SUBCODE.READY_RAW);
            dataBuilder.PAR8((byte)layer);
            dataBuilder.PAR8((byte)port);
            dataBuilder.PAR8((byte)type);
            dataBuilder.PAR8((byte)mode);
            dataBuilder.PAR8((byte)numberOfValues);
            dataBuilder.GlobalIndex(index);
            return DataType.DATA32;
        }

        /// <summary>
        /// This function enables reading specific device and mode
        /// </summary>
        /// <param name="socket">Socket for executing command to brick</param>
        /// <param name="port">Port</param>
        /// <param name="type">Type of the device (0 = Don’t change type)</param>
        /// <param name="mode"> Device mode [0 - 7] (-1 = Don’t change mode)</param>
        /// <param name="numberOfValues">Number of values to expect in return</param>
        /// <returns></returns>
        /// <remarks>
        /// Instruction opInput_ReadSI (LAYER, NO, TYPE, MODE, SI)
        /// Opcode 0x9D
        /// CMD: READY_RAW = 0x1C
        ///Arguments
        ///(Data8) LAYER – Specify chain layer number[0 - 3]
        ///(Data8) NO – Port number
        ///(Data8) TYPE – Specify device type(0 = Don’t change type)
        ///(Data8) MODE – Device mode[0 - 7] (-1 = Don’t change mode)
        ///(Data8) VALUES – Number of return values
        ///Returns(Depending on number of data samples requested in (VALUES))
        ///(Data32) VALUE1 – First value received from sensor in the specified mode
        /// </remarks>
        internal static async Task<int> GetReadyRaw(Socket socket, int port, ushort type = 0, int mode = -1, int numberOfValues = 1)
        {
            if (port < 0 || port > 31) throw new ArgumentException("Number of port must be between 0 and 31", "port");
            ChainLayer layer = GetLayer(port);

            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, 4, 0))
            {
                GetReadyRaw_BatchCommand(cb, layer, port, type, mode, numberOfValues, 1);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            int value = 0;
            if (response.Type == ResponseType.OK)
            {
                byte[] data = response.PayLoad;
                if (data.Length > 0)
                {
                    value = BitConverter.ToInt32(data, 0);
                }
            }
            return value;
        }


        /// <summary>
        /// Method called from autopoll to build batch command
        /// </summary>
        internal static DataType GetReadyPct_BatchCommand(PayLoadBuilder dataBuilder, ChainLayer layer, int port, ushort type = 0, int mode = -1, int numberOfValues = 1, int index = 1)
        {
            dataBuilder.Raw((byte)OP.opINPUT_DEVICE);
            dataBuilder.Raw((byte)INPUT_DEVICE_SUBCODE.READY_PCT);
            dataBuilder.PAR8((byte)layer);
            dataBuilder.PAR8((byte)port);
            dataBuilder.PAR8((byte)type);
            dataBuilder.PAR8((byte)mode);
            dataBuilder.PAR8((byte)numberOfValues);
            dataBuilder.GlobalIndex(index);
            return DataType.DATA32;
        }

        /// <summary>
        /// This function enables reading specific device and mode in Pct
        /// </summary>
        /// <param name="socket">Socket for executing command to brick</param>
        /// <param name="port">Port</param>
        /// <param name="type">Type of the device (0 = Don’t change type)</param>
        /// <param name="mode"> Device mode [0 - 7] (-1 = Don’t change mode)</param>
        /// <param name="numberOfValues">Number of values to expect in return</param>
        /// <returns></returns>
        /// <remarks>
        /// Instruction 
        ///        opINPUT_DEVICE
        ///         CMD: READY_PCT = 0x1B
        ///Arguments
        ///(Data8) LAYER – Specify chain layer number [0-3]
        ///(Data8) NO – Port number
        ///(Data8) TYPE – Specify device type (0 = Don’t change type)
        ///(Data8) MODE – Device mode [0-7] (-1 = Don’t change mode)
        ///(Data8) VALUES – Number of return values
        ///Returns (Depending on number of data samples requested in (VALUES))
        ///(Data8) VALUE1 – First value received from sensor in the specified mode
        /// </remarks>
        internal static async Task<int> GetReadyPct(Socket socket, int port, ushort type = 0, int mode = -1, int numberOfValues = 1)
        {
            if (port < 0 || port > 31) throw new ArgumentException("Number of port must be between 0 and 31", "port");
            ChainLayer layer = GetLayer(port);

            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, 4, 0))
            {
                GetReadyPct_BatchCommand(cb, layer, port, type, mode, numberOfValues, 1);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            int value = 0;
            if (response.Type == ResponseType.OK)
            {
                byte[] data = response.PayLoad;
                if (data.Length > 0)
                {
                    value = BitConverter.ToInt32(data, 0);
                }
            }
            return value;
        }

        /*opINPUT_DEVICE

         * 

         * 
CMD: GET_MINMAX = 0x1E
Arguments
(Data8) LAYER – Specify chain layer number [0-3]
(Data8) NO – Port number
Returns
(DataF) MIN – Minimum SI value
(DataF) MAX – Maximum SI value
         * 
       
         * 
         * 
         * Instruction opInput_Read (LAYER, NO, TYPE, MODE, PCT)
Opcode 0x9A
Arguments (Data8) LAYER – Specify chain layer number [0-3]
(Data8) NO – Port number
(Data8) TYPE – Specify device type (0 = Don’t change type)
(Data8) MODE – Device mode [0-7] (-1 = Don’t change mode)
Return (Data8) PCT – Percentage value from device
Dispatch status Unchanged
Description This function enables reading specific device and mode in percentage
         * 
         * Instruction opInput_Test (LAYER, NO, BUSY)
Opcode 0x9B
Arguments (Data8) LAYER – Specify chain layer number [0-3]
(Data8) NO – Port number
Return (Data8) BUSY – Device busy flag (0 = Ready, 1 = Busy)
Dispatch status Unchanged
Description This function enables testing if a sensor is busy changing type or mode
         * 
         * Instruction opInput_Ready (LAYER, NO)
Opcode 0x9C
Arguments (Data8) LAYER – Specify chain layer number [0 - 3]
(Data8) NO – Port number
Dispatch status Can change to BUSYBREAK
Description This function enables a program to wait for a device to be ready (Wait for valid data)

         * 

         * Instruction opInput_ReadExt (LAYER, NO, TYPE, MODE, FORMAT, VALUES, VALUE1)
Opcode 0x9E
Arguments (Data8) LAYER – Specify chain layer number [0 - 3]
(Data8) NO – Port number
(Data8) TYPE – Specify device type (0 = Don’t change type)
(Data8) MODE – Device mode [0 - 7] (-1 = Don’t change mode)
(Data8) FORMAT – Format selection (PCT, RAW, SI,… )
(Data8) Values – Number of return values
Return (Depending on number of data samples requested in (VALUES))
(FORMAT) VALUE1 – First value received from device in specified mode
Dispatch status Unchanged
Description This function enables reading multiple data from external device simultanously
         * 
         * Instruction opInput_Write (LAYER, NO, BYTES, DATA)
Opcode 0x9F
Arguments (Data8) LAYER – Specify chain layer number [0 - 3]
(Data8) NO – Port number
(Data8) BYTES – No of bytes to write [1 - 32]
(Data8) DATA – First byte of data8 array to write
Dispatch status Can change to BUSYBREAK
Description This function enables writting data to extenal digital devices
         * 
         * 
         * */
        //TODO
        /* 
         * 
CMD: GET_SYMBOL = 0x06
Arguments
(Data8) LAYER – Specify chain layer number [0-3]
(Data8) NO – Port number
(Data8) LENGTH – Maximum length of string returned (-1 = No check)
Returns
(Data8) DESTINATION – String variable or handle to string
         * 
         * 
CMD: SETUP = 0x09
Arguments
(Data8) LAYER – Specify chain layer number [0-3]
(Data8) NO – Port number
(Data8) REPEAT – Repeat setup/read “Repeat” times (0 = Infinite)
(Data16) TIME – Time between repeats [10-1000 mS] (0 = 10)
(Data8) WRLNG – No of bytes to write
(Data8) WRDATA – Array handle (Data8) of data to write
(Data8) RDLNG – No of bytes to read
Returns
(Data8) RDDATA – Array handle (Data8) for data to read
Description
Generic setup / read for IIC sensor
         * 
         */



        /*
         * CMD: GET_NAME = 0x15
Arguments
(Data8) LAYER – Specify chain layer number [0-3]
(Data8) NO – Port number
(Data8) LENGTH – Maximum length of string returned (-1 = No check)
Returns
(Data8) DESTINATION – String variable or handle to string
         * 
         * CMD: GET_MODENAME = 0x16
Arguments
(Data8) LAYER – Specify chain layer number [0-3]
(Data8) NO – Port number
(Data8) MODE – Mode number
(Data8) LENGTH – Maximum length of string returned (-1 = No check)
Returns
(Data8) DESTINATION – String variable or handle to string
         * 
CMD: GET_FIGURES = 0x18
Arguments
(Data8) LAYER – Specify chain layer number [0-3]
(Data8) NO – Port number
Returns
(Data8) FIGURES – Total number of figures (Inclusive decimal points and decimals)
(Data8) DECIMALES – Number of decimals
         * 
         * */



    }
}
