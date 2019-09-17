using Lego.Ev3.Framework.Firmware;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework
{
    /// <summary>
    /// LEGO® MINDSTORMS® EV3 SDCard drive
    /// </summary>
    public sealed class SDCardDrive
    {

        /// <summary>
        /// State of the drive either OK or Empty if no device is present
        /// </summary>
        public DriveState State { get; internal set; }


        /// <summary>
        /// If drivestate = OK gets SDCard otherwise null
        /// </summary>
        public SDCard SDCard { get; internal set; }


        internal SDCardDrive() { }


        /// <summary>
        /// Refreshes sd card drive
        /// </summary>
        /// <returns></returns>
        public async Task Refresh()
        {
            SDCardDrive drive = await UIReadMethods.GetSDCardDrive(Brick.Socket);
            if (drive != null)
            {
                State = drive.State;
                SDCard = drive.SDCard;
            }
        }

    }
}
