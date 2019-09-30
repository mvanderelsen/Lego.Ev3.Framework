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


        internal BrickInfo() { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{OS} {Firmware} {Hardware} Version:{Version}";
        }

        internal static async Task<BrickInfo> GetBrickInfo()
        {

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

            };
            return info;

        }


        /// <summary>
        /// Firmware Information
        /// </summary>
        public class FirmwareInfo
        {
            /// <summary>
            /// Gets the version
            /// </summary>
            public string Version { get; internal set; }

            /// <summary>
            /// gets the build
            /// </summary>
            public string Build { get; internal set; }

            internal FirmwareInfo() { }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return $"Firmware Version:{Version} Build:{Build}";
            }
        }


        /// <summary>
        /// Operating System Information
        /// </summary>
        public class OSInfo
        {

            /// <summary>
            /// Gets the version
            /// </summary>
            public string Version { get; internal set; }


            /// <summary>
            /// Gets the build
            /// </summary>
            public string Build { get; internal set; }

            internal OSInfo() { }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return $"OS Version:{Version} Build:{Build}";
            }
        }

        /// <summary>
        /// Hardware information
        /// </summary>
        public class HardwareInfo
        {

            /// <summary>
            /// Gets the version
            /// </summary>
            public string Version { get; internal set; }

            internal HardwareInfo() { }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return $"Hardware Version:{Version}";
            }
        }
    }
}
