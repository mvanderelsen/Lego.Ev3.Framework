using Lego.Ev3.Framework.Firmware;
using Lego.Ev3.Framework.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Lego.Ev3.Framework
{
    /// <summary>
    /// A directory on the filesystem of a drive
    /// Directories can not contain subdirectories only files
    /// </summary>
    public class Directory
    {
        /// <summary>
        /// Name of the directory
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Relative path to the directory
        /// </summary>
        public string Path { get; private set; }

        internal Directory(string path)
        {
            Path = path;
            Name = System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(path));
        }


        internal static async Task<List<Directory>> GetDirectories(string path)
        {
            //check to filter for SD and USB drive
            bool filter = path == FileSystemPath.Projects.GetRelativePath() || path == FileSystemPath.SDCard.GetRelativePath();
            return await GetDirectories(path, filter);
        }

        /// <summary>
        /// Use this method from Firmware.FileSystem Console only to set filtering to false!!
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        internal static async Task<List<Directory>> GetDirectories(string path, bool filter)
        {
            List<Directory> directories = new List<Directory>();
            string[] entries = await FileSystemMethods.ListFiles(Brick.Socket, path);
            foreach (string entry in entries)
            {
                string item = entry.Trim();

                if (item.EndsWith("/"))
                {
                    bool isParentPath = string.IsNullOrWhiteSpace(item.Replace("/", "").Replace(".", ""));
                    if (!isParentPath)
                    {
                        string directoryPath = System.IO.Path.Combine(path, item);
                        if (filter)
                        {
                            Directory dir = new Directory(directoryPath);
                            if (!IsSystemDirectory(dir.Name))
                            {
                                directories.Add(dir);
                            }

                        }
                        else directories.Add(new Directory(directoryPath));
                    }
                }
            }
            directories.Sort(delegate(Directory obj1, Directory obj2) { return obj1.Name.CompareTo(obj2.Name); });
            return directories;
        }


        //DOES NOT WORK PROPERLY
        //public async Task<DirectoryInfo> GetDirectoryInfo()
        //{
        //    int[] values = await M.MemoryMethods.GetDirectoryInfo(Brick.InternalSocket, Path);
        //    return new DirectoryInfo(values);
        //}

        /// <summary>
        /// Gets all files in this directory
        /// </summary>
        /// <returns>an array of files</returns>
        public async Task<File[]> GetFiles()
        {
            List<File> files = await File.GetFiles(Path);
            return files.ToArray();
        }

        /// <summary>
        /// Gets all soundfiles in this directory
        /// </summary>
        /// <returns>an array of files</returns>
        public async Task<SoundFile[]> GetSoundFiles()
        {
            List<File> files = await File.GetFiles(Path);
            List<SoundFile> sounds = new List<SoundFile>();
            foreach (File file in files)
            {
                if(file.Type == FileType.SoundFile)
                {
                    sounds.Add((SoundFile)file);
                }
            }
            return sounds.ToArray();
        }

        /// <summary>
        /// Gets all graphicfiles in this directory
        /// </summary>
        /// <returns>an array of files</returns>
        public async Task<GraphicFile[]> GraphicFiles()
        {
            List<File> files = await File.GetFiles(Path);
            List<GraphicFile> graphics = new List<GraphicFile>();
            foreach (File file in files)
            {
                if (file.Type == FileType.GraphicFile)
                {
                    graphics.Add((GraphicFile)file);
                }
            }
            return graphics.ToArray();
        }

        /// <summary>
        /// Gets all textfiles in this directory
        /// </summary>
        /// <returns>an array of files</returns>
        public async Task<TextFile[]> TextFiles()
        {
            List<File> files = await File.GetFiles(Path);
            List<TextFile> texts = new List<TextFile>();
            foreach (File file in files)
            {
                if (file.Type == FileType.TextFile)
                {
                    texts.Add((TextFile)file);
                }
            }
            return texts.ToArray();
        }

        /// <summary>
        /// Uploads a file to this directory. Any existing file will not be overriden.
        /// </summary>
        /// <param name="localFilePath">The absolute local filePath to a lego file on the local machine</param>
        /// <returns></returns>
        public async Task<File> UploadFile(string localFilePath)
        {
            return await UploadFile(localFilePath, System.IO.Path.GetFileName(localFilePath));
        }

        /// <summary>
        /// Uploads a file to this directory. Any existing file will not be overriden.
        /// </summary>
        /// <param name="localFilePath">The absolute local filePath to a lego file on the local machine</param>
        /// <param name="fileName">The renamed fileName as stored on brick excl. extension</param>
        /// <returns></returns>
        public async Task<File> UploadFile(string localFilePath, string fileName)
        {

            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = System.IO.Path.GetFileName(localFilePath);
            }

            fileName = System.IO.Path.ChangeExtension(fileName, System.IO.Path.GetExtension(localFilePath));

            string brickPath = System.IO.Path.Combine(Path,fileName);

            SYSTEM_COMMAND_STATUS status = await FileSystemMethods.UploadFileToBrick(Brick.Socket, localFilePath, brickPath);

            if (status != SYSTEM_COMMAND_STATUS.SUCCESS && status != SYSTEM_COMMAND_STATUS.FILE_EXITS)
            {
                throw new InvalidOperationException(status.ToString());
            }

            return await File.GetFile(Path, fileName);
        }


        /// <summary>
        /// Uploads a file to this directory. Any existing file will not be overriden.
        /// </summary>
        /// <param name="file">The byte[] filedata of a lego file on the local machine or resource file</param>
        /// <param name="type">The type of the filedata</param>
        /// <param name="fileName">The renamed fileName as stored on brick excl. extension</param>
        /// <returns></returns>
        public async Task<File> UploadFile(byte[] file, FileType type, string fileName)
        {

            fileName = System.IO.Path.ChangeExtension(fileName, FileSystemMethods.GetExtension(type));

            string brickPath = System.IO.Path.Combine(Path, fileName);

            SYSTEM_COMMAND_STATUS status = await FileSystemMethods.UploadFileToBrick(Brick.Socket, file, brickPath);

            if (status != SYSTEM_COMMAND_STATUS.SUCCESS && status != SYSTEM_COMMAND_STATUS.FILE_EXITS)
            {
                throw new InvalidOperationException(status.ToString());
            }

            return await File.GetFile(Path, fileName);
        }


        /// <summary>
        /// Uploads a file to this directory
        /// </summary>
        /// <param name="stream">The stream to a lego file on the local machine or resource file</param>
        /// <param name="type">The type of the filedata</param>
        /// <param name="fileName">The renamed fileName as stored on brick excl. extension</param>
        /// <returns></returns>
        public async Task<File> UploadFile(System.IO.Stream stream, FileType type, string fileName)
        {

            fileName = System.IO.Path.ChangeExtension(fileName, FileSystemMethods.GetExtension(type));

            string brickPath = System.IO.Path.Combine(Path, fileName);

            SYSTEM_COMMAND_STATUS status = await FileSystemMethods.UploadFileToBrick(Brick.Socket, stream, brickPath);
            if (status != SYSTEM_COMMAND_STATUS.SUCCESS && status != SYSTEM_COMMAND_STATUS.FILE_EXITS)
            {
                throw new InvalidOperationException(status.ToString());
            }

            return await File.GetFile(Path, fileName);
        }

        /// <summary>
        /// Deletes this lego brick directory. Will not delete directory when not empty
        /// Use with care!!
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task Delete()
        {
            if (!IsSystemDirectory(Name))
            {
                SYSTEM_COMMAND_STATUS status = await FileSystemMethods.DeleteFile(Brick.Socket, Path);
                if(status != SYSTEM_COMMAND_STATUS.SUCCESS)
                {
                    throw new InvalidOperationException(status.ToString());
                }
            }
            else
            {
                throw new InvalidOperationException(SYSTEM_COMMAND_STATUS.ILLEGAL_PATH.ToString());
            }
        }

        /// <summary>
        /// Deletes this lego brick directory and all files in this directory if recursive = true
        /// </summary>
        /// <param name="recursive">if false will not delete directory if directory contains files</param>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task Delete(bool recursive = false)
        {
            //first check if not is system dir before deleting inner files
            if (!IsSystemDirectory(Name))
            {
                if (!recursive)
                {
                    await Delete();
                    return;
                }

                File[] files = await GetFiles();
                foreach (File file in files)
                {
                    await file.Delete();
                }

                await Delete();
            }
            else
            {
                throw new InvalidOperationException(SYSTEM_COMMAND_STATUS.ILLEGAL_PATH.ToString());
            }
        }



        /*
         * 
         * DATA8     NoOfFavourites[SORT_TYPES] =
{
  0,  // -
  6,  // Prjs
  5,  // Apps
  8   // Tools
};

DATA8     FavouriteExts[FILETYPES] =
{
  [TYPE_BYTECODE] = 1,
  [TYPE_SOUND]    = 2,
  [TYPE_GRAPHICS] = 3,
  [TYPE_TEXT]     = 4,
};

char      *pFavourites[SORT_TYPES][8] =
{ // Priority
  //  0             1                 2                 3                 4
  { },
  { "",           "BrkProg_SAVE",   "BrkDL_SAVE",     "SD_Card",        "USB_Stick",          "TEST"   },
  { "Port View",  "Motor Control",  "IR Control",     "Brick Program",  "Brick Datalog" },
  { "Volume",     "Sleep",          "Bluetooth",      "WiFi",           "Brick Name",         "Brick Info",         "Test",       "Debug" }
};
         * */

        /// <summary>
        /// Checks if directoryname is a predefined system name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsSystemDirectory(string name)
        {
            name = name.ToLowerInvariant();
            switch (name)
            {
                case "":
                case "sd_card":
                case "usb_stick":
                case "brkprog_save":
                case "brkdl_save":
                case "test":
                    {
                        return true;
                    }
            }
            return false;
        }
    }
}
