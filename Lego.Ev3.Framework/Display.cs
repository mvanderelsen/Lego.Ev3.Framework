using Lego.Ev3.Framework.Firmware;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework
{
    /// <summary>
    /// Port to all graphic methods
    /// Methods can only be called after Brick is connected
    /// </summary>
    /// <example>
    /// <code>
    /// The user interface library contains different functionality used to support a graphical user interface.
    /// Graphic files as fonts, icons and pictures are ".xbm" files containing monochrome bitmap that can also
    /// be opened with a normal text editor. These files are coming from the LINUX world.
    /// 
    /// 
    ///     Fonts:      must be X axis aligned on 8 pixel boundaries (build into the firmware)
    /// 
    ///                 Normal font   8  x 9    (W x H)         Used in browser
    ///                 Small font    8  x 8    (W x H)         Used on display top line
    ///                 Large font    16 x 19   (W x H)         Used in numbers
    /// 
    /// 
    ///     Icons:      must be X axis aligned on 8 pixel boundaries (build into the firmware)
    /// 
    ///                 Normal icon   16 x 12   (W x H)         Used in browser
    ///                 Small icon    16 x 8    (W x H)         Used on display top line
    ///                 Large icon    24 x 24   (W x H)         Used as keys
    /// 
    /// 
    ///     Pictures:   no restrictions (files in the file system)
    /// 
    ///                 Smallest       1 x 1    (W x H)
    ///                 Largest      178 x 128  (W x H)
    /// 
    /// </code>
    /// </example>
    public sealed class Display
    {
        /// <summary>
        /// Width of the lcd display in pixels
        /// </summary>
        public const int WIDTH = 178;

        /// <summary>
        /// Height of the lcd display in pixels
        /// </summary>
        public const int HEIGHT = 128;

        internal Display() { }


        /// <summary>
        /// Enable displaying a graphic file.
        /// </summary>
        /// <param name="file">A graphic file</param>
        /// <param name="x">Specify X start point, [0 - 177]</param>
        /// <param name="y">Specify Y start point, [0 - 127]</param>
        /// <param name="color">Specify either black or white</param>
        public async Task Draw(GraphicFile file, int x = 0, int y = 0, UIColor color = UIColor.Black)
        {
            //TODO check width and height of graphic file in compare with width and height
            await UIDrawMethods.BMPFile(Brick.Socket, file.FilePath, x, y, color);
        }
    }
}
