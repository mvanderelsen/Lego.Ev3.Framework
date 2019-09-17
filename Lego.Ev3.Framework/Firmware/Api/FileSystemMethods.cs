using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Text;

namespace Lego.Ev3.Framework.Firmware
{

    /*

     * TODO!!!

        #define BEGIN_UPLOAD 0x94 // Begin file upload
        #define CONTINUE_UPLOAD 0x95 // Continue file upload
     * 
     * 
        #define BEGIN_GETFILE 0x96 // Begin get bytes from a file (while writing to the file)
        #define CONTINUE_GETFILE 0x97 // Continue get byte from a file (while writing to the file)
        
        #define LIST_OPEN_HANDLES 0x9D // List handles
        #define WRITEMAILBOX 0x9E // Write to mailbox
        #define BLUETOOTHPIN 0x9F // Transfer trusted pin code to brick
        #define ENTERFWUPDATE 0xA0 // Restart the brick in Firmware update mode
     */


    /// <summary>
    /// Methods to interact with the bricks file system.
    /// </summary>
    /// <remarks>
    /// #define SYSTEM_COMMAND_REPLY 0x01 // System command, reply required
    /// #define SYSTEM_COMMAND_NO_REPLY 0x81 // System command, reply not require
    /// Byte 0 – 1: Command size, Little Endian. Command size not including these 2 bytes
    /// Byte 2 – 3: Message counter, Little Endian. Forth running counter
    /// Byte 4: Command type. See defines above:
    /// Byte 5: System Command. See the definitions below:
    /// Byte 6 – n: Depends on the System Command given in byte 5.
    /// 
    /// See: LEGO® MINDSTORMS® EV3 Firmware Developer Kit page 102 for FileSystem Folder Structure
    /// </remarks>
    internal static class FileSystemMethods
    {

        internal static bool IsRobotFile(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLower();
            if (string.IsNullOrWhiteSpace(ext)) return false;
            return (
                ext == FILE_TYPE.vmEXT_ARCHIVE
                || ext == FILE_TYPE.vmEXT_BYTECODE
                || ext == FILE_TYPE.vmEXT_CONFIG
                || ext == FILE_TYPE.vmEXT_DATALOG
                || ext == FILE_TYPE.vmEXT_GRAPHICS
                || ext == FILE_TYPE.vmEXT_PROGRAM
                || ext == FILE_TYPE.vmEXT_SOUND
                || ext == FILE_TYPE.vmEXT_TEXT
                );
        }

        internal static FileType GetFileType(string path)
        {
            string ext = Path.GetExtension(path).ToLower();
            if (ext == FILE_TYPE.vmEXT_ARCHIVE) return FileType.ArchiveFile;
            if (ext == FILE_TYPE.vmEXT_BYTECODE) return FileType.ByteCodeFile;
            if (ext == FILE_TYPE.vmEXT_CONFIG) return FileType.ConfigFile;
            if (ext == FILE_TYPE.vmEXT_DATALOG) return FileType.DataLogFile;
            if (ext == FILE_TYPE.vmEXT_GRAPHICS) return FileType.GraphicFile;
            if (ext == FILE_TYPE.vmEXT_PROGRAM) return FileType.ProgramFile;
            if (ext == FILE_TYPE.vmEXT_SOUND) return FileType.SoundFile;
            if (ext == FILE_TYPE.vmEXT_TEXT) return FileType.TextFile;
            return FileType.SystemFile;
        }

        internal static string GetExtension(FileType type)
        {
            switch(type)
            {
                case FileType.ArchiveFile:
                    {
                        return FILE_TYPE.vmEXT_ARCHIVE;
                    }
                case FileType.ByteCodeFile:
                    {
                        return FILE_TYPE.vmEXT_BYTECODE;
                    }
                case FileType.ConfigFile:
                    {
                        return FILE_TYPE.vmEXT_CONFIG;
                    }
                case FileType.DataLogFile:
                    {
                        return FILE_TYPE.vmEXT_DATALOG;
                    }
                case FileType.GraphicFile:
                    {
                        return FILE_TYPE.vmEXT_GRAPHICS;
                    }
                case FileType.ProgramFile:
                    {
                        return FILE_TYPE.vmEXT_PROGRAM;
                    }
                case FileType.SoundFile:
                    {
                        return FILE_TYPE.vmEXT_SOUND;
                    }
                case FileType.TextFile:
                    {
                        return FILE_TYPE.vmEXT_TEXT;
                    }
            }

            return null;
        }

        /// <summary>
        /// Default buffer payload size
        /// </summary>
        private const int PAYLOAD_SIZE = 960;


