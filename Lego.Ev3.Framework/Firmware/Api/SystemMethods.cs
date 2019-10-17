using Lego.Ev3.Framework.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework.Firmware
{

    /// <summary>
    /// Methods to interact with the brick file system.
    /// </summary>
    /// <remarks>
    /// See: LEGO® MINDSTORMS® EV3 Communication Developer Kit
    /// </remarks>
    public static class SystemMethods
    {
        private const int PAYLOAD_SIZE = 960;
        private const char LIST_DELIMITER = '\n';
        private static SemaphoreSlim semaPhoreSlim = new SemaphoreSlim(1);

        /// <summary>
        /// Download a file from the brick
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <param name="brickFilePath">relative filepath on brick</param>
        /// <returns><c>byte[]</c> containing the file data</returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FirmwareException"/>
        public static async Task<byte[]> DownLoadFile(ISocket socket, string brickFilePath)
        {
            await semaPhoreSlim.WaitAsync(); //stop multithread for same file handle
            try
            {
                brickFilePath.IsBrickFilePath();

                Response response = await BeginDownload(socket, brickFilePath);
                if (response.Status == ResponseStatus.UNKNOWN_HANDLE) throw new ArgumentException("File does not exist", nameof(brickFilePath));
                if (response.Status != ResponseStatus.SUCCESS) throw new FirmwareException(response);

                int fileSize = BitConverter.ToInt32(response.PayLoad, 0);
                byte handle = response.PayLoad[4];

                List<byte> data = new List<byte>();

                int bytesLoaded = 0;
                while (bytesLoaded < fileSize)
                {
                    int payLoadSize = Math.Min(PAYLOAD_SIZE, fileSize - bytesLoaded);
                    response = await ContinueDownload(socket, handle, (ushort)payLoadSize);
                    if (!(response.Status == ResponseStatus.SUCCESS || response.Status == ResponseStatus.END_OF_FILE)) throw new FirmwareException(response);

                    if (response.PayLoad != null && response.PayLoad.Length > 1)
                    {
                        byte[] chunk = new byte[payLoadSize];
                        Array.Copy(response.PayLoad, 1, chunk, 0, payLoadSize);
                        data.AddRange(chunk);
                    }
                    bytesLoaded += payLoadSize;
                }

                return data.ToArray();
            }
            finally 
            {
                semaPhoreSlim.Release();
            } 
        }

        #region download a file from the Brick
        private static async Task<Response> BeginDownload(ISocket socket, string brickFilePath)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.SYSTEM_COMMAND_REPLY))
            {
                cb.OpCode(SYSTEM_OP.BEGIN_UPLOAD);
                cb.Raw((ushort)0); // set to 0 to just have handle and filesize returned
                cb.Raw(brickFilePath);
                cmd = cb.ToCommand();
            }
            return await socket.Execute(cmd);
        }

        private static async Task<Response> ContinueDownload(ISocket socket, byte handle, ushort payLoad)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.SYSTEM_COMMAND_REPLY))
            {
                cb.OpCode(SYSTEM_OP.CONTINUE_UPLOAD);
                cb.Raw(handle);
                cb.Raw(payLoad);
                cmd = cb.ToCommand();
            }
            return await socket.Execute(cmd);
        }
        #endregion

        /// <summary>
        /// Upload a file on to the brick
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <param name="file">byte data of source file on local machine to upload</param>
        /// <param name="brickFilePath">relative filepath on brick</param>
        /// <returns><c>true</c> if success, otherwise <c>false</c></returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FirmwareException"/>
        public static async Task<bool> UploadFile(ISocket socket, byte[] file, string brickFilePath)
        {
            await semaPhoreSlim.WaitAsync();
            try
            {
                brickFilePath.IsBrickFilePath();
                if (file == null || file.Length == 0) throw new ArgumentNullException("file data can not be empty", nameof(file));

                int fileSize = file.Length;

                Response response = await BeginUpload(socket, fileSize, brickFilePath);
                if (response.Status == ResponseStatus.FILE_EXISTS) return false; // do not override existing files

                byte handle = response.PayLoad[0];

                int bytesSent = 0;
                while (bytesSent < fileSize)
                {
                    int payLoadSize = Math.Min(PAYLOAD_SIZE, fileSize - bytesSent);
                    byte[] payLoad = new byte[payLoadSize];
                    Array.Copy(file, bytesSent, payLoad, 0, payLoadSize);
                    response = await ContinueUpload(socket, handle, payLoad);
                    if (!(response.Status == ResponseStatus.SUCCESS || response.Status == ResponseStatus.END_OF_FILE)) throw new FirmwareException(response);
                    bytesSent += payLoadSize;
                }
                return true;
            }
            finally 
            {
                semaPhoreSlim.Release();
            }
        }

        #region upload a file to the brick
        private static async Task<Response> BeginUpload(ISocket socket, int fileSize, string brickFilePath)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.SYSTEM_COMMAND_REPLY))
            {
                cb.OpCode(SYSTEM_OP.BEGIN_DOWNLOAD);
                cb.Raw((uint)fileSize);
                cb.Raw(brickFilePath);
                cmd = cb.ToCommand();
            }
            return await socket.Execute(cmd);
        }
        private static async Task<Response> ContinueUpload(ISocket socket, byte handle, byte[] payLoad)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.SYSTEM_COMMAND_REPLY))
            {
                cb.OpCode(SYSTEM_OP.CONTINUE_DOWNLOAD);
                cb.Raw(handle);
                cb.Raw(payLoad);
                cmd = cb.ToCommand();
            }
            return await socket.Execute(cmd);
        }
        #endregion

        /// <summary>
        /// Gets a list of folders and/or files for given path
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <param name="brickDirectoryPath">relative path to folder on brick</param>
        /// <returns>DirectoryContent</returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FirmwareException"/>
        public static async Task<DirectoryContent> GetDirectoryContent(ISocket socket, string brickDirectoryPath)
        {
            await semaPhoreSlim.WaitAsync();
            try
            {
                brickDirectoryPath = FileSystem.ToBrickDirectoryPath(brickDirectoryPath);
                brickDirectoryPath.IsBrickDirectoryPath();

                Command cmd = null;
                using (CommandBuilder cb = new CommandBuilder(CommandType.SYSTEM_COMMAND_REPLY))
                {
                    cb.OpCode(SYSTEM_OP.LIST_FILES);
                    cb.Raw((ushort)PAYLOAD_SIZE);
                    cb.Raw(brickDirectoryPath);
                    cmd = cb.ToCommand();
                }


                Response response = await socket.Execute(cmd);
                if (!(response.Status == ResponseStatus.SUCCESS || response.Status == ResponseStatus.END_OF_FILE)) throw new FirmwareException(response);

                byte[] data = response.PayLoad;
                ushort listSize = (ushort)BitConverter.ToUInt32(data, 0);
                byte handle = data[4];

                List<byte> list = new List<byte>();

                if (data.Length > 5)
                {
                    byte[] chunk = new byte[data.Length - 5];
                    Array.Copy(data, 5, chunk, 0, chunk.Length);
                    list.AddRange(chunk);

                    int bytesRead = chunk.Length;
                    while (bytesRead < listSize)
                    {
                        ushort payLoadSize = (ushort)Math.Min(PAYLOAD_SIZE, listSize - bytesRead);
                        response = await ContinueList(socket, handle, payLoadSize);
                        if (!(response.Status == ResponseStatus.SUCCESS || response.Status == ResponseStatus.END_OF_FILE)) throw new FirmwareException(response);

                        data = response.PayLoad;
                        if (data.Length > 1)
                        {
                            chunk = new byte[data.Length - 1];
                            Array.Copy(data, 1, chunk, 0, chunk.Length);
                            list.AddRange(chunk);
                        }
                        bytesRead += payLoadSize;
                    }
                }

                data = list.ToArray();
                string value = Encoding.UTF8.GetString(data, 0, data.Length).TrimEnd(LIST_DELIMITER);
                string[] entries = value.Split(LIST_DELIMITER);

                DirectoryContent contents = new DirectoryContent();
                List<Directory> directories = new List<Directory>();
                List<File> files = new List<File>();
                foreach (string entry in entries)
                {
                    string item = entry.Trim();
                    if (string.IsNullOrWhiteSpace(item)) continue;
                    switch (item)
                    {
                        case FileSystem.ROOT_PATH:
                            {
                                contents.Root = new Directory(item);
                                break;
                            }
                        case FileSystem.PARENT_PATH:
                            {
                                if (brickDirectoryPath == FileSystem.ROOT_PATH) contents.Parent = new Directory(FileSystem.ROOT_PATH);
                                else
                                {
                                    string parent = brickDirectoryPath.Substring(0, brickDirectoryPath.LastIndexOf(FileSystem.DIRECTORY_SEPERATOR));
                                    contents.Parent = new Directory(parent);
                                }
                                break;
                            }
                        default:
                            {
                                if (item.EndsWith(FileSystem.DIRECTORY_SEPERATOR))
                                {
                                    string directoryPath = $"{brickDirectoryPath}{item}";
                                    directories.Add(new Directory(directoryPath));
                                }
                                else
                                {
                                    string[] fileInfo = entry.Split(' ');
                                    if (fileInfo.Length >= 3)
                                    {
                                        string md5sum = fileInfo[0].Trim();
                                        long byteSize = Convert.ToInt64(fileInfo[1].Trim(), 16);
                                        string fileName = string.Join(" ", fileInfo, 2, fileInfo.Length - 2);
                                        if (!string.IsNullOrWhiteSpace(fileName)) files.Add(new File(brickDirectoryPath, fileName, md5sum, byteSize));
                                    }
                                }
                                break;
                            }
                    }

                }
                directories.Sort(delegate (Directory obj1, Directory obj2) { return obj1.Name.CompareTo(obj2.Name); });
                contents.Directories = directories.ToArray();
                files.Sort(delegate (File obj1, File obj2) { return obj1.FileName.CompareTo(obj2.FileName); });
                contents.Files = files.ToArray();
                return contents;
            }
            finally 
            {
                semaPhoreSlim.Release();
            }
        }

        #region list directories/files for given brickpath 
        private static async Task<Response> ContinueList(ISocket socket, byte handle, ushort payLoad)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.SYSTEM_COMMAND_REPLY))
            {
                cb.OpCode(SYSTEM_OP.CONTINUE_LIST_FILES);
                cb.Raw(handle);
                cb.Raw(payLoad);
                cmd = cb.ToCommand();
            }
            return await socket.Execute(cmd);
        }
        #endregion

        /// <summary>
        /// Create a directory on the brick
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <param name="brickDirectoryPath">relative path to folder on brick. e.g. "../[prjs|apps|tools]/directory*/"</param>
        /// <returns><c>true</c> if success, otherwise <c>false</c></returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        public static async Task<bool> CreateDirectory(ISocket socket, string brickDirectoryPath)
        {
            brickDirectoryPath = FileSystem.ToBrickDirectoryPath(brickDirectoryPath);
            brickDirectoryPath.IsBrickDirectoryPath();

            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.SYSTEM_COMMAND_REPLY))
            {
                cb.OpCode(SYSTEM_OP.CREATE_DIR);
                cb.Raw(brickDirectoryPath);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);
            return response.Status == ResponseStatus.SUCCESS;
        }

        /// <summary>
        /// Delete a file or folder on the brick
        /// </summary>
        /// <param name="socket">socket for executing command to brick</param>
        /// <param name="brickPath">relative path to file or folder on brick</param>
        /// <returns><c>true</c> if success, otherwise <c>false</c></returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        public static async Task<bool> Delete(ISocket socket, string brickPath)
        {
            brickPath.IsBrickPath();

            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.SYSTEM_COMMAND_REPLY))
            {
                cb.OpCode(SYSTEM_OP.DELETE_FILE);
                cb.Raw(brickPath);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);
            return response.Status == ResponseStatus.SUCCESS;
        }

        #region TODO
        /*
        #define BEGIN_GETFILE 0x96 // Begin get bytes from a file (while writing to the file)
        #define CONTINUE_GETFILE 0x97 // Continue get byte from a file (while writing to the file)
        #define CLOSE_FILEHANDLE 0x98 // Close file handle
        #define LIST_OPEN_HANDLES 0x9D // List handles
        #define WRITEMAILBOX 0x9E // Write to mailbox
        #define BLUETOOTHPIN 0x9F // Transfer trusted pin code to brick
        #define ENTERFWUPDATE 0xA0 // Restart the brick in Firmware update mode
        */
        #endregion
    }
}


