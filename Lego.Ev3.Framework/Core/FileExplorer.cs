using Lego.Ev3.Framework.Firmware;
using System;
using System.Collections.Generic;
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
        internal const string ROOT_PATH = "../";
        internal const string DIRECTORY_SEPERATOR = "/";
        internal const string ALTERNATE_DIRECTORY_SEPERATOR = "\\";

        #region extension path checks
        private static string ToBrickPath(this string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            path = path.Replace(ALTERNATE_DIRECTORY_SEPERATOR, DIRECTORY_SEPERATOR);
            if (!path.EndsWith(DIRECTORY_SEPERATOR)) path = $"{path}{DIRECTORY_SEPERATOR}";
            if (!path.StartsWith(ROOT_PATH)) throw new ArgumentException(nameof(path), "path is not a valid brick path");
            return path;
        }

        private static string ToBrickDirectoryName(this string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (!name.Contains(DIRECTORY_SEPERATOR) || name.Contains(ALTERNATE_DIRECTORY_SEPERATOR)) throw new ArgumentException(nameof(name), "name is not a valid brick directory name");
            return name;
        }

        private static string ToBrickFileName(this string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (!name.Contains(DIRECTORY_SEPERATOR) || name.Contains(ALTERNATE_DIRECTORY_SEPERATOR)) throw new ArgumentException(nameof(name), "name is not a valid brick file name");
            return name;
        }
        #endregion

        #region core file explorer methods
        /// <summary>
        /// Lists all files and directories for given path
        /// </summary>
        /// <param name="path">The relative path, must start with ../</param>
        /// <returns>Files and directories as <c>string[]</c></returns>
        public static async Task<string[]> List(string path)
        {
            return await FileSystemMethods.ListFiles(Brick.Socket, path.ToBrickPath());
        }

        /// <summary>
        /// Deletes a file or directory
        /// </summary>
        /// <param name="path">The relative path, must start with ../</param>
        /// <returns><c>true</c> if deleted otherwise <c>false</c></returns>
        public static async Task<bool> Delete(string path)
        {
            SYSTEM_COMMAND_STATUS status = await FileSystemMethods.DeleteFile(Brick.Socket, path.ToBrickPath());
            return status == SYSTEM_COMMAND_STATUS.SUCCESS;
        }

        /// <summary>
        /// Deletes a file/directory on the brick this can even be a system file.. USE WITH GREAT CARE!!
        /// </summary>
        /// <param name="path">The relative path must start with ../</param>
        /// <returns></returns>
        public static async Task Remove(string path)
        {
            await MemoryMethods.Remove(Brick.Socket, path.ToBrickPath());
        }

        /// <summary>
        /// Tests if path exists on brick
        /// </summary>
        /// <param name="path">The relative path, must start with ../</param>
        /// <returns><c>true</c> if exists otherwise <c>false</c></returns>
        public static async Task<bool> Exists(string path)
        {
            return await MemoryMethods.Exists(Brick.Socket, path.ToBrickPath());
        }

        /// <summary>
        /// Method creates new directory. An existing directory will not be overriden.
        /// </summary>
        /// <param name="path">The relative path, must start with ../</param>
        /// <param name="name">The name of the directory</param>
        /// <returns><c>true</c> if exists otherwise <c>false</c></returns>
        public static async Task<bool> CreateDirectory(string path)
        {
            path = path.ToBrickPath();
            System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(path)).ToBrickDirectoryName();
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
            path = path.ToBrickPath();
            System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(path)).ToBrickDirectoryName();
            return await MemoryMethods.MakeFolder(Brick.Socket, path);
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
            path = path.ToBrickPath();
            fileName = fileName.ToBrickFileName();

            string brickPath = System.IO.Path.Combine(path, fileName);
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
            string fileName = System.IO.Path.GetFileName(path).ToBrickFileName();
            path = System.IO.Path.GetDirectoryName(path);
            path = path.ToBrickPath();
            path = System.IO.Path.Combine(path, fileName);
            return await FileSystemMethods.DownLoadFileFromBrick(Brick.Socket, path);
        }

        /// <summary>
        /// Gets the item count of path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task<int> GetItemCount(string path)
        {
            return await MemoryMethods.GetItemCount(Brick.Socket, path.ToBrickPath());
        }
        #endregion

        #region directory

        /// <summary>
        /// Gets directory handles to directories on the brick. Use with great care upon deleting!!
        /// </summary>
        /// <param name="path">Full relative path to a directory e.g. ../sys/ui/</param>
        /// <returns><c>Directory[]</c></returns>
        public static async Task<Directory[]> GetDirectories(string path)
        {
            path = path.ToBrickPath();

            List<Directory> directories = new List<Directory>();
            string[] entries = await FileSystemMethods.ListFiles(Brick.Socket, path);
            foreach (string entry in entries)
            {
                string item = entry.Trim();

                if (item.EndsWith(DIRECTORY_SEPERATOR))
                {
                    if (item != ROOT_PATH)
                    {
                        string directoryPath = System.IO.Path.Combine(path, item);
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
            path = path.ToBrickPath();
            if (path == ROOT_PATH) return new Directory(ROOT_PATH);
            bool b = await MemoryMethods.Exists(Brick.Socket, path);
            if (b) return new Directory(path);
            return null;
        }

        #endregion

        #region file

        /// <summary>
        /// Gets file handles to files on the brick. Use with great care upon deleting!!
        /// </summary>
        /// <param name="path">Full relative path to a directory e.g. ../sys/ui/</param>
        /// <returns></returns>
        public static async Task<File[]> GetFiles(string path)
        {
            path = path.ToBrickPath();

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
                        int byteSize = Convert.ToInt32(fileInfo[1].Trim(), 16);
                        string fileName = "";
                        for (int i = 2; i < fileInfo.Length; i++)
                        {
                            fileName += fileInfo[i];
                            fileName += " ";
                        }
                        fileName = fileName.Trim();
                        files.Add(new File(path, fileName, md5sum, byteSize));
                    }
                }
            }
            files.Sort(delegate (File obj1, File obj2) { return obj1.Name.CompareTo(obj2.Name); });
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
            path = System.IO.Path.GetDirectoryName(path);
            path = path.ToBrickPath();
            File[] files = await GetFiles(path);
            foreach (File file in files)
            {
                if (file.Name.Equals(fileName, StringComparison.InvariantCultureIgnoreCase)) return file;
            }
            return null;
        }

        #endregion


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
                fe.SetAttribute("Name", file.Name);
                fe.SetAttribute("Size", file.FileSize());
                fe.SetAttribute("Type", file.Type.ToString());
                fe.SetAttribute("MD5SUM", file.MD5SUM);
                fe.SetAttribute("Bytes", file.Size.ToString());
                parent.AppendChild(fe);
            }
        }
    }
}
