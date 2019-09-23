using Lego.Ev3.Framework.Core;
using System;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework
{
    /// <summary>
    /// A Ev3 Robot File. Can be explicitly cast if proper FileType to SoundFile, GraphicFile, TextFile
    /// </summary>
    public class File
    {
        /// <summary>
        /// Gets or sets an unique Id to identify file
        /// Default set to fileName without extension
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Full name of the file including extension
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Size of the file in bytes
        /// </summary>
        public int Size { get; }

        /// <summary>
        /// MD5SUM
        /// </summary>
        public string MD5SUM { get; }

        /// <summary>
        /// The type of the file
        /// </summary>
        public FileType Type { get; }

        /// <summary>
        /// Relative path to the file on the brick
        /// </summary>
        public string Path { get; }

        internal File(string directoryPath, string fileName, string md5sum, int size)
        {
            Id = System.IO.Path.GetFileNameWithoutExtension(fileName);
            Name = fileName;
            Path = $"{directoryPath}{fileName}";
            MD5SUM = md5sum;
            Size = size;
            Type = Firmware.FileSystemMethods.GetFileType(fileName);
        }

        /// <summary>
        /// Returns FileSize to formatted string e.g. 18 KB
        /// </summary>
        public string FileSize()
        {
            return FileSize(Size);
        }


        internal static string FileSize(int byteLength)
        {
            if (byteLength == 0) return "0 KB";
            if (byteLength > 1048576) // 1024 * 1024
            {

                double mb = Math.Ceiling(byteLength / 1048576d);
                mb = Math.Round(mb, 1);
                return $"{mb:0.0} MB";

            }
            else if (byteLength > 1024)
            {
                int kb = (int)Math.Ceiling(byteLength / 1024d);
                return $"{kb} KB";
            }

            return $"{byteLength} bytes";
        }


        /// <summary>
        /// Deletes this lego robot file
        /// Use with care!!
        /// </summary>
        public async Task<bool> Delete()
        {
            return await FileExplorer.DeleteFile(Path);
        }

        /// <summary>
        /// Downloads this file from the Brick and returns this file as byte[]
        /// </summary>
        /// <returns>byte[] data of the file</returns>
        public async Task<byte[]> Download()
        {
            return await FileExplorer.DownloadFile(Path);
        }


        /// <summary>
        /// Downloads the file to specified directory path. if directory does not exists it creates the directory
        /// </summary>
        /// <param name="localFilePath">path to a directory on the local machine</param>
        /// <returns></returns>
        public async Task Download(string localFilePath)
        {
            byte[] data = await Download();
            await Download(localFilePath, Name, data);
        }


        protected async Task Download(string localFilePath, string fileName, byte[] data)
        {
            if (string.IsNullOrWhiteSpace(localFilePath)) throw new ArgumentNullException(nameof(localFilePath));
            localFilePath = System.IO.Path.GetDirectoryName(localFilePath);
            if (!System.IO.Directory.Exists(localFilePath)) System.IO.Directory.CreateDirectory(localFilePath);
            localFilePath = $"{localFilePath}{fileName}";
            using (System.IO.FileStream fileStream = System.IO.File.Create(localFilePath))
            {
               if(data != null) await fileStream.WriteAsync(data, 0, data.Length);
            }
        }


        #region operators
        /// <summary>
        /// Explicit cast to soundfile
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static explicit operator SoundFile(File file)
        {
            if (file == null || file.Type != FileType.SoundFile) throw new InvalidCastException("File is not a soundfile or null");
            return new SoundFile(file.Id,file.Name,file.Path);
        }

        /// <summary>
        /// Explicit cast to graphicfile
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static explicit operator GraphicFile(File file)
        {
            if (file == null || file.Type != FileType.GraphicFile) throw new InvalidCastException("File is not a graphicfile or null");
            return new GraphicFile(file.Id, file.Name, file.Path);
        }

        /// <summary>
        /// Explicit cast to textfile
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static explicit operator TextFile(File file)
        {
            if (file == null || file.Type != FileType.TextFile) throw new InvalidCastException("File is not a textfile or null");
            return new TextFile(file.Id, file.Name, file.Path);
        }
        #endregion

    }
}
