using Lego.Ev3.Framework.Core;

namespace Lego.Ev3.Framework
{
    /// <summary>
    /// A SDCard
    /// </summary>
    public sealed class SDCard : FileSystem
    {

        internal SDCard() : base(BrickExplorer.SDCARD_PATH) { }
       
    }
}