        internal class BeginUploadResponse
        {
            public int FileSize { get; set; }

            public byte Handle { get; set; }

            public byte[] Data { get; set; }
        }

        /// <summary>
        /// Function throws InvalidOperationException
        /// </summary>
        /// <param name="method">Name of the calling method</param>
        /// <param name="parameters">parameters used in calling method</param>
        /// <param name="status">Status of the command</param>
        private static void ThrowException(string method, string parameters, SYSTEM_COMMAND_STATUS status)
        {
            string message = string.Format("FileSystem method: {0}, parameters: {1}, status: {2}", method, parameters, status);
            //TODO add logging here

            throw new InvalidOperationException(message);
        }



        public static async Task<byte[]> DownLoadFileFromBrick(Socket socket, string brickFilePath)
        {
            BeginUploadResponse response = await BeginUpLoad(socket, brickFilePath);
            if (response == null) return null;

            int fileSize = response.FileSize;
            byte handle = response.Handle;

            List<byte> data = new List<byte>();

            int bytesLoaded = 0;
            while(bytesLoaded < fileSize)
            {
                int payLoadSize = Math.Min(PAYLOAD_SIZE, fileSize - bytesLoaded);
                byte[] payLoad = await ContinueUpLoad(socket, handle, (ushort)payLoadSize);
                if (payLoad != null)
                {
                    data.AddRange(payLoad);
                    bytesLoaded += payLoadSize;
                }
                
            }
            return data.ToArray();
        }

