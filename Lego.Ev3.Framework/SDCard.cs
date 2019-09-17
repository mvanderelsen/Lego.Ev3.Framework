using Lego.Ev3.Framework.Core;

namespace Lego.Ev3.Framework
{
    /// <summary>
    /// A SDCard
    /// </summary>
    public sealed class SDCard : FileSystem
    {
        /// <summary>
        /// SDCard total memory size [KB] if drivestate = OK
        /// </summary>
        public int Total { get; internal set; }

        /// <summary>
        /// SDCard free or available memory size [KB] if drivestate = OK
        /// </summary>
        public int Free { get; internal set; }

        internal SDCard() : base(FileExplorer.SDCARD_PATH) { }
       
    }
}
