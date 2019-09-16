using Lego.Ev3.Framework.Firmware;
using System.Threading.Tasks;
namespace Lego.Ev3.Framework
{
    /// <summary>
    /// Gets all information about LEGO® MINDSTORMS® EV3 Brick
    /// </summary>
    public sealed class BrickInfo
    {
        /// <summary>
        /// Firmware information
        /// </summary>
        public FirmwareInfo Firmware { get; private set; }

        /// <summary>
        /// Operating system information
        /// </summary>
        public OSInfo OS { get; private set; }

        /// <summary>
        /// Hardware information
        /// </summary>
        public HardwareInfo Hardware { get; private set; }

        /// <summary>
        /// Version string
        /// </summary>
        public string Version { get; private set; }

        /// <summary>
        /// IP if assigned
        /// </summary>
        public string IP { get; private set; }

        /// <summary>
        /// Memory informatie total and free Kb
        /// </summary>
        public MemoryInfo Memory { get; private set; }

        /// <summary>
        /// Connection type
        /// </summary>
        public string Connection { get; private set; }

        internal BrickInfo() { }


        public override string ToString()
        {
            return $"{OS} {Firmware} {Hardware} {Memory} Version:{Version} Ip:{IP} Connection:{Connection}";
        }

        internal static async Task<BrickInfo> GetBrickInfo()
        {
            int[] memory = await MemoryMethods.MemoryUsage(Brick.Socket);

            BrickInfo info = new BrickInfo
            {
                Firmware = new FirmwareInfo
                {
                    Version = await UIReadMethods.GetFirmwareVersion(Brick.Socket),
                    Build = await UIReadMethods.GetFirmwareBuild(Brick.Socket)
                },


                OS = new OSInfo
                {
                    Version = await UIReadMethods.GetOSVersion(Brick.Socket),
                    Build = await UIReadMethods.GetOSBuild(Brick.Socket)
                },


                Hardware = new HardwareInfo
                {
                    Version = await UIReadMethods.GetHardwareVersion(Brick.Socket)
                },

                Version = await UIReadMethods.GetVersion(Brick.Socket),
                IP = await UIReadMethods.GetIP(Brick.Socket),



                Memory = new MemoryInfo
                {
                    Total = memory[0],
                    Free = memory[1]
                },

                Connection = Brick.Socket.ConnectionInfo
            };
            return info;

        }

    }

    public class FirmwareInfo
    {
        public string Version { get; internal set; }

        public string Build { get; internal set; }

        internal FirmwareInfo() { }

        public override string ToString()
        {
            return $"Firmware Version:{Version} Build:{Build}";
        }
    }

    public class OSInfo
    {
        public string Version { get; internal set; }

        public string Build { get; internal set; }

        internal OSInfo() { }

        public override string ToString()
        {
            return $"OS Version:{Version} Build:{Build}";
        }
    }

    public class HardwareInfo
    {
        public string Version { get; internal set; }

        internal HardwareInfo() { }

        public override string ToString()
        {
            return $"Hardware Version:{Version}";
        }
    }

    public class MemoryInfo
    {
        /// <summary>
        /// Free memory in Kb
        /// </summary>
        public int Free { get; internal set; }

        /// <summary>
        /// Total memory in Kb
        /// </summary>
        public int Total { get; internal set; }

        internal MemoryInfo() { }

        public override string ToString()
        {
            return $"Memory Free:{File.FileSize(Free * 1024)} Total:{File.FileSize(Total*1024)}";
        }
    }
}
