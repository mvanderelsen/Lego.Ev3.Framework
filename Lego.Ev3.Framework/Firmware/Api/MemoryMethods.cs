using Lego.Ev3.Framework.Core;
using System;
using System.Text;
using System.Threading.Tasks;
namespace Lego.Ev3.Framework.Firmware
{
    internal static class MemoryMethods
    {
        #region  OpFile

         //      *  - CMD = MAKE_FOLDER
         //*\n  Make folder if not present\n
         //*    -  \param  (DATA8)    NAME        - First character in folder name (character string)\n
         //*    -  \return (DATA8)    SUCCESS     - Success (0 = no, 1 = yes)\n
        internal static async Task<bool> MakeFolder(Socket socket, string path)
        {

            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, 1, 0))
            {
                cb.OpCode(OP.opFILE);
                cb.Raw((byte)FILE_SUBCODE.MAKE_FOLDER);
                cb.PARS(path);
                cb.GlobalIndex(0);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);


            bool value = false;

            if (response.Type == ResponseType.OK)
            {
                byte[] data = response.PayLoad;
                value = data[0] == 1;
            }
            return value;
        }


         //*  - CMD = GET_FOLDERS
         //*    -  \param  (DATA8)    NAME    - First character in folder name (ex "../prjs/")\n
         //*    -  \return (DATA8)    ITEMS   - No of sub folders\n
        internal static async Task<int> GetItemCount(Socket socket, string path)
        {

            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, 1, 0))
            {
                cb.OpCode(OP.opFILE);
                cb.Raw((byte)FILE_SUBCODE.GET_FOLDERS);
                cb.PARS(path);
                cb.GlobalIndex(0);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            int value = 0;

            if (response.Type == ResponseType.OK)
            {
                byte[] data = response.PayLoad;
                value = data[0];
            }
            return value;
        }

         //        *  - CMD = GET_SUBFOLDER_NAME
         //*    -  \param  (DATA8)    NAME    - First character in folder name (ex "../prjs/")\n
         //*    -  \param  (DATA8)    ITEM    - Sub folder index [1..ITEMS]\n
         //*    -  \param  (DATA8)    LENGTH  - Maximal string length\n
         //*    -  \return (DATA8)    STRING  - First character in character string\n
         //*
        internal static async Task<string> GetItemName(Socket socket, string path, int itemIndex, int stringLength = -1)
        {
            if (stringLength < 1 || stringLength > 255) stringLength = 255;
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, (ushort)stringLength, 0))
            {
                cb.OpCode(OP.opFILE);
                cb.Raw((byte)FILE_SUBCODE.GET_SUBFOLDER_NAME);
                cb.PARS(path);
                cb.PAR8(itemIndex);
                cb.PAR32(stringLength);
                cb.GlobalIndex(0);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);

            string value = "";

            if (response.Type == ResponseType.OK)
            {
                byte[] data = response.PayLoad;
                int index = Array.IndexOf(data, (byte)0);
                if(index > 0) value = Encoding.UTF8.GetString(data, 0, index);
            }
            return value;
        }


         // *  - CMD = REMOVE
         //*\n  Delete file (if name starts with '~','/' or '.' it is not from current folder) \n
         //*    -  \param  (DATA8)    NAME        - First character in file name (character string)\n
        internal static async Task Remove(Socket socket, string path)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_NO_REPLY))
            {
                cb.OpCode(OP.opFILE);
                cb.Raw((byte)FILE_SUBCODE.REMOVE);
                cb.PARS(path);
                cmd = cb.ToCommand();
            }
            await socket.Execute(cmd);
        }
        #endregion


        #region opFileName

         //      *  - CMD = EXIST
         //*\n  Test if file exists (if name starts with '~','/' or '.' it is not from current folder) \n
         //*    -  \param  (DATA8)    NAME        - First character in file name (character string)\n
         //*    -  \return (DATA8)    FLAG        - Exist (0 = no, 1 = yes)\n
        internal static async Task<bool> Exists(Socket socket, string path)
        {

            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, 1, 0))
            {
                cb.OpCode(OP.opFILENAME);
                cb.Raw((byte)FILENAME_SUBCODE.EXIST);
                cb.PARS(path);
                cb.GlobalIndex(0);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);


            bool value = false;

            if (response.Type == ResponseType.OK)
            {
                byte[] data = response.PayLoad;
                value = data[0] == 1;
            }
            return value;
        }

         //        *  - CMD = TOTALSIZE
         //*\n  Calculate folder/file size (if name starts with '~','/' or '.' it is not from current folder) \n
         //*    -  \param  (DATA8)    NAME        - First character in file name (character string)\n
         //*    -  \return (DATA32)   FILES       - Total number of files\n
         //*    -  \return (DATA32)   SIZE        - Total folder size [KB]\n
        internal static async Task<int[]> GetDirectoryInfo(Socket socket, string path)
        {
            Command cmd = null;
            using (CommandBuilder cb = new CommandBuilder(CommandType.DIRECT_COMMAND_REPLY, 8, 0))
            {
                cb.OpCode(OP.opFILENAME);
                cb.Raw((byte)FILENAME_SUBCODE.TOTALSIZE);
                cb.PARS(path);
                cb.GlobalIndex(0);
                cb.GlobalIndex(4);
                cmd = cb.ToCommand();
            }
            Response response = await socket.Execute(cmd);


            int[] value = new int[2];

            if (response.Type == ResponseType.OK)
            {
                byte[] data = response.PayLoad;
                value[0] = BitConverter.ToInt32(data, 0);
                value[1] = BitConverter.ToInt32(data, 4);
            }
            return value;
        }

        #endregion

        internal static async Task<int[]> MemoryUsage(Socket socket)
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


            int[] value = new int[2];

            if (response.Type == ResponseType.OK)
            {
                byte[] data = response.PayLoad;
                int index = 0;
                value[0] = BitConverter.ToInt32(data, index);
                index += DataType.DATA32.ByteLength();
                value[1] = BitConverter.ToInt32(data, index);
            }
            return value;
        }


















        //internal static async Task<string> GetCurrentFolderName(Socket socket, int stringLength = -1)
        //{

        //    if (stringLength < 1 || stringLength > 255) stringLength = 255;
        //    Command cmd = null;
        //    //(ushort)stringLength
        //    using (CommandBuilder cb = new CommandBuilder(DIRECT_COMMAND_TYPE.DIRECT_COMMAND_REPLY, (ushort)stringLength, 0))
        //    {
        //        cb.OpCode(OP.opFILENAME);
        //        cb.Raw((byte)FILENAME_SUBCODE.GET_FOLDERNAME);
        //        cb.PAR32(stringLength);
        //        cb.GlobalIndex(0);
        //        cmd = cb.ToCommand();
        //    }
        //    await socket.Execute(cmd);


        //    string value = "";

        //    //if (response.Type == ResponseType.OK)
        //    //{
        //        byte[] data = response.PayLoad;
        //        int index = Array.IndexOf(data, (byte)0);
        //        value = Encoding.UTF8.GetString(data, 0, index);
        //    //}
        //    return value;
        //}





        //******* BYTE CODE SNIPPETS **************************************************


        /*! \page cMemory Memory
         *  <hr size="1"/>
         *  \subpage MemoryLibraryCodes
         *  \n
         *  \n
         *  <b>     opFILE (CMD, ....)  </b>
         *
         *- Memory file entry\n
         *- Dispatch status unchanged
         *
         *  \param  (DATA8)   CMD               - \ref memoryfilesubcode
         *
         *\n
         *  - CMD = OPEN_APPEND
         *\n  Create file or open for append (if name starts with '~','/' or '.' it is not from current folder) \n
         *    -  \param  (DATA8)    NAME        - First character in file name (character string)\n
         *    -  \return (HANDLER)  HANDLE      - Handle to file\n
         *
         *\n
         *  - CMD = OPEN_READ
         *\n  Open file for read (if name starts with '~','/' or '.' it is not from current folder) \n
         *    -  \param  (DATA8)    NAME        - First character in file name (character string)\n
         *    -  \return (HANDLER)  HANDLE      - Handle to file\n
         *    -  \return (DATA32)   SIZE        - File size (0 = not found)\n
         *
         *\n
         *  - CMD = OPEN_WRITE
         *\n  Create file for write (if name starts with '~','/' or '.' it is not from current folder) \n
         *    -  \param  (DATA8)    NAME        - First character in file name (character string)\n
         *    -  \return (HANDLER)  HANDLE      - Handle to file\n
         *
         *\n
         *  - CMD = CLOSE
         *\n  Close file\n
         *    -  \param  (HANDLER)  HANDLE      - Handle to file\n
         *
         *\n

         *\n
         *  - CMD = MOVE
         *\n  Move file SOURCE to DEST (if name starts with '~','/' or '.' it is not from current folder) \n
         *    -  \param  (DATA8)    SOURCE      - First character in source file name (character string)\n
         *    -  \param  (DATA8)    DEST        - First character in destination file name (character string)\n
         *
         *\n
         *  - CMD = WRITE_TEXT
         *\n  Write text to file \n
         *    -  \param  (HANDLER)  HANDLE      - Handle to file\n
         *    -  \param  (DATA8)     \ref delimiters "DEL" - Delimiter code\n
         *    -  \param  (DATA8)    TEXT        - First character in text to write (character string)\n
         *
         *\n
         *  - CMD = READ_TEXT
         *\n  Read text from file \n
         *    -  \param  (HANDLER)  HANDLE      - Handle to file\n
         *    -  \param  (DATA8)     \ref delimiters "DEL" - Delimiter code\n
         *    -  \param  (DATA8)    LENGTH      - Maximal string length\n
         *    -  \return (DATA8)    TEXT        - First character in text to read (character string)\n
         *
         *\n
         *  - CMD = WRITE_VALUE
         *\n  Write floating point value to file \n
         *    -  \param  (HANDLER)  HANDLE      - Handle to file\n
         *    -  \param  (DATA8)     \ref delimiters "DEL" - Delimiter code\n
         *    -  \param  (DATAF)    VALUE       - Value to write\n
         *    -  \param  (DATA8)    FIGURES     - Total number of figures inclusive decimal point\n
         *    -  \param  (DATA8)    DECIMALS    - Number of decimals\n
         *
         *\n
         *  - CMD = READ_VALUE
         *\n  Read floating point value from file \n
         *    -  \param  (HANDLER)  HANDLE      - Handle to file\n
         *    -  \param  (DATA8)     \ref delimiters "DEL" - Delimiter code\n
         *    -  \return (DATAF)    VALUE       - Value to write\n
         *
         *\n
         *  - CMD = WRITE_BYTES
         *\n  Write a number of bytes to file \n
         *    -  \param  (HANDLER)  HANDLE      - Handle to file\n
         *    -  \param  (DATA16)   BYTES       - Number of bytes to write\n
         *    -  \param  (DATA8)    SOURCE      - First byte in byte stream to write\n
         *
         *\n
         *  - CMD = READ_BYTES
         *\n  Read a number of bytes from file \n
         *    -  \param  (HANDLER)  HANDLE      - Handle to file\n
         *    -  \param  (DATA16)   BYTES       - Number of bytes to write\n
         *    -  \return (DATA8)    DESTINATION - First byte in byte stream to write\n
         *
         *\n
         *  - CMD = OPEN_LOG
         *\n  Create file for data logging (if name starts with '~','/' or '.' it is not from current folder) (see \ref cinputsample "Example")\n
         *    -  \param  (DATA8)    NAME        - First character in file name (character string)\n
         *    -  \param  (DATA32)   syncedTime  -
         *    -  \param  (DATA32)   syncedTick  -
         *    -  \param  (DATA32)   nowTick     -
         *    -  \param  (DATA32)   sample_interval_in_ms -
         *    -  \param  (DATA32)   duration_in_ms -
         *    -  \param  (DATA8)    SDATA       - First character in sensor type data (character string)\n
         *    -  \return (HANDLER)  HANDLE      - Handle to file\n
         *
         *\n
         *  - CMD = WRITE_LOG
         *\n  Write time slot samples to file (see \ref cinputsample "Example")\n
         *    -  \param  (HANDLER)  HANDLE      - Handle to file\n
         *    -  \param  (DATA32)   TIME        - Relative time in mS\n
         *    -  \param  (DATA8)    ITEMS       - Total number of values in this time slot\n
         *    .  \param  (DATAF)    VALUES      - DATAF array (handle) containing values\n
         *
         *\n
         *  - CMD = CLOSE_LOG
         *\n  Close data log file (see \ref cinputsample "Example")\n
         *    -  \param  (HANDLER)  HANDLE      - Handle to file\n
         *    -  \param  (DATA8)    NAME        - First character in file name (character string)\n
         *
         *\n
         *  - CMD = GET_LOG_NAME
         *\n  Get the current open log filename\n
         *    -  \param  (DATA8)    LENGTH      - Max string length (don't care if NAME is a HND\n
         *    -  \param  (DATA8)    NAME        - First character in file name (character string or HND)\n
         *
         *\n
         *  - CMD = GET_HANDLE
         *\n  Get handle from filename (if name starts with '~','/' or '.' it is not from current folder) \n
         *    -  \param  (DATA8)    NAME        - First character in file name (character string)\n
         *    -  \return (HANDLER)  HANDLE      - Handle to file\n
         *    -  \return (DATA8)    WRITE       - Open for write / append (0 = no, 1 = yes)\n
         *
         *\n
  
         *
         *\n
         *  - CMD = LOAD_IMAGE
         *    -  \param  (DATA16)   PRGID   - Program id (see \ref prgid)\n
         *    -  \param  (DATA8)    NAME    - First character in name (character string)\n
         *    -  \return (DATA32)   SIZE    - Size\n
         *    -  \return (DATA32)   *IP     - Address of image\n
         *
         *\n
         *  - CMD = GET_POOL
         *    -  \param  (DATA32)   SIZE    - Size of pool\n
         *    -  \return (HANDLER)  HANDLE  - Handle to pool\n
         *    -  \return (DATA32)   *IP     - Address of image\n
         *

         *\n

         *\n
         *  - CMD = DEL_SUBFOLDER
         *    -  \param  (DATA8)    NAME    - First character in folder name (ex "../prjs/")\n
         *    -  \param  (DATA8)    ITEM    - Sub folder index [1..ITEMS]\n
         *
         *\n
         *  - CMD = SET_LOG_SYNC_TIME
         *    -  \param  (DATA32)   TIME    - Sync time used in data log files\n
         *    -  \param  (DATA32)   TICK    - Sync tick used in data log files\n
         *
         *\n
         *  - CMD = GET_LOG_SYNC_TIME
         *    -  \return (DATA32)   TIME    - Sync time used in data log files\n
         *    -  \return (DATA32)   TICK    - Sync tick used in data log files\n
         *
         *\n
         *  - CMD = GET_IMAGE
         *    -  \param  (DATA8)    NAME    - First character in folder name (ex "../prjs/")\n
         *    -  \param  (DATA16)   PRGID   - Program id (see \ref prgid)\n
         *    -  \param  (DATA8)    ITEM    - Sub folder index [1..ITEMS]\n
         *    -  \return (DATA32)   *IP     - Address of image\n
         *
         *\n
         *  - CMD = GET_ITEM
         *    -  \param  (DATA8)    NAME    - First character in folder name (ex "../prjs/")\n
         *    -  \param  (DATA8)    STRING  - First character in item name string\n
         *    -  \return (DATA8)    ITEM    - Sub folder index [1..ITEMS]\n
         *
         *\n
         *  - CMD = GET_CACHE_FILES
         *    -  \return (DATA8)    ITEMS   - No of files in cache\n
         *
         *\n
         *  - CMD = GET_CACHE_FILE
         *    -  \param  (DATA8)    ITEM    - Cache index [1..ITEMS]\n
         *    -  \param  (DATA8)    LENGTH  - Maximal string length\n
         *    -  \return (DATA8)    STRING  - First character in character string\n
         *
         *\n
         *  - CMD = PUT_CACHE_FILE
         *    -  \param  (DATA8)    STRING  - First character in character string\n
         *
         *\n
         *  - CMD = DEL_CACHE_FILE
         *    -  \param  (DATA8)    ITEM    - Cache index [1..ITEMS]\n
         *    -  \param  (DATA8)    LENGTH  - Maximal string length\n
         *    -  \return (DATA8)    STRING  - First character in character string\n
         *
         *\n
         *
         */
        /*! \brief  opFILE byte code
         *
         */




        /*! \page cMemory
         *  <hr size="1"/>
         *  <b>     opFILENAME (CMD, ....)  </b>
         *
         *- Memory filename entry\n
         *- Dispatch status unchanged
         *


         *
         *\n
         *  - CMD = SPLIT
         *\n  Split filename into Folder, name, extension \n
         *    -  \param  (DATA8)    FILENAME    - First character in file name (character string) "../folder/subfolder/name.ext"\n
         *    -  \param  (DATA8)    LENGTH      - Maximal length for each of the below parameters\n
         *    -  \return (DATA8)    FOLDER      - First character in folder name (character string) "../folder/subfolder"\n
         *    -  \return (DATA8)    NAME        - First character in name (character string) "name"\n
         *    -  \return (DATA8)    EXT         - First character in extension (character string) ".ext"\n
         *
         *\n
         *  - CMD = MERGE
         *\n  Merge Folder, name, extension into filename\n
         *    -  \param  (DATA8)    FOLDER      - First character in folder name (character string) "../folder/subfolder"\n
         *    -  \param  (DATA8)    NAME        - First character in name (character string) "name"\n
         *    -  \param  (DATA8)    EXT         - First character in extension (character string) ".ext"\n
         *    -  \param  (DATA8)    LENGTH      - Maximal length for the below parameter\n
         *    -  \return (DATA8)    FILENAME    - First character in file name (character string) "../folder/subfolder/name.ext"\n
         *
         *\n
         *  - CMD = CHECK
         *\n  Check filename\n
         *    -  \param  (DATA8)    FILENAME    - First character in file name (character string) "../folder/subfolder/name.ext"\n
         *    -  \return (DATA8)    OK          - Filename ok (0 = FAIL, 1 = OK)\n
         *
         *\n
         *  - CMD = PACK
         *\n  Pack file or folder into "raf" container\n
         *    -  \param  (DATA8)    FILENAME    - First character in file name (character string) "../folder/subfolder/name.ext"\n
         *
         *\n
         *  - CMD = UNPACK
         *\n  Unpack "raf" container\n
         *    -  \param  (DATA8)    FILENAME    - First character in file name (character string) "../folder/subfolder/name"\n
         *
         *\n
         *  - CMD = GET_FOLDERNAME
         *\n  Get current folder name\n
         *    -  \param  (DATA8)    LENGTH      - Maximal length for the below parameter\n
         *    -  \return (DATA8)    FOLDERNAME  - First character in folder name (character string) "../folder/subfolder"\n
         *
         *\n
         *
         */
        /*! \brief  opFILENAME byte code
         *
         */


        /*! \page cMemory
         *  <hr size="1"/>
         *  <b>     opARRAY (CMD, ....)  </b>
         *
         *- Array entry\n
         *- Dispatch status can change to BUSYBREAK or FAILBREAK
         *
         *  \param  (DATA8)   CMD     - \ref memoryarraysubcode
         *
         *\n
         *  - CMD = CREATE8
         *    -   \param  (DATA32)  ELEMENTS  - Number of elements\n
         *    -   \return (HANDLER) HANDLE    - Array handle\n
         *
         *\n
         *  - CMD = CREATE16
         *    -   \param  (DATA32)  ELEMENTS  - Number of elements\n
         *    -   \return (HANDLER) HANDLE    - Array handle\n
         *
         *\n
         *  - CMD = CREATE32
         *    -   \param  (DATA32)  ELEMENTS  - Number of elements\n
         *    -   \return (HANDLER) HANDLE    - Array handle\n
         *
         *\n
         *  - CMD = CREATEF
         *    -   \param  (DATA32)  ELEMENTS  - Number of elements\n
         *    -   \return (HANDLER) HANDLE    - Array handle\n
         *
         *\n
         *  - CMD = SIZE
         *    -   \param  (HANDLER) HANDLE    - Array handle\n
         *    -   \return (DATA32)  ELEMENTS  - Total number of elements in array\n
         *
         *\n
         *  - CMD = RESIZE
         *    -   \param  (HANDLER) HANDLE    - Array handle\n
         *    -   \param  (DATA32)  ELEMENTS  - Total number of elements\n
         *
         *\n
         *  - CMD = DELETE
         *    -   \param  (HANDLER) HANDLE    - Array handle\n
         *
         *\n
         *  - CMD = FILL
         *    -   \param  (HANDLER) HANDLE    - Array handle\n
         *    -   \param  (type)    VALUE     - Value to write - type depends on type of array\n
         *
         *\n
         *  - CMD = COPY
         *    -   \param  (HANDLER) HSOURCE   - Source array handle\n
         *    -   \param  (HANDLER) HDEST     - Destination array handle\n
         *
         *\n
         *  - CMD = INIT8
         *    -   \param  (HANDLER) HANDLE    - Array handle\n
         *    -   \param  (DATA32)  INDEX     - Index to element to write
         *    -   \param  (DATA32)  ELEMENTS  - Number of elements to write\n
         *
         *    Below a number of VALUES equal to "ELEMENTS"
         *    -   \param  (DATA8)   VALUE     - First value to write - type must be equal to the array type\n
         *
         *\n
         *  - CMD = INIT16
         *    -   \param  (HANDLER) HANDLE    - Array handle\n
         *    -   \param  (DATA32)  INDEX     - Index to element to write
         *    -   \param  (DATA32)  ELEMENTS  - Number of elements to write\n
         *
         *    Below a number of VALUES equal to "ELEMENTS"
         *    -   \param  (DATA16)  VALUE     - First value to write - type must be equal to the array type\n
         *
         *\n
         *  - CMD = INIT32
         *    -   \param  (HANDLER) HANDLE    - Array handle\n
         *    -   \param  (DATA32)  INDEX     - Index to element to write
         *    -   \param  (DATA32)  ELEMENTS  - Number of elements to write\n
         *
         *    Below a number of VALUES equal to "ELEMENTS"
         *    -   \param  (DATA32)  VALUE     - First value to write - type must be equal to the array type\n
         *
         *\n
         *  - CMD = INITF
         *    -   \param  (HANDLER) HANDLE    - Array handle\n
         *    -   \param  (DATA32)  INDEX     - Index to element to write
         *    -   \param  (DATA32)  ELEMENTS  - Number of elements to write\n
         *
         *    Below a number of VALUES equal to "ELEMENTS"
         *    -   \param  (DATAF)   VALUE     - First value to write - type must be equal to the array type\n
         *
         *\n
         *  - CMD = READ_CONTENT
         *    -   \param  (DATA16)  PRGID     - Program slot number (must be running) (see \ref prgid)\n
         *    -   \param  (HANDLER) HANDLE    - Array handle\n
         *    -   \param  (DATA32)  INDEX     - Index to first byte to read\n
         *    -   \param  (DATA32)  BYTES     - Number of bytes to read\n
         *    -   \return (DATA8)   ARRAY     - First byte of array to receive data\n
         *
         *\n
         *  - CMD = WRITE_CONTENT
         *    -   \param  (DATA16)  PRGID     - Program slot number (must be running) (see \ref prgid)\n
         *    -   \param  (HANDLER) HANDLE    - Array handle\n
         *    -   \param  (DATA32)  INDEX     - Index to first byte to write\n
         *    -   \param  (DATA32)  BYTES     - Number of bytes to write\n
         *    -   \param  (DATA8)   ARRAY     - First byte of array to deliver data\n
         *
         *\n
         *  - CMD = READ_SIZE
         *    -   \param  (DATA16)  PRGID     - Program slot number (must be running) (see \ref prgid)\n
         *    -   \param  (HANDLER) HANDLE    - Array handle\n
         *    -   \return (DATA32)  BYTES     - Number of bytes in array\n
         *
         *\n
         *
         */


        /*! \brief  opARRAY byte code
         *
         */

        /*! \page cMemory
         *
         *  <hr size="1"/>
         *  <b>     opARRAY_WRITE (HANDLE, INDEX, VALUE)  </b>
         *
         *- Array element write\n
         *- Dispatch status can change to FAILBREAK
         *
         *  \param  (HANDLER) HANDLE    - Array handle
         *  \param  (DATA32)  INDEX     - Index to element to write
         *  \param  (type)    VALUE     - Value to write - type depends on type of array\n
         *
         *\n
         *
         */
        /*! \brief  opARRAY_WRITE byte code
         *
         */


        /*! \page cMemory
         *
         *  <hr size="1"/>
         *  <b>     opARRAY_READ (HANDLE, INDEX, VALUE)  </b>
         *
         *- Array element write\n
         *- Dispatch status can change to FAILBREAK
         *
         *  \param  (HANDLER) HANDLE    - Array handle
         *  \param  (DATA32)  INDEX     - Index of element to read
         *  \return (type)    VALUE     - Value to read - type depends on type of array
         *
         *\n
         *
         */
        /*! \brief  opARRAY_READ byte code
         *
         */


        /*! \page cMemory
         *
         *  <hr size="1"/>
         *  <b>     opARRAY_APPEND (HANDLE, VALUE)  </b>
         *
         *- Array element append\n
         *- Dispatch status can change to FAILBREAK
         *
         *  \param  (HANDLER) HANDLE    - Array handle
         *  \param  (type)    VALUE     - Value (new element) to append - type depends on type of array\n
         *
         *\n
         *
         */
        /*! \brief  opARRAY_APPEND byte code
         *
         */




    }
}
