using Lego.Ev3.Framework.Core;

namespace Lego.Ev3.Framework
{

    /// <summary>
    /// LEGO® MINDSTORMS® EV3 Brick Drive
    /// Projects folder is exposed on this drive
    /// </summary>
    public class Drive : FileSystem
    {

        internal Drive():base(FileExplorer.PROJECTS_PATH) {}
    }
}
