using Lego.Ev3.Framework.Core;
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
        public string Name { get; }

        /// <summary>
        /// Relative path to the directory
        /// </summary>
        public string Path { get; }

        internal Directory(string path)
        {
            Path = path;
            Name = System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(path));
        }

        /// <summary>
        /// Gets all files in this directory
        /// </summary>
        /// <returns><c>File[]</c></returns>
        public async Task<File[]> GetFiles()
        {
            return await BrickExplorer.GetFiles(Path);
        }


        /// <summary>
        /// Gets a file in this directory by file name
        /// </summary>
        /// <param name="fileName">filename incl. extension</param>
        /// <returns><c>File</c></returns>
        public async Task<File> GetFile(string fileName)
        {
            string path = $"{Path}{fileName}";
            return await BrickExplorer.GetFile(path);
        }

        /// <summary>
        /// Gets all soundfiles in this directory
        /// </summary>
        /// <returns>an array of files</returns>
        public async Task<SoundFile[]> GetSoundFiles()
        {
            File[] files = await GetFiles();
            List<SoundFile> sounds = new List<SoundFile>();
            foreach (File file in files)
            {
                if (file.Type == FileType.SoundFile)
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
            File[] files = await GetFiles();
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
            File[] files = await GetFiles();
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
        /// <param name="fileName">The renamed fileName as stored on brick incl. extension</param>
        /// <returns></returns>
        public async Task<File> UploadFile(string localFilePath, string fileName)
        {
            bool success = await BrickExplorer.UploadFile(localFilePath, Path, fileName);
            if (success) return await GetFile(fileName);
            return null;
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
            fileName = $"{System.IO.Path.GetFileNameWithoutExtension(fileName)}{File.GetExtension(type)}";
            bool success = await BrickExplorer.UploadFile(file, Path, fileName);
            if (success) return await GetFile(fileName);
            return null;
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

            fileName = System.IO.Path.ChangeExtension(fileName, File.GetExtension(type));
            bool success = await BrickExplorer.UploadFile(stream, Path, fileName);
            if (success) return await GetFile(fileName);
            return null;
        }


        /// <summary>
        /// Deletes this lego brick directory and all files in this directory if recursive = true
        /// </summary>
        /// <param name="recursive">if false will not delete directory if directory contains files</param>
        public async Task<bool> Delete(bool recursive = false)
        {
            //first check if not is system dir before deleting inner files
            if (FileSystem.IsReservedDirectoryName(Name)) return false;

            if (!recursive) return await Delete();

            File[] files = await GetFiles();
            foreach (File file in files)
            {
                if (!await file.Delete()) return false;
            }

            return await Delete();
        }
    }
}
