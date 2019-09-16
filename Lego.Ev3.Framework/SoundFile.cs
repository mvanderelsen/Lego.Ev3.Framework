using Lego.Ev3.Framework.Core;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework
{
    /// <summary>
    /// A playable soundFile
    /// </summary>
    public class SoundFile : FileHandle
    {


        internal SoundFile(string id, string fileName, string filePath)
            :base(id,fileName,filePath)
        {
        }

       

        /// <summary>
        /// Downloads this soundfile from the Brick and returns this file as WAV byte[]
        /// </summary>
        /// <returns>byte[] data of the file</returns>
        public async Task<byte[]> DownloadAsWAV()
        {
            byte[] rsf = await Download();
            return await FileConverter.RSFtoWAV(rsf);
        }


        /// <summary>
        /// Downloads this soundfile from the Brick as wav
        /// </summary>
        /// <returns></returns>
        public async Task DownloadAsWAV(string path)
        {
            byte[] wav = await DownloadAsWAV();
            string fileName = $"{System.IO.Path.GetFileNameWithoutExtension(FileName)}.wav";
            await Download(path, fileName, wav);
        }

    }
}
