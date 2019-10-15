using System;
using System.Threading.Tasks;
namespace Lego.Ev3.Framework.Firmware
{
    /// <summary>
    /// Methods to interact with the brick memory system.
    /// </summary>
    /// <remarks>
    /// See: LEGO® MINDSTORMS® EV3 Firmware Developer Kit 4.14 Memory operations
    /// </remarks>
    public static class MemoryMethods
    {

        /// <summary>
        /// Gets the memory usage total and free
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <returns>MemoryInfo</returns>
        /// <exception cref="FirmwareException"/>
        public static async Task<MemoryInfo> GetMemoryInfo(ISocket socket)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, 8, 0))
            {
                cb.OpCode(OP.opMEMORY_USAGE);
                cb.GlobalIndex(0);
                cb.GlobalIndex(4);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            byte[] data = response.PayLoad;
            int total = BitConverter.ToInt32(data, 0);
            int free = BitConverter.ToInt32(data, 4);

            return new MemoryInfo(total, free);
        }


        /// <summary>
        /// Check if file or folder exists
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <param name="brickPath">relative path to file or folder on brick</param>
        /// <returns><c>true</c> if file or folder exists otherwise <c>false</c></returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FirmwareException"/>
        public static async Task<bool> Exists(ISocket socket, string brickPath)
        {
            brickPath.IsBrickPath();

            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, 1, 0))
            {
                cb.OpCode(OP.opFILENAME);
                cb.Raw((byte)FILENAME_SUBCODE.EXIST);
                cb.PARS(brickPath);
                cb.GlobalIndex(0);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            return response.PayLoad[0] == 1;
        }


        /// <summary>
        /// Gets directoryinfo item count and total size
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <param name="brickDirectoryPath">relative path to folder on brick</param>
        /// <returns>DirectoryInfo</returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FirmwareException"/>
        public static async Task<DirectoryInfo> GetDirectoryInfo(ISocket socket, string brickDirectoryPath)
        {
            brickDirectoryPath = FileSystem.ToBrickDirectoryPath(brickDirectoryPath);
            brickDirectoryPath.IsBrickDirectoryPath();

            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, 8, 0))
            {
                cb.OpCode(OP.opFILENAME);
                cb.Raw((byte)FILENAME_SUBCODE.TOTALSIZE);
                cb.PARS(brickDirectoryPath);
                cb.GlobalIndex(0);
                cb.GlobalIndex(4);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            byte[] data = response.PayLoad;
            int items = BitConverter.ToInt32(data, 0);
            int size = BitConverter.ToInt32(data, 4);

            return new DirectoryInfo(items,size);
        }



    }
}