        /// <summary>
        /// This function begins downloading file from the brick.
        /// </summary>
        /// <param name="socket">Socket for executing command to brick</param>
        /// <param name="brickFilePath">download file to path on brick</param>
        /// <returns>handle (byte) to file,. If 0 no valid handle is returned.</returns>
        /// <remarks>
        /// #define BEGIN_UPLOAD
        /// </remarks>
        private static async Task<BeginUploadResponse> BeginUpLoad(Socket socket, string brickFilePath)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.SYSTEM_COMMAND_REPLY))
            {
                cb.OpCode(SYSTEM_OP.BEGIN_UPLOAD);
                cb.Raw((ushort)0); // set to 0 to just have handle and filesize returned
                cb.Raw(brickFilePath);
                cmd = cb.ToCommand();
            }

            Response response = await socket.Execute(cmd );

            BeginUploadResponse ret = null;
            if (response.Type == ResponseType.OK)
            {
                if (response.Status != SYSTEM_COMMAND_STATUS.SUCCESS) ThrowException("BeginUpload", brickFilePath, response.Status);

                ret = new BeginUploadResponse
                {
                    FileSize = BitConverter.ToInt32(response.PayLoad, 0),
                    Handle = response.PayLoad[4]
                };
            }
            return ret;
        }

        /// <summary>
        /// Continues file download from the brick
        /// </summary>
        /// <param name="socket">Socket for executing command to brick</param>
        /// <param name="handle">Handle to the file</param>
        /// <param name="payLoad">chunk of bytes of file to download</param>
        /// <returns>Status of the reply</returns>
        /// <remarks>
        ///CONTINUE_UPLOAD
        /// </remarks>
        private static async Task<byte[]> ContinueUpLoad(Socket socket, byte handle, ushort payLoad)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.SYSTEM_COMMAND_REPLY))
            {
                cb.OpCode(SYSTEM_OP.CONTINUE_UPLOAD);
                cb.Raw(handle);
                cb.Raw(payLoad);
                cmd = cb.ToCommand();
            }

            Response response  = await socket.Execute(cmd);
            if (response.Type == ResponseType.OK)
            {
                if (!(response.Status == SYSTEM_COMMAND_STATUS.SUCCESS || response.Status == SYSTEM_COMMAND_STATUS.END_OF_FILE)) ThrowException("ContinueUpLoad", handle + " " + payLoad, response.Status);

                byte[] data = new byte[payLoad];
                Array.Copy(response.PayLoad, 1, data, 0, payLoad);
                return data;
            }
            return null;
        }

        /// <summary>
        /// This function uploads a file on to the brick.
        /// </summary>
        /// <param name="socket">Socket for executing command to brick</param>
        /// <param name="stream">stream to source file to upload. Extension *.rgf|*.rbf|*.rsf|*.rdf</param>
        /// <param name="brickFilePath">relative filepath on brick ../[prjs|apps|tools]/directory*/filename.extension</param>
        /// <returns>SYSTEM_COMMAND_STATUS</returns>
        internal static async Task<SYSTEM_COMMAND_STATUS> UploadFileToBrick(Socket socket, Stream stream, string brickFilePath)
        {
            byte[] file = null;
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                file = ms.ToArray();
            }
            return await UploadFileToBrick(socket, file, brickFilePath);
        }

        /// <summary>
        /// This function uploads a file on to the brick.
        /// </summary>
        /// <param name="socket">Socket for executing command to brick</param>
        /// <param name="filePath">path to source file to upload. Extension *.rgf|*.rbf|*.rsf|*.rdf</param>
        /// <param name="brickFilePath">relative filepath on brick ../[prjs|apps|tools]/directory*/filename.extension</param>
        /// <returns>SYSTEM_COMMAND_STATUS</returns>
        internal static async Task<SYSTEM_COMMAND_STATUS> UploadFileToBrick(Socket socket, string filePath, string brickFilePath)
        {
            //Check filePath argument
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentException("File path can not be empty", filePath);
            if (!System.IO.File.Exists(filePath)) throw new ArgumentException("File does not exist", filePath);
            if (!IsRobotFile(filePath)) throw new ArgumentException("File path has no a proper lego format extension (*.rgf|*.rbf|*.rsf|*.rdf|*.rtf|*.rpf|*.rcf|*.raf)", filePath);

            byte[] file = await Task.FromResult(System.IO.File.ReadAllBytes(filePath));
            return await UploadFileToBrick(socket, file, brickFilePath);
        }

        /// <summary>
        /// This function uploads a file on to the brick.
        /// </summary>
        /// <param name="socket">Socket for executing command to brick</param>
        /// <param name="file">bytedata of source file to upload. Extension *.rgf|*.rbf|*.rsf|*.rdf</param>
        /// <param name="brickFilePath">relative filepath on brick ../[prjs|apps|tools]/directory*/filename.extension</param>
        /// <returns>SYSTEM_COMMAND_STATUS</returns>
        internal static async Task<SYSTEM_COMMAND_STATUS> UploadFileToBrick(Socket socket, byte[] file, string brickFilePath)
        {

            //Check brickFilePath argument
            if (file == null || file.Length == 0) throw new ArgumentException("File can not be empty", "file");
            if (string.IsNullOrEmpty(brickFilePath)) throw new ArgumentException("Brick path can not be empty", brickFilePath);
            if (!IsRobotFile(brickFilePath)) throw new ArgumentException("File path has no a proper lego format extension (*.rgf|*.rbf|*.rsf|*.rdf|*.rtf|*.rpf|*.rcf|*.raf)", brickFilePath);

            int fileSize = file.Length;

            Response response = await BeginDownLoad(socket, fileSize, brickFilePath);
            if (response.Status == SYSTEM_COMMAND_STATUS.FILE_EXISTS) return response.Status; // do not override existing files
            if (response.Type == ResponseType.ERROR && response.Status == SYSTEM_COMMAND_STATUS.SUCCESS) return SYSTEM_COMMAND_STATUS.UNKNOWN_ERROR;
            if (response.Type == ResponseType.ERROR && response.Status != SYSTEM_COMMAND_STATUS.SUCCESS) return response.Status;

            byte handle = response.PayLoad[0];

            int bytesSent = 0;
            while (bytesSent < fileSize)
            {
                int payLoadSize = Math.Min(PAYLOAD_SIZE, fileSize - bytesSent);
                byte[] payLoad = new byte[payLoadSize];
                Array.Copy(file, bytesSent, payLoad, 0, payLoadSize);
                SYSTEM_COMMAND_STATUS status = await ContinueDownLoad(socket, handle, payLoad);
                if (!(status == SYSTEM_COMMAND_STATUS.SUCCESS || status == SYSTEM_COMMAND_STATUS.END_OF_FILE)) return status;
                bytesSent += payLoadSize;
            }
            return SYSTEM_COMMAND_STATUS.SUCCESS;
        }



        /// <summary>
        /// This function begins downloading file on to the brick.
        /// </summary>
        /// <param name="socket">Socket for executing command to brick</param>
        /// <param name="fileSize">file data total bytes size to download on brick</param>
        /// <param name="brickFilePath">download file to path on brick</param>
        /// <returns>handle (byte) to file,. If 0 no valid handle is returned.</returns>
        /// <remarks>
        /// #define BEGIN_DOWNLOAD 0x92
        /// </remarks>
        private static async Task<Response> BeginDownLoad(Socket socket, int fileSize, string brickFilePath)
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


            //Response response = cmd.Reply;
            //byte handle = 0;
            //if (response.Type == ResponseType.OK)
            //{
            //    if (response.Status != SYSTEM_COMMAND_STATUS.SUCCESS) ThrowException("BeginDownLoad", fileSize + " " + brickFilePath, response.Status);
            //    handle = response.PayLoad[0];
            //}
            //return handle;
        }

        /// <summary>
        /// Continues file download on to the brick
        /// </summary>
        /// <param name="socket">Socket for executing command to brick</param>
        /// <param name="handle">Handle to the file</param>
        /// <param name="payLoad">chunk of bytes of file to download</param>
        /// <returns>Status of the reply</returns>
        /// <remarks>
        /// #define CONTINUE_DOWNLOAD 0x93
        /// </remarks>
        private static async Task<SYSTEM_COMMAND_STATUS> ContinueDownLoad(Socket socket, byte handle, byte[] payLoad)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.SYSTEM_COMMAND_REPLY))
            {
                cb.OpCode(SYSTEM_OP.CONTINUE_DOWNLOAD);
                cb.Raw(handle);
                cb.Raw(payLoad);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);
            SYSTEM_COMMAND_STATUS status = SYSTEM_COMMAND_STATUS.UNKNOWN_ERROR;
            if (response.Type == ResponseType.OK)
            {
                if (!(response.Status == SYSTEM_COMMAND_STATUS.SUCCESS || response.Status == SYSTEM_COMMAND_STATUS.END_OF_FILE)) ThrowException("ContinueDownLoad", handle + " " + payLoad, response.Status);
                status = response.Status;
            }
            return status;
        }

        /// <summary>
        /// This function gets  new line delimited list of folders and/or files
        /// </summary>
        /// <param name="socket">Socket for executing command to brick</param>
        /// <param name="brickPath">relative path to folder on brick. Path should always start with "../"  and end with "/"
        /// eg. Folder: "../sys/ui/". Warning illegal paths may lead to system hang up. "../" will list root directory.
        /// </param>
        /// <returns>
        /// The new line delimited list is formatted as:
        /// If it is a file:
        /// 32 chars (hex) of MD5SUM + space + 8 chars (hex) of filesize + space + filename + new line
        /// If it is a folder:
        /// foldername + / + new line
        /// </returns>
        /// <remarks>
        /// #define LIST_FILES 0x99
        /// </remarks>
        internal static async Task<string[]> ListFiles(Socket socket, string brickPath)
        {
            if (string.IsNullOrEmpty(brickPath)) throw new ArgumentException("Brick path can not be empty", brickPath);
            if (!brickPath.StartsWith("../")) throw new ArgumentException("Brick path must start with ../", brickPath);
            if (!brickPath.EndsWith("/")) throw new ArgumentException("Brick path must end with /", brickPath);

            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.SYSTEM_COMMAND_REPLY))
            {
                cb.OpCode(SYSTEM_OP.LIST_FILES);
                cb.Raw((ushort)PAYLOAD_SIZE);
                cb.Raw(brickPath);
                cmd = cb.ToCommand();
            }


            Response response = await socket.Execute(cmd);
            string[] list = new string[0];
            if (response.Type == ResponseType.OK)
            {

                if (!(response.Status == SYSTEM_COMMAND_STATUS.SUCCESS || response.Status == SYSTEM_COMMAND_STATUS.END_OF_FILE)) ThrowException("ListFiles", brickPath, response.Status);

                byte[] data = response.PayLoad;
                ushort listSize = (ushort)BitConverter.ToUInt32(data, 0);
                byte handle = data[4];

                byte[] fullList = new byte[data.Length - 5];
                Array.Copy(data, 5, fullList, 0, fullList.Length);

                if (response.Status == SYSTEM_COMMAND_STATUS.SUCCESS)
                {
                    using (PayLoadBuilder payLoadBuilder = new PayLoadBuilder())
                    {
                        int bytesRead = fullList.Length;
                        if (bytesRead > 0) payLoadBuilder.Raw(fullList); // append the already read bytes first
                        while (bytesRead < listSize)
                        {
                            int payLoadSize = Math.Min(PAYLOAD_SIZE, listSize - bytesRead);
                            byte[] chunk = await ContinueListFiles(socket, handle, (ushort)payLoadSize);
                            if (chunk != null) payLoadBuilder.Raw(chunk);
                            bytesRead += payLoadSize;
                        }

                        //reset fullList to all bytes in builder.
                        fullList = payLoadBuilder.ToBytes();

                    }
                }

                string value = Encoding.UTF8.GetString(fullList, 0, fullList.Length);
                list = value.Split('\n');
            }
            return list;
        }

        /// <summary>
        /// Continues reading file list
        /// </summary>
        /// <param name="socket">Socket for executing command to brick</param>
        /// <param name="handle">Handle to the file</param>
        /// <param name="payLoad">chunk of bytes of file to read</param>
        /// <returns>chunk</returns>
        /// <remarks>
        /// #define CONTINUE_LIST_FILES 0x9A
        /// </remarks>
        private static async Task<byte[]> ContinueListFiles(Socket socket, byte handle, ushort payLoad)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.SYSTEM_COMMAND_REPLY))
            {
                cb.OpCode(SYSTEM_OP.CONTINUE_LIST_FILES);
                cb.Raw(handle);
                cb.Raw(payLoad);
                cmd = cb.ToCommand();
            }

            Response response = await socket.Execute(cmd);
            byte[] chunk = null;
            if (response.Type == ResponseType.OK)
            {
                if (!(response.Status == SYSTEM_COMMAND_STATUS.SUCCESS || response.Status == SYSTEM_COMMAND_STATUS.END_OF_FILE)) ThrowException("ContinueListFiles", handle + " " + payLoad, response.Status);

                byte[] data = response.PayLoad;

                chunk = new byte[data.Length - 1];
                Array.Copy(data, 1, chunk, 0, chunk.Length);
            }
            return chunk;
        }

        /// <summary>
        /// This function creates a directory on the brick
        /// </summary>
        /// <param name="socket">Socket for executing command to brick</param>
        /// <param name="brickPath">relative path to folder on brick. "../[prjs|apps|tools]/directory*/"</param>
        /// <returns>directory created</returns>
        /// <remarks>
        /// #define CREATE_DIR 0x9B
        /// </remarks>
        internal static async Task<SYSTEM_COMMAND_STATUS> CreateDirectory(Socket socket, string brickPath)
        {
            //Check brickFilePath argument
            if (string.IsNullOrEmpty(brickPath)) throw new ArgumentException("Brick path can not be empty", brickPath);
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.SYSTEM_COMMAND_REPLY))
            {
                cb.OpCode(SYSTEM_OP.CREATE_DIR);
                cb.Raw(brickPath);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            if (response.Type == ResponseType.ERROR && response.Status == SYSTEM_COMMAND_STATUS.SUCCESS) return SYSTEM_COMMAND_STATUS.UNKNOWN_ERROR;
            return response.Status;
        }

        /// <summary>
        /// This function deletes a file or folder on the brick
        /// </summary>
        /// <param name="socket">Socket for executing command to brick</param>
        /// <param name="brickPath">relative path to folder on brick. "../[prjs|apps|tools]/directory/" or "../[prjs|apps|tools]/directory*/fileName.ext"</param>
        /// <returns>file deleted</returns>
        /// <remarks>
        /// #define DELETE_FILE 0x9C 
        /// </remarks>
        internal static async Task<SYSTEM_COMMAND_STATUS> DeleteFile(Socket socket, string brickPath)
        {
            //Check brickFilePath argument
            if (string.IsNullOrEmpty(brickPath)) throw new ArgumentException("Brick path can not be empty", brickPath);

            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.SYSTEM_COMMAND_REPLY))
            {
                cb.OpCode(SYSTEM_OP.DELETE_FILE);
                cb.Raw(brickPath);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            if (response.Type == ResponseType.ERROR && response.Status == SYSTEM_COMMAND_STATUS.SUCCESS) return SYSTEM_COMMAND_STATUS.UNKNOWN_ERROR;
            return response.Status;
        }






        //TODO!!

        ///// <summary>
        ///// This function closes a file handle (Upload)
        ///// </summary>
        ///// <param name="socket">Socket for executing command to brick</param>
        ///// <param name="handle">handle to file being uploaded</param>
        ///// <remarks>
        ///// #define CLOSE_FILEHANDLE 0x98
        ///// </remarks>
        ///// <returns></returns>
        //private static async Task CloseHandle(Socket socket, byte handle)
        //{
        //    Command cmd = null;
        //    using (CommandBuilder cb = new CommandBuilder(SYSTEM_COMMAND_TYPE.SYSTEM_COMMAND_REPLY))
        //    {
        //        cb.OpCode(SYSTEM_OP.CLOSE_FILEHANDLE);
        //        cb.Raw(handle);
        //        //add hash ??
        //        cmd = cb.ToCommand();
        //    }
        //    await socket.Execute(cmd);

        //    //SYSTEM_COMMAND_STATUS status = cmd.Reply.Status;
        //}




    }
}
