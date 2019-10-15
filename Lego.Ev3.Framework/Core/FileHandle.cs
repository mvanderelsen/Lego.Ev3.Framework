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
        /// Name of the file
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Full relative brick file path
        /// </summary>
        public string FilePath { get; internal set; }


        /// <summary>
        /// Creates a simple slim filehandle
        /// </summary>
        /// <param name="fileName">name of the file as on brick</param>
        /// <param name="filePath">full relative brick file path</param>
        protected FileHandle(string fileName, string filePath)
        {
            FileName = fileName;
            FilePath = filePath;
        }

        /// <summary>
        /// Deletes this file
        /// Use with care!!
        /// </summary>
        public async Task<bool> Delete()
        {
            return await SystemMethods.Delete(Brick.Socket, FilePath);
        }

        /// <summary>
        /// Downloads this file from the Brick and returns this file as byte[]
        /// </summary>
        /// <returns>byte[] data of the file</returns>
        public async Task<byte[]> Download()
        {
            return await SystemMethods.DownLoadFile(Brick.Socket, FilePath);
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


        /// <summary>
        /// Downloads a file to local machine
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
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
