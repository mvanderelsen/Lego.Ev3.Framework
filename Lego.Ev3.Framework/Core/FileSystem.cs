using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using F = Lego.Ev3.Framework.Firmware;
using I = System.IO;

namespace Lego.Ev3.Framework.Core
{
    /// <summary>
    /// A lego Ev3 Filesystem
    /// Files can be placed anywhere on the drive but if placed directly on a drive root files they won't show up in the UI dialog browser
    /// So no way to delete files/resources manually when developer forgets to to do resource clean up from project.
    /// Make file upload method(s) restricted for Directory class for now and leave number of levels to 1 directory here ;-)
    /// Ps. Linux is case sensitive for folders and files.
    /// </summary>
    public abstract class FileSystem
    {
        /// <summary>
        /// The relative path to this filesystem on brick, sdcard or usb stick.
        /// </summary>
        public string Path { get; private set; }


        /// <summary>
        /// Initializes FileSystem class and sets up fileSystem Path to provided predefined path
        /// </summary>
        /// <param name="path">the predefined brick path</param>
        protected FileSystem(FileSystemPath path)
        {
            Path = path.GetRelativePath();
        }


        /// <summary>
        /// Gets all directories on this drive
        /// </summary>
        /// <returns></returns>
        public async Task<Directory[]> GetDirectories()
        {
            List<Directory> directories = await Directory.GetDirectories(Path);
            return directories.ToArray();
        }

        /// <summary>
        /// Gets a directory on this drive
        /// </summary>
        /// <returns>null if not found otherwise the directory</returns>
        public async Task<Directory> GetDirectory(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            name = name.ToLowerInvariant().Trim();
            List<Directory> directories = await Directory.GetDirectories(Path);
            return directories.Find(t => t.Name.ToLowerInvariant() == name);
        }

        /// <summary>
        /// Checks if directory exists
        /// </summary>
        /// <param name="name">Name of the directory</param>
        /// <returns>True if exists</returns>
        public async Task<bool> DirectoryExists(string name)
        {
            name = name.ToLowerInvariant().Trim();
            Directory[] dirs = await GetDirectories();
            foreach (Directory dir in dirs)
            {
                if (dir.Name.ToLowerInvariant() == name) return true;
            }
            return false;
        }

        /// <summary>
        /// Method creates new directory. Any existing directory will not be overriden.
        /// </summary>
        /// <param name="name">The name of the directory</param>
        /// <exception cref="ArgumentNullException">Name must be provided</exception>
        /// <exception cref="ArgumentException">Name can not contains slashes or be a reserved system name</exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns>Directory</returns>
        public async Task<Directory> CreateDirectory(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("name", "Name must be provided");
            if (name.Contains("/") || name.Contains("\\")) throw new ArgumentException("name", "Name can not contain slashes");

            if (Directory.IsSystemDirectory(name)) throw new ArgumentException("name", "Name has reserved system value: " + name);

            string directoryPath = I.Path.Combine(Path, name + "/");
            F.SYSTEM_COMMAND_STATUS status = await Firmware.FileSystemMethods.CreateDirectory(Brick.Socket, directoryPath);
            if (status != F.SYSTEM_COMMAND_STATUS.SUCCESS && status != F.SYSTEM_COMMAND_STATUS.FILE_EXITS)
            {
                throw new InvalidOperationException(status.ToString());
            }
            return new Directory(directoryPath);
        }
    }
}
