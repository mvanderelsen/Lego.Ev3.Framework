using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using I = System.IO;

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
        public string Id { get; set; }

        /// <summary>
        /// Full name of the file including extension
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Size of the file in bytes
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// MD5SUM
        /// </summary>
        public string MD5SUM { get; private set; }

        /// <summary>
        /// The type of the file
        /// </summary>
        public FileType Type { get; private set; }

        /// <summary>
        /// Relative path to the file on the brick
        /// </summary>
        public string Path { get; internal set; }

        /// <summary>
        /// Indicator if file was found through regular FileSystem methods or added from Memory items.
        /// </summary>
        public bool Hidden { get; internal set; }

        internal File(string directoryPath, string fileName, string md5sum, int size)
        {
            Id = I.Path.GetFileNameWithoutExtension(fileName);
            Name = fileName;
            Path = I.Path.Combine(directoryPath, fileName);
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

        internal static async Task<List<File>> GetFiles(string path)
        {
            List<File> files = new List<File>();
            string[] entries = await Firmware.FileSystemMethods.ListFiles(Brick.Socket, path);
            foreach (string entry in entries)
            {
                string item = entry.Trim();
                //skip directories and empty lines
                if (!item.EndsWith("/")  && ! string.IsNullOrWhiteSpace(item))
                {
                    //fileInfo:   32 chars (hex) of MD5SUM + space + 8 chars (hex) of filesize + space + filename
                    string[] fileInfo = entry.Split(' ');
                    if(fileInfo.Length >=  3)
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
            files.Sort(delegate(File obj1, File obj2) { return obj1.Name.CompareTo(obj2.Name); });
            return files;
        }


        internal static async Task<File> GetFile(string path, string fileName)
        {
            fileName = fileName.ToLowerInvariant();
            List<File> files = await GetFiles(path);
            return files.Find(t => t.Name.ToLowerInvariant() == fileName);
        }


        /// <summary>
        /// Deletes this lego robot file
        /// Use with care!!
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task Delete()
        {
            if (Type == FileType.SystemFile || Hidden) return;
            await Firmware.FileSystemMethods.DeleteFile(Brick.Socket, Path);
        }

        /// <summary>
        /// Downloads this file from the Brick and returns this file as byte[]
        /// </summary>
        /// <returns>byte[] data of the file</returns>
        public async Task<byte[]> Download()
        {
            return await Firmware.FileSystemMethods.DownLoadFileFromBrick(Brick.Socket, Path);
        }


        /// <summary>
        /// Downloads the file to specified directory path. if directory does not exists it creates the directory
        /// </summary>
        /// <param name="path">path to a directory</param>
        /// <returns></returns>
        public async Task Download(string path)
        {
            byte[] data = await Download();
            await Download(path, Name, data);
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


        /// <summary>
        /// Test method wether local file is a lego ev3 robot file.
        /// Will test fileName extension only (*.rgf|*.rbf|*.rsf|*.rdf|*.rtf|*.rpf|*.rcf|*.raf)
        /// </summary>
        /// <param name="filePath">Absolute local path or fileName of a lego file on the local machine</param>
        /// <returns></returns>
        public static bool IsRobotFile(string filePath)
        {
            return Firmware.FileSystemMethods.IsRobotFile(filePath);
        }


        /// <summary>
        /// Gets the file type pending on fileName's extension
        /// </summary>
        public static FileType GetType(string path)
        {
            return Firmware.FileSystemMethods.GetFileType(path);
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
