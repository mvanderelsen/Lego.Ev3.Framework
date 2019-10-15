using Lego.Ev3.Framework.Firmware;
using System;
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
    public static class BrickExplorer
    {

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
        /// Brick application path
        /// </summary>
        public const string APPLICATION_PATH = "../apps/";



        #region core file explorer methods


        /// <summary>
        /// Tests if path to file or folder exists on brick
        /// </summary>
        /// <param name="brickPath">The relative path, must start with ../</param>
        /// <returns><c>true</c> if exists otherwise <c>false</c></returns>
        public static async Task<bool> Exists(string brickPath)
        {
            return await MemoryMethods.Exists(Brick.Socket, brickPath);
        }

        /// <summary>
        /// Uploads a file
        /// </summary>
        /// <param name="localFilePath">The absolute local filePath to a lego file on the local machine</param>
        /// <param name="brickDirectoryPath">The relative path, must start with ../ and end with /</param>
        /// <param name="fileName">The fileName as stored on brick incl. extension</param>
        /// <returns><c>true</c> if success otherwise <c>false</c></returns>
        public static async Task<bool> UploadFile(string localFilePath, string brickDirectoryPath, string fileName)
        {
            if (!System.IO.File.Exists(localFilePath)) throw new ArgumentException(nameof(localFilePath));
            byte[] file = System.IO.File.ReadAllBytes(localFilePath);
            return await UploadFile(file, brickDirectoryPath, fileName);
        }

        /// <summary>
        /// Uploads a file
        /// </summary>
        /// <param name="stream">The stream to a lego file on the local machine</param>
        /// <param name="brickDirectoryPath">The relative path must start with ../ and end with /</param>
        /// <param name="fileName">The fileName as stored on brick incl. extension</param>
        /// <returns><c>true</c> if success otherwise <c>false</c></returns>
        public static async Task<bool> UploadFile(System.IO.Stream stream, string brickDirectoryPath, string fileName)
        {
            byte[] file = null;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                stream.CopyTo(ms);
                file = ms.ToArray();
            }
            return await UploadFile(file, brickDirectoryPath, fileName);
        }


        /// <summary>
        /// Uploads a file
        /// </summary>
        /// <param name="file">The byte[] filedata of a lego file on the local machine or resource file</param>
        /// <param name="brickDirectoryPath">The relative path, must start with ../ and end with /</param>
        /// <param name="fileName">The renamed fileName as stored on brick incl. extension</param>
        /// <returns><c>true</c> if success otherwise <c>false</c></returns>
        public static async Task<bool> UploadFile(byte[] file, string brickDirectoryPath, string fileName)
        {
            brickDirectoryPath = Firmware.FileSystem.ToBrickDirectoryPath(brickDirectoryPath);
            string brickFilePath = $"{brickDirectoryPath}{fileName}";
            return await SystemMethods.UploadFile(Brick.Socket, file, brickFilePath);
        }


        /// <summary>
        /// Downloads a file from the Brick and returns file as byte[]
        /// </summary>
        /// <param name="brickFilePath">relative path to the file on the brick</param>
        /// <returns></returns>
        public static async Task<byte[]> DownloadFile(string brickFilePath)
        {
            return await SystemMethods.DownLoadFile(Brick.Socket, brickFilePath);
        }

        #endregion

        #region directory

        /// <summary>
        /// Deletes a file or directory
        /// </summary>
        /// <param name="brickDirectoryPath">The relative path, must start with ../ and end with /</param>
        /// <param name="recursive">if true deletes the directory and all its contents</param>
        /// <returns><c>true</c> if deleted otherwise <c>false</c></returns>
        public static async Task<bool> DeleteDirectory(string brickDirectoryPath, bool recursive = false)
        {
            brickDirectoryPath = Firmware.FileSystem.ToBrickDirectoryPath(brickDirectoryPath);
            if (recursive)
            {
                DirectoryContent content = await GetDirectoryContent(brickDirectoryPath);
                foreach (File file in content.Files)
                {
                    await DeleteFile(file.Path);
                }

                foreach (Directory directory in content.Directories)
                {
                    await DeleteDirectory(directory.Path, recursive);
                }
            }
            return await SystemMethods.Delete(Brick.Socket, brickDirectoryPath);
        }


        /// <summary>
        /// Method creates new directory. An existing directory will not be overriden.
        /// </summary>
        /// <param name="brickDirectoryPath">The relative path, must start with ../ and end with /</param>
        /// <returns><c>true</c> if exists otherwise <c>false</c></returns>
        public static async Task<bool> CreateDirectory(string brickDirectoryPath)
        {
            return await SystemMethods.CreateDirectory(Brick.Socket, brickDirectoryPath);
        }

        /// <summary>
        /// Gets directory handles to directories on the brick. Use with great care upon deleting!!
        /// </summary>
        /// <param name="brickDirectoryPath">Full relative path to a directory e.g. ../sys/ui/</param>
        /// <returns><c>Directory[]</c></returns>
        public static async Task<Directory[]> GetDirectories(string brickDirectoryPath)
        {
            DirectoryContent content = await SystemMethods.GetDirectoryContent(Brick.Socket, brickDirectoryPath);
            return content.Directories;

        }

        /// <summary>
        /// Gets a directory handle to a directory on the brick. Use with great care upon deleting!!
        /// </summary>
        /// <param name="brickDirectoryPath">The relative path, must start with ../ and end with /</param>
        /// <returns></returns>
        public static async Task<Directory> GetDirectory(string brickDirectoryPath)
        {
            brickDirectoryPath = Firmware.FileSystem.ToBrickDirectoryPath(brickDirectoryPath);
            if (brickDirectoryPath == ROOT_PATH) return new Directory(ROOT_PATH);
            bool b = await MemoryMethods.Exists(Brick.Socket, brickDirectoryPath);
            if (b) return new Directory(brickDirectoryPath);
            return null;
        }


        /// <summary>
        /// Lists all files and directories for given path
        /// </summary>
        /// <param name="brickDirectoryPath">The relative path, must start with ../ and end with /</param>
        /// <returns>Files and directories as <c>string[]</c></returns>
        public static async Task<DirectoryContent> GetDirectoryContent(string brickDirectoryPath)
        {
            return await SystemMethods.GetDirectoryContent(Brick.Socket, brickDirectoryPath);
        }
        #endregion

        #region file

        /// <summary>
        /// Deletes a file
        /// </summary>
        /// <param name="brickFilePath">The relative path, must start with ../</param>
        /// <returns><c>true</c> if deleted otherwise <c>false</c></returns>
        public static async Task<bool> DeleteFile(string brickFilePath)
        {
            return await SystemMethods.Delete(Brick.Socket, brickFilePath);
        }

        /// <summary>
        /// Gets file handles to files on the brick. Use with great care upon deleting!!
        /// </summary>
        /// <param name="brickDirectoryPath">The relative path, must start with ../ and end with /</param>
        /// <returns></returns>
        public static async Task<File[]> GetFiles(string brickDirectoryPath)
        {
            DirectoryContent content = await SystemMethods.GetDirectoryContent(Brick.Socket, brickDirectoryPath);
            return content.Files;
        }

        /// <summary>
        /// Gets a file handle to a robotfile on the brick. Use with great care upon deleting!!
        /// </summary>
        /// <param name="brickFilePath">Full path to file e.g. ../sys/ui/Startup.rsf</param>
        /// <returns></returns>
        public static async Task<File> GetFile(string brickFilePath)
        {
            string fileName = System.IO.Path.GetFileName(brickFilePath);
            string brickDirectoryPath = brickFilePath.Substring(0, brickFilePath.LastIndexOf(Firmware.FileSystem.DIRECTORY_SEPERATOR));
            File[] files = await GetFiles(brickDirectoryPath);
            foreach (File file in files)
            {
                if (file.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase)) return file;
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
            return File.IsRobotFile(filePath);
        }

        /// <summary>
        /// Gets the file type for given brickpath
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static FileType GetFileType(string filePath)
        {
            return File.GetFileType(filePath);
        }



        /// <summary>
        /// Returns the entire FileSystem structure as a XmlDocument
        /// </summary>
        /// <returns></returns>
        public static async Task<XmlDocument> FileSystemToXml()
        {
            XmlDocument xd = new XmlDocument();
            XmlElement root = xd.CreateElement("EV3");
            await WriteToXml(xd, root, new Directory(ROOT_PATH));
            xd.AppendChild(root);
            return xd;
        }

        private static async Task WriteToXml(XmlDocument xd, XmlElement parent, Directory directory)
        {
            DirectoryContent content = await GetDirectoryContent(directory.Path);
            foreach (Directory dir in content.Directories)
            {
                XmlElement de = xd.CreateElement("Directory");
                de.SetAttribute("Path", dir.Path);
                de.SetAttribute("Name", dir.Name);
                parent.AppendChild(de);

                await WriteToXml(xd, de, dir);

            }
            foreach (File file in content.Files)
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
