using System;
using System.Threading.Tasks;
using Lego.Ev3.Framework.Firmware;
using I = System.IO;
namespace Lego.Ev3.Framework.Core
{
    /// <summary>
    /// A base class for soundfiles, graphicfiles
    /// </summary>
    public abstract class FileHandle
    {

        /// <summary>
        /// Gets or sets if file is default on brick through firmware resource so not deletable
        /// </summary>
        /// <see cref="Sound.GetOnBrickSoundFiles"/>
        internal bool IsOnBrickFile { get; set; }

        /// <summary>
        /// Name of the sound file
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Full relative path on brick to file
        /// </summary>
        public string FilePath { get; internal set; }


        /// <summary>
        /// Creates a simple slim filehandle
        /// </summary>
        /// <param name="fileName">name of the file as on brick</param>
        /// <param name="filePath">full relative path to file on brick</param>
        protected FileHandle(string fileName, string filePath)
        {
            FileName = fileName;
            FilePath = filePath;
        }

        /// <summary>
        /// Deletes this file
        /// Use with care!!
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task Delete()
        {
            if (IsOnBrickFile) return; // do not allow deletion of firmware resources

            SYSTEM_COMMAND_STATUS status = await FileSystemMethods.DeleteFile(Brick.Socket, FilePath);
            if (status != SYSTEM_COMMAND_STATUS.SUCCESS) throw new InvalidOperationException(status.ToString());
        }

        /// <summary>
        /// Downloads this file from the Brick and returns this file as byte[]
        /// </summary>
        /// <returns>byte[] data of the file</returns>
        public async Task<byte[]> Download()
        {
            return await FileSystemMethods.DownLoadFileFromBrick(Brick.Socket, FilePath);
        }

        /// <summary>
        /// Downloads the file to specified directory path. if directory does not exists it creates the directory
        /// </summary>
        /// <param name="path">path to a directory</param>
        /// <returns></returns>
        public async Task Download(string path)
        {
            byte[] data = await Download();
            await Download(path, FileName, data);
        }


        protected async Task Download(string path, string fileName, byte[] data)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));
            path = I.Path.GetDirectoryName(path);
            if (!I.Directory.Exists(path)) I.Directory.CreateDirectory(path);
            path = I.Path.Combine(path, fileName);
            using (I.FileStream fileStream = I.File.Create(path))
            {
                await fileStream.WriteAsync(data, 0, data.Length);
            }
        }
    }
}
