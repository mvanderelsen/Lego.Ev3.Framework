using Lego.Ev3.Framework.Core;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework
{
    /// <summary>
    /// A textFile
    /// </summary>
    public class TextFile : FileHandle
    {


        internal TextFile(string fileName, string filePath)
            : base(fileName, filePath)
        {
        }



        /// <summary>
        /// Downloads this textfile from the Brick and returns this file as string
        /// </summary>
        /// <returns>byte[] data of the file</returns>
        public async Task<string> DownloadAsString()
        {
            byte[] rsf = await Download();
            return FileConverter.RTFtoText(rsf);
        }


        /// <summary>
        /// Downloads this textfile from the Brick as txt
        /// </summary>
        /// <returns></returns>
        public async Task DownloadAsTxt(string path)
        {
            byte[] txt = await Download();
            string fileName = $"{System.IO.Path.GetFileNameWithoutExtension(FileName)}.txt";
            await Download(path, fileName, txt);
        }

    }
}
