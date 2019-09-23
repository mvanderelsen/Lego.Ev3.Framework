using System;
using System.Linq;
using System.Threading.Tasks;

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
        private readonly string _path;


        /// <summary>
        /// Initializes FileSystem class and sets up fileSystem Path to provided predefined path
        /// </summary>
        /// <param name="path">the predefined brick path</param>
        protected FileSystem(string path)
        {
            _path = path;
        }


        /// <summary>
        /// Gets all directories on this drive
        /// </summary>
        /// <returns></returns>
        public async Task<Directory[]> GetDirectories()
        {
            Directory[] directories  = await FileExplorer.GetDirectories(_path);
            return directories.Where(d => !IsReservedDirectoryName(d.Name)).ToArray();
        }

        /// <summary>
        /// Gets a directory on this drive
        /// </summary>
        /// <returns>null if not found otherwise the directory</returns>
        /// <exception cref="ArgumentNullException">name is required</exception>
        public async Task<Directory> GetDirectory(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if (IsReservedDirectoryName(name)) return null;
            string path = $"{_path}{name}";
            return await FileExplorer.GetDirectory(path);
        }

        /// <summary>
        /// Checks if directory exists
        /// </summary>
        /// <param name="name">name of the directory</param>
        /// <returns><c>true</c> if exists otherwise <c>false</c></returns>
        public async Task<bool> DirectoryExists(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            if (IsReservedDirectoryName(name)) return false;
            string path = $"{_path}{name}";
            return await FileExplorer.Exists(path);
        }

        /// <summary>
        /// Method creates new directory. Any existing directory will not be overriden.
        /// </summary>
        /// <param name="name">The name of the directory</param>
        /// <returns><c>Directory</c> if exists otherwise <c>null</c></returns>
        public async Task<Directory> CreateDirectory(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            if (IsReservedDirectoryName(name)) return null;
            string path = $"{_path}{name}";
            bool success = await FileExplorer.CreateDirectory(path);
            if (success) return new Directory(path);
            return null;
        }


        public static bool IsReservedDirectoryName(string name)
        {
            switch (name.ToLowerInvariant())
            {
                case "sd_card":
                case "usb_stick":
                case "brkprog_save":
                case "brkdl_save":
                case "":
                case ".":
                case "..":
                    {
                        return true;
                    }
            }
            return false;
        }

    }
}
