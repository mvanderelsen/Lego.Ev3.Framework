using Lego.Ev3.Framework.Devices;
using Lego.Ev3.Framework.Core;

namespace Lego.Ev3.Framework
{

    /// <summary>
    /// A fixed partition on the Brick drive : Tools, Applications, Projects
    /// </summary>
    public class Partition : FileSystem
    {
        internal Partition(FileSystemPath drive) : base(drive) { }
    }
}
