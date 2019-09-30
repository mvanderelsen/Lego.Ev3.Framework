using Lego.Ev3.Framework.Firmware;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Lego.Ev3.Framework.Core
{
    /// <summary>
    /// Static class to control all admin filesystem methods. Use with care and only when brick is connected!! This will allow deletion of firmware files 
    /// </summary>
    /// <example>
    /// <code>
    /// see bytecodes.h in lms2012
    /// FOLDERS
    /// 
    /// #define   vmMEMORY_FOLDER               "/mnt/ramdisk"                Folder for non volatile user programs/data
    /// #define   vmPROGRAM_FOLDER              "../prjs/BrkProg_SAVE"        Folder for On Brick Programming programs
    /// #define   vmDATALOG_FOLDER              "../prjs/BrkDL_SAVE"          Folder for On Brick Data log files
    /// #define   vmSDCARD_FOLDER               "../prjs/SD_Card"             Folder for SD card mount
    /// #define   vmUSBSTICK_FOLDER             "../prjs/USB_Stick"           Folder for USB stick mount
    /// 
    /// #define   vmPRJS_DIR                    "../prjs"                     Project folder
    /// #define   vmAPPS_DIR                    "../apps"                     Apps folder
    /// #define   vmTOOLS_DIR                   "../tools"                    Tools folder
    /// #define   vmTMP_DIR                     "../tmp"                      Temporary folder
    /// 
    /// #define   vmSETTINGS_DIR                "../sys/settings"             Folder for non volatile settings
    /// 
    /// #define   vmDIR_DEEPT                   127                           Max directory items allocated including "." and ".." 
    /// </code>
    /// </example>
    /// <example>
    /// <code>
    /// PROJECTS
    /// 
    ///     lms2012-------prjs-----,----xxxxxxxx------icon file       (icon.rgf)
    ///                   |        |                  byte code file  (xxxxxxxx.rbf)
    ///                   |        |                  sound files     (.rsf)
    ///                   |        |                  graphics files  (.rgf)
    ///                   |        |                  datalog files   (.rdf)
    ///                   |        |
    ///                   |        |----yyyyyyyy------icon file       (icon.rgf)
    ///                   |        |                  byte code file  (yyyyyyyy.rbf)
    ///                   |        |                  sound files
    ///                   |        |                  graphics files  (.rgf)
    ///                   |        |                  datalog files   (.rdf)
    ///                   |        |
    ///                   |        |--BrkProg_SAVE----byte code files (.rbf)
    ///                   |        |
    ///                   |        |---BrkDL_SAVE-----datalog files   (.rdf)
    ///                   |        |
    ///                   |        |
    ///                   |        '---SD_Card---,----vvvvvvvv------icon file       (icon.rgf)
    ///                   |                      |                  byte code file  (xxxxxxxx.rbf)
    ///                   |                      |                  sound files     (.rsf)
    ///                   |                      |                  graphics files  (.rgf)
    ///                   |                      |                  datalog files   (.rdf)
    ///                   |                      |
    ///                   |                      |----wwwwwwww------icon file       (icon.rgf)
    ///                   |                      |                  byte code file  (yyyyyyyy.rbf)
    ///                   |                      |                  sound files
    ///                   |                      |                  graphics files  (.rgf)
    ///                   |                      |                  datalog files   (.rdf)
    ///                   |                      |
    ///                   |                      |--BrkProg_SAVE----byte code files (.rbf)
    ///                   |                      |
    ///                   |                      '---BrkDL_SAVE-----datalog files   (.rdf)
    ///                   |
    ///                   |
    ///                   |         APPS
    ///                   |
    ///                   apps-----,----aaaaaaaa------icon file       (icon.rgf)
    ///                   |        |                  byte code files (aaaaaaaa.rbf)
    ///                   |        |                  sound files     (.rsf)
    ///                   |        |                  graphics files  (.rgf)
    ///                   |        |
    ///                   |        '----bbbbbbbb------icon file       (icon.rgf)
    ///                   |                           byte code files (bbbbbbbb.rbf)
    ///                   |                           sound files     (.rsf)
    ///                   |                           graphics files  (.rgf)
    ///                   |
    ///                   |         TOOLS
    ///                   |
    ///                   tools----,----cccccccc------icon file       (icon.rgf)
    ///                   |        |                  byte code files (cccccccc.rbf)
    ///                   |        |                  sound files     (.rsf)
    ///                   |        |                  graphics files  (.rgf)
    ///                   |        |
    ///                   |        '----dddddddd------icon file       (icon.rgf)
    ///                   |                           byte code files (dddddddd.rbf)
    ///                   |                           sound files     (.rsf)
    ///                   |                           graphics files  (.rgf)
    ///                   |
    ///                   |         SYSTEM
    ///                   |
    ///                   sys------,----ui------------byte code file  (gui.rbf)
    ///                            |                  sound files     (.rsf)
    ///                            |                  graphics files  (.rgf)
    ///                            |
    ///                            |----lib-----------shared librarys (.so)
    ///                            |
    ///                            |----mod-----------kernel modules  (.ko)
    ///                            |
    ///                            |----settings------config files    (.rcf, .rtf, .dat)
    ///                            |                  typedata.rcf    (device type data)
    ///                            |
    ///                            |
    ///                            '----lms2012   (executable)
    ///                                 bash files
    /// </code>
    /// </example>
    public static class FileExplorer
    {
        /* 

see bytecodes.h
FOLDERS

#define   vmMEMORY_FOLDER               "/mnt/ramdisk"                //!< Folder for non volatile user programs/data
#define   vmPROGRAM_FOLDER              "../prjs/BrkProg_SAVE"        //!< Folder for On Brick Programming programs
#define   vmDATALOG_FOLDER              "../prjs/BrkDL_SAVE"          //!< Folder for On Brick Data log files
#define   vmSDCARD_FOLDER               "../prjs/SD_Card"             //!< Folder for SD card mount
#define   vmUSBSTICK_FOLDER             "../prjs/USB_Stick"           //!< Folder for USB stick mount

#define   vmPRJS_DIR                    "../prjs"                     //!< Project folder
#define   vmAPPS_DIR                    "../apps"                     //!< Apps folder
#define   vmTOOLS_DIR                   "../tools"                    //!< Tools folder
#define   vmTMP_DIR                     "../tmp"                      //!< Temporary folder

#define   vmSETTINGS_DIR                "../sys/settings"             //!< Folder for non volatile settings

#define   vmDIR_DEEPT                   127                           //!< Max directory items allocated including "." and ".." 
* 
*/

        /// <summary>
        /// Brick root path
        /// </summary>
        public const string ROOT_PATH = "../";


        /// <summary>
        /// Brick projects path
        /// </summary>
        public const string PROJECTS_PATH = "../prjs/";

        /// <summary>
        /// Brick sd card path
        /// </summary>
        public const string SDCARD_PATH = "../prjs/SD_Card/";


        /// <summary>
        /// Brick usb path
        /// </summary>
        public const string USBSTICK_PATH = "../prjs/USB_Stick/";

        /// <summary>
        /// Brick tools path
        /// </summary>
        public const string TOOLS_PATH = "../tools/";

        /// <summary>
        /// Bri9ck application path
        /// </summary>
        public const string APPLICATION_PATH = "../apps/";

        private const string DIRECTORY_SEPERATOR = "/";
        private const string UP_PATH = "./";
        private const string BRICK_NAME_PATH = "../sys/settings/BrickName";


        //brick will allow more like # and @ todo

        /// <summary>
        /// Regular brick fileName (without extension) or directoryname expression
        /// </summary>
        public const string BRICK_NAME_EXPRESSION = "[a-zA-Z0-9 _~]";

        /// <summary>
        /// Regular brick fileName expression
        /// </summary>
        public static string BRICK_FILENAME_EXPRESSION = $@"({BRICK_NAME_EXPRESSION})+(\.(\w)+)?";
        private static readonly string BRICK_DIRECTORY_PATH_EXPRESSION = $@"^\.\./(({BRICK_NAME_EXPRESSION}+)/)*$";
        private static readonly string BRICK_FILE_PATH_EXPRESSION = $@"^\.\./(({BRICK_NAME_EXPRESSION}+)/)*{BRICK_FILENAME_EXPRESSION}$";

        #region  validation checks
        private static string GetValidatedDirectoryPath(this string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (!path.EndsWith(DIRECTORY_SEPERATOR)) path = $"{path}{DIRECTORY_SEPERATOR}";
            if (!Regex.IsMatch(path, BRICK_DIRECTORY_PATH_EXPRESSION)) throw new ArgumentException(nameof(path), "path is not a valid brick directory path");
            return path;
        }

        private static string GetValidateFilePath(this string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (!Regex.IsMatch(path, BRICK_FILE_PATH_EXPRESSION)) throw new ArgumentException(nameof(path), "path is not a valid brick file path");
            return path;
        }

        /// <summary>
        /// Gets a validated directory name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetValidateDirectoryName(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (!Regex.IsMatch(name, $"^{BRICK_NAME_EXPRESSION}+$")) throw new ArgumentException(nameof(name), "name is not a valid brick directory name");
            return name;
        }


        /// <summary>
        /// Gets a validated file name
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetValidateFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException(nameof(fileName));
            if (!Regex.IsMatch(fileName, $"^{BRICK_FILENAME_EXPRESSION}$")) throw new ArgumentException(nameof(fileName), "name is not a valid brick file name");
            return fileName;
        }


        #endregion

        #region core file explorer methods


        /// <summary>
        /// Deletes a file/directory on the brick this can even be a system file.. USE WITH GREAT CARE!!
        /// </summary>
        /// <param name="path">The relative path must start with ../</param>
        /// <returns></returns>
        public static async Task Remove(string path)
        {
            await MemoryMethods.Remove(Brick.Socket, path);
        }

        /// <summary>
        /// Tests if path exists on brick
        /// </summary>
        /// <param name="path">The relative path, must start with ../</param>
        /// <returns><c>true</c> if exists otherwise <c>false</c></returns>
        public static async Task<bool> Exists(string path)
        {
            return await MemoryMethods.Exists(Brick.Socket, path);
        }

        /// <summary>
        /// Uploads a file
        /// </summary>
        /// <param name="localFilePath">The absolute local filePath to a lego file on the local machine</param>
        /// <param name="path">The relative path, must start with ../</param>
        /// <param name="fileName">The fileName as stored on brick incl. extension</param>
        /// <returns><c>true</c> if success otherwise <c>false</c></returns>
        public static async Task<bool> UploadFile(string localFilePath, string path, string fileName)
        {
            if (!System.IO.File.Exists(localFilePath)) throw new ArgumentException(nameof(localFilePath));
            byte[] file = await Task.FromResult(System.IO.File.ReadAllBytes(localFilePath));
            return await UploadFile(file, path, fileName);
        }

        /// <summary>
        /// Uploads a file
        /// </summary>
        /// <param name="stream">The stream to a lego file on the local machine</param>
        /// <param name="path">The relative path must start with ../</param>
        /// <param name="fileName">The fileName as stored on brick incl. extension</param>
        /// <returns><c>true</c> if success otherwise <c>false</c></returns>
        public static async Task<bool> UploadFile(System.IO.Stream stream, string path, string fileName)
        {
            byte[] file = null;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                stream.CopyTo(ms);
                file = ms.ToArray();
            }
            return await UploadFile(file, path, fileName);
        }


        /// <summary>
        /// Uploads a file
        /// </summary>
        /// <param name="file">The byte[] filedata of a lego file on the local machine or resource file</param>
        /// <param name="path">The relative path, must start with ../</param>
        /// <param name="fileName">The renamed fileName as stored on brick incl. extension</param>
        /// <returns><c>true</c> if success otherwise <c>false</c></returns>
        public static async Task<bool> UploadFile(byte[] file, string path, string fileName)
        {
            path = path.GetValidatedDirectoryPath();
            fileName = GetValidateFileName(fileName);

            string brickPath = $"{path}{fileName}";
            SYSTEM_COMMAND_STATUS status = await FileSystemMethods.UploadFileToBrick(Brick.Socket, file, brickPath);
            if (status != SYSTEM_COMMAND_STATUS.SUCCESS && status != SYSTEM_COMMAND_STATUS.FILE_EXISTS) return false;
            return true;
        }


        /// <summary>
        /// Downloads a file from the Brick and returns file as byte[]
        /// </summary>
        /// <param name="path">relative path to the file on the brick</param>
        /// <returns></returns>
        public static async Task<byte[]> DownloadFile(string path)
        {
            path = path.GetValidateFilePath();
            return await FileSystemMethods.DownLoadFileFromBrick(Brick.Socket, path);
        }

        #endregion

        #region directory

        /// <summary>
        /// Deletes a file or directory
        /// </summary>
        /// <param name="path">The relative path, must start with ../</param>
        /// <param name="recursive">if true deletes the directory and all its contents</param>
        /// <returns><c>true</c> if deleted otherwise <c>false</c></returns>
        public static async Task<bool> DeleteDirectory(string path, bool recursive = false)
        {
            path = path.GetValidatedDirectoryPath();
            if (recursive)
            {
                File[] files = await GetFiles(path);
                foreach (File file in files)
                {
                    await DeleteFile(file.Path);
                }

                Directory[] directories = await GetDirectories(path);
                foreach (Directory directory in directories)
                {
                    await DeleteDirectory(directory.Path, recursive);
                }
            }
            SYSTEM_COMMAND_STATUS status = await FileSystemMethods.DeleteFile(Brick.Socket, path);
            return status == SYSTEM_COMMAND_STATUS.SUCCESS;
        }


        /// <summary>
        /// Method creates new directory. An existing directory will not be overriden.
        /// </summary>
        /// <param name="path">The relative path, must start with ../</param>
        /// <returns><c>true</c> if exists otherwise <c>false</c></returns>
        public static async Task<bool> CreateDirectory(string path)
        {
            path = path.GetValidatedDirectoryPath();
            SYSTEM_COMMAND_STATUS status = await FileSystemMethods.CreateDirectory(Brick.Socket, path);
            if (status != SYSTEM_COMMAND_STATUS.SUCCESS && status != SYSTEM_COMMAND_STATUS.FILE_EXISTS) return false;
            return true;
        }

        /// <summary>
        /// Makes directory but uses COM => MemoryMethod
        /// </summary>
        /// <param name="path">The relative path, must start with ../</param>
        /// <returns><c>true</c> if exists otherwise <c>false</c></returns>
        public static async Task<bool> MakeDirectory(string path)
        {
            path = path.GetValidatedDirectoryPath();
            return await MemoryMethods.MakeFolder(Brick.Socket, path);
        }


        /// <summary>
        /// Gets directory handles to directories on the brick. Use with great care upon deleting!!
        /// </summary>
        /// <param name="path">Full relative path to a directory e.g. ../sys/ui/</param>
        /// <returns><c>Directory[]</c></returns>
        public static async Task<Directory[]> GetDirectories(string path)
        {
            path = path.GetValidatedDirectoryPath();

            List<Directory> directories = new List<Directory>();
            string[] entries = await FileSystemMethods.ListFiles(Brick.Socket, path);
            foreach (string entry in entries)
            {
                string item = entry.Trim();

                if (item.EndsWith(DIRECTORY_SEPERATOR))
                {
                    if (item != ROOT_PATH && item != UP_PATH)
                    {
                        string directoryPath = $"{path}{item}";
                        directories.Add(new Directory(directoryPath));
                    }
                }
            }
            directories.Sort(delegate (Directory obj1, Directory obj2) { return obj1.Name.CompareTo(obj2.Name); });
            return directories.ToArray();
        }

        /// <summary>
        /// Gets a directory handle to a directory on the brick. Use with great care upon deleting!!
        /// </summary>
        /// <param name="path">Full relative path to a directory e.g. ../sys/ui/</param>
        /// <returns></returns>
        public static async Task<Directory> GetDirectory(string path)
        {
            path = path.GetValidatedDirectoryPath();
            if (path == ROOT_PATH) return new Directory(ROOT_PATH);
            bool b = await MemoryMethods.Exists(Brick.Socket, path);
            if (b) return new Directory(path);
            return null;
        }


        /// <summary>
        /// Gets the item count of path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task<int> GetItemCount(string path)
        {
            return await MemoryMethods.GetItemCount(Brick.Socket, path.GetValidatedDirectoryPath());
        }

        /// <summary>
        /// Lists all files and directories for given path
        /// </summary>
        /// <param name="path">The relative path, must start with ../</param>
        /// <returns>Files and directories as <c>string[]</c></returns>
        public static async Task<string[]> List(string path)
        {
            return await FileSystemMethods.ListFiles(Brick.Socket, path.GetValidatedDirectoryPath());
        }
        #endregion

        #region file

        /// <summary>
        /// Deletes a file
        /// </summary>
        /// <param name="path">The relative path, must start with ../</param>
        /// <returns><c>true</c> if deleted otherwise <c>false</c></returns>
        public static async Task<bool> DeleteFile(string path)
        {
            path = path.GetValidateFilePath();
            SYSTEM_COMMAND_STATUS status = await FileSystemMethods.DeleteFile(Brick.Socket, path);
            return status == SYSTEM_COMMAND_STATUS.SUCCESS;
        }

        /// <summary>
        /// Gets file handles to files on the brick. Use with great care upon deleting!!
        /// </summary>
        /// <param name="path">Full relative path to a directory e.g. ../sys/ui/</param>
        /// <returns></returns>
        public static async Task<File[]> GetFiles(string path)
        {
            path = path.GetValidatedDirectoryPath();

            List<File> files = new List<File>();
            string[] entries = await Firmware.FileSystemMethods.ListFiles(Brick.Socket, path);
            foreach (string entry in entries)
            {
                string item = entry.Trim();
                //skip directories and empty lines
                if (!string.IsNullOrWhiteSpace(item) && !item.EndsWith(DIRECTORY_SEPERATOR))
                {
                    //fileInfo:   32 chars (hex) of MD5SUM + space + 8 chars (hex) of filesize + space + filename
                    string[] fileInfo = entry.Split(' ');
                    if (fileInfo.Length >= 3)
                    {
                        string md5sum = fileInfo[0].Trim();
                        int byteSize = 0;
                        try
                        {
                            byteSize = Convert.ToInt32(fileInfo[1].Trim(), 16);
                        }
                        catch (OverflowException)
                        {
                        }
                        string fileName = string.Join(" ", fileInfo, 2, fileInfo.Length - 2);
                        if (!string.IsNullOrWhiteSpace(fileName)) files.Add(new File(path, fileName, md5sum, byteSize));
                    }
                }
            }
            files.Sort(delegate (File obj1, File obj2) { return obj1.FileName.CompareTo(obj2.FileName); });
            return files.ToArray();
        }

        /// <summary>
        /// Gets a file handle to a robotfile on the brick. Use with great care upon deleting!!
        /// </summary>
        /// <param name="path">Full path to file e.g. ../sys/ui/Startup.rsf</param>
        /// <returns></returns>
        public static async Task<File> GetFile(string path)
        {
            string fileName = System.IO.Path.GetFileName(path);
            path = System.IO.Path.GetDirectoryName(path).Replace(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
            File[] files = await GetFiles(path);
            foreach (File file in files)
            {
                if (file.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase)) return file;
            }
            return null;
        }

        #endregion



        /// <summary>
        /// Gets the actual brick name
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetBrickName()
        {
            byte[] data = await DownloadFile(BRICK_NAME_PATH);
            string name = Encoding.ASCII.GetString(data);
            return name.TrimEnd('\0', '\n');
        }


        /// <summary>
        /// Test method wether local file is a lego ev3 robot file.
        /// Will test fileName extension only (*.rgf|*.rbf|*.rsf|*.rdf|*.rtf|*.rpf|*.rcf|*.raf)
        /// </summary>
        /// <param name="filePath">Absolute local path or fileName of a lego file on the local machine</param>
        /// <returns></returns>
        public static bool IsRobotFile(string filePath)
        {
            return FileSystemMethods.IsRobotFile(filePath);
        }

        /// <summary>
        /// Gets the file type for given brickpath
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static FileType GetFileType(string filePath)
        {
            return FileSystemMethods.GetFileType(filePath);
        }



        /// <summary>
        /// Returns the entire FileSystem structure as a XmlDocument
        /// </summary>
        /// <returns></returns>
        public static async Task<XmlDocument> FileSystemToXml()
        {
            XmlDocument xd = new XmlDocument();
            XmlElement root = xd.CreateElement("FileSystem");
            await WriteToXml(xd, root, new Directory(ROOT_PATH));
            xd.AppendChild(root);
            return xd;
        }

        private static async Task WriteToXml(XmlDocument xd, XmlElement parent, Directory directory)
        {
            Directory[] dirs = await GetDirectories(directory.Path);
            foreach (Directory dir in dirs)
            {
                XmlElement de = xd.CreateElement("Directory");
                de.SetAttribute("Path", dir.Path);
                de.SetAttribute("Name", dir.Name);
                parent.AppendChild(de);

                await WriteToXml(xd, de, dir);

            }

            File[] files = await directory.GetFiles();
            foreach (File file in files)
            {
                XmlElement fe = xd.CreateElement("File");
                fe.SetAttribute("Path", file.Path);
                fe.SetAttribute("Name", file.FileName);
                fe.SetAttribute("Size", file.FileSize());
                fe.SetAttribute("Type", file.Type.ToString());
                fe.SetAttribute("MD5SUM", file.MD5SUM);
                fe.SetAttribute("Bytes", file.Size.ToString());
                parent.AppendChild(fe);
            }
        }
    }
}
