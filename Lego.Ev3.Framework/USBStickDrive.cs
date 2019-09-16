using Lego.Ev3.Framework.Firmware;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework
{
    /// <summary>
    /// LEGO® MINDSTORMS® EV3 USB drive
    /// </summary>
    public sealed class USBStickDrive
    {

        /// <summary>
        /// State of the drive either OK or Empty if no device is present
        /// </summary>
        public DriveState State { get; internal set; }


        /// <summary>
        /// If drivestate = OK gets USBStick otherwise null
        /// </summary>
        public USBStick USBStick { get; internal set; }


        internal USBStickDrive()
        {
         
        }


        /// <summary>
        /// Refreshes all battery information
        /// </summary>
        /// <returns></returns>
        public async Task Refresh()
        {
            USBStickDrive drive = await UIReadMethods.GetUSBDrive(Brick.Socket);
            if (drive != null)
            {
                State = drive.State;
                USBStick = drive.USBStick;
            }
        }

    }

}
