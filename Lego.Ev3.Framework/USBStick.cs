using Lego.Ev3.Framework.Core;

namespace Lego.Ev3.Framework
{

    /// <summary>
    /// A USB memory stick
    /// </summary>
    public sealed class USBStick : FileSystem
    {
        /// <summary>
        /// USB Stick's total memory size [KB] if drivestate = OK
        /// </summary>
        public int Total { get; internal set; }

        /// <summary>
        /// USB Stick's free or available memory size [KB] if drivestate = OK
        /// </summary>
        public int Free { get; internal set; }

        internal USBStick() : base(FileExplorer.USBSTICK_PATH) { }
    }
}
