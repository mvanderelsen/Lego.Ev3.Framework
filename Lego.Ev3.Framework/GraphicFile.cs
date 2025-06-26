using Lego.Ev3.Framework.Core;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using I = System.IO;
namespace Lego.Ev3.Framework
{
    /// <summary>
    /// A graphic file that can be rendered in de the display port
    /// </summary>
    public class GraphicFile : FileHandle
    {
        internal GraphicFile(string fileName, string filePath) :base(fileName,filePath){}


        //TODO!!

        ///// <summary>
        ///// Downloads this graphicfile from the Brick and returns this file as Png byte[]
        ///// </summary>
        ///// <returns>byte[] data of the file</returns>
        //public async Task<byte[]> DownloadAsPng()
        //{
        //    byte[] rgf = await Download();
        //    byte[] data;
        //    using (Bitmap bitMap = FileConverter.RGFtoBitmap(rgf))
        //    {
        //        using (I.MemoryStream stream = new I.MemoryStream())
        //        {
        //            bitMap.Save(stream, ImageFormat.Png);
        //            data = stream.ToArray();
        //        }
        //    }
        //    return data;
        //}


        ///// <summary>
        ///// Downloads this graphicfile from the Brick as png
        ///// </summary>
        ///// <returns></returns>
        //public async Task DownloadAsPng(string path)
        //{
        //    byte[] png = await DownloadAsPng();
        //    string fileName = $"{I.Path.GetFileNameWithoutExtension(FileName)}.png";
        //    await Download(path, fileName, png);
        //}
    }
}
