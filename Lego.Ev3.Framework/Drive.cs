using Lego.Ev3.Framework.Firmware;
using Lego.Ev3.Framework.Core;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework
{

    /// <summary>
    /// LEGO® MINDSTORMS® EV3 Brick Drive
    /// Projects folder is exposed on this drive
    /// </summary>
    public class Drive : FileSystem
    {

        internal Drive():base(FileExplorer.PROJECTS_PATH) {}


        /// <summary>
        /// Gets drive information about total and free states.
        /// </summary>
        /// <returns></returns>
        public async Task<DriveInfo> GetDriveInfo()
        {
            int[] values = await MemoryMethods.MemoryUsage(Brick.Socket);
            return new DriveInfo(values);
        }
    }
}
