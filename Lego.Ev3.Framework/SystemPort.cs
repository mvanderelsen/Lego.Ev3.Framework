using Lego.Ev3.Framework.Firmware;
using Lego.Ev3.Framework.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework
{
    /// <summary>
    /// A port to advanced brick functions
    /// </summary>
    public class SystemPort
    {

        internal SystemPort() 
        {
        }


        /// <summary>
        /// Read information about external device
        /// </summary>
        /// <param name="portNumber">Port number [0-31]</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <returns>Format</returns>
        public async Task<Format> GetFormat(int portNumber)
        {
            if (portNumber < 0 || portNumber > 31) throw new ArgumentOutOfRangeException(nameof(portNumber), "value must be [0-31]");
            return await InputMethods.GetFormat(Brick.Socket, portNumber);
        }

        /// <summary>
        /// Scans all possible ports (all chainlayers) for connected devices
        /// </summary>
        /// <returns>A list of devices with absolute chained portnumbers and devices.</returns>
        public async Task<IEnumerable<PortInfo>> PortScan()
        {
            return await InputMethods.PortScan(Brick.Socket);
        }

        /// <summary>
        /// Gets information about the brick: firmware, hardware, OS etc.
        /// </summary>
        /// <returns></returns>
        public async Task<BrickInfo> GetBrickInfo()
        {
            return await BrickInfo.GetBrickInfo();
        }
    }
}
