using Lego.Ev3.Framework.Core;
using Lego.Ev3.Framework.Firmware;
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
        /// Full name of the file including extension
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Size of the file in bytes
        /// </summary>
        public long Size { get; }

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

        internal File(string directoryPath, string fileName, string md5sum, long size)
        {
            FileName = fileName;
            Path = $"{directoryPath}{fileName}";
            MD5SUM = md5sum;
            Size = size;
            Type = GetFileType(fileName);
        }

        /// <summary>
        /// Returns FileSize to formatted string e.g. 18 KB
        /// </summary>
        public string FileSize()
        {
            return FileSize(Size);
        }


        internal static string FileSize(long byteLength)
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
            return await BrickExplorer.DeleteFile(Path);
        }

        /// <summary>
        /// Downloads this file from the Brick and returns this file as byte[]
        /// </summary>
        /// <returns>byte[] data of the file</returns>
        public async Task<byte[]> Download()
        {
            return await BrickExplorer.DownloadFile(Path);
        }


        /// <summary>
        /// Downloads the file to specified directory path. if directory does not exists it creates the directory
        /// </summary>
        /// <param name="localFilePath">path to a directory on the local machine</param>
        /// <returns></returns>
        public async Task Download(string localFilePath)
        {
            byte[] data = await Download();
            await Download(localFilePath, FileName, data);
        }


        /// <summary>
        /// Downloads the file to local machine
        /// </summary>
        /// <param name="localFilePath"></param>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
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

        internal static bool IsRobotFile(string filePath)
        {
            string ext = System.IO.Path.GetExtension(filePath).ToLower();
            if (string.IsNullOrWhiteSpace(ext)) return false;
            return (
                ext == FILE_TYPE.vmEXT_ARCHIVE
                || ext == FILE_TYPE.vmEXT_BYTECODE
                || ext == FILE_TYPE.vmEXT_CONFIG
                || ext == FILE_TYPE.vmEXT_DATALOG
                || ext == FILE_TYPE.vmEXT_GRAPHICS
                || ext == FILE_TYPE.vmEXT_PROGRAM
                || ext == FILE_TYPE.vmEXT_SOUND
                || ext == FILE_TYPE.vmEXT_TEXT
                );
        }

        internal static FileType GetFileType(string path)
        {
            string ext = System.IO.Path.GetExtension(path).ToLower();
            if (ext == FILE_TYPE.vmEXT_ARCHIVE) return FileType.ArchiveFile;
            if (ext == FILE_TYPE.vmEXT_BYTECODE) return FileType.ByteCodeFile;
            if (ext == FILE_TYPE.vmEXT_CONFIG) return FileType.ConfigFile;
            if (ext == FILE_TYPE.vmEXT_DATALOG) return FileType.DataLogFile;
            if (ext == FILE_TYPE.vmEXT_GRAPHICS) return FileType.GraphicFile;
            if (ext == FILE_TYPE.vmEXT_PROGRAM) return FileType.ProgramFile;
            if (ext == FILE_TYPE.vmEXT_SOUND) return FileType.SoundFile;
            if (ext == FILE_TYPE.vmEXT_TEXT) return FileType.TextFile;
            return FileType.SystemFile;
        }

        internal static string GetExtension(FileType type)
        {
            switch (type)
            {
                case FileType.ArchiveFile:
                    {
                        return FILE_TYPE.vmEXT_ARCHIVE;
                    }
                case FileType.ByteCodeFile:
                    {
                        return FILE_TYPE.vmEXT_BYTECODE;
                    }
                case FileType.ConfigFile:
                    {
                        return FILE_TYPE.vmEXT_CONFIG;
                    }
                case FileType.DataLogFile:
                    {
                        return FILE_TYPE.vmEXT_DATALOG;
                    }
                case FileType.GraphicFile:
                    {
                        return FILE_TYPE.vmEXT_GRAPHICS;
                    }
                case FileType.ProgramFile:
                    {
                        return FILE_TYPE.vmEXT_PROGRAM;
                    }
                case FileType.SoundFile:
                    {
                        return FILE_TYPE.vmEXT_SOUND;
                    }
                case FileType.TextFile:
                    {
                        return FILE_TYPE.vmEXT_TEXT;
                    }
            }

            return null;
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
            return new SoundFile(file.FileName,file.Path);
        }

        /// <summary>
        /// Explicit cast to graphicfile
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static explicit operator GraphicFile(File file)
        {
            if (file == null || file.Type != FileType.GraphicFile) throw new InvalidCastException("File is not a graphicfile or null");
            return new GraphicFile(file.FileName, file.Path);
        }

        /// <summary>
        /// Explicit cast to textfile
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static explicit operator TextFile(File file)
        {
            if (file == null || file.Type != FileType.TextFile) throw new InvalidCastException("File is not a textfile or null");
            return new TextFile(file.FileName, file.Path);
        }
        #endregion

    }
}
