using Lego.Ev3.Framework.Core;
using Lego.Ev3.Framework.Firmware;
using Lego.Ev3.Framework.Internals;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework
{
    /// <summary>
    /// A console to advanced brick functions
    /// </summary>
    public sealed class BrickConsole
    {
        internal BrickConsole() {}


        /// <summary>
        /// Gets the brick IP address if assigned else null
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetIpAddress()
        {
            return await UIReadMethods.GetIP(Brick.Socket);
        }

        /// <summary>
        /// Gets the connectionInfo e.g. "Usb", "BlueTooth COM5", "Network 192.168.1.1"
        /// </summary>
        /// <returns></returns>
        public string GetConnnectionInfo()
        {
            return Brick.Socket.ConnectionInfo;
        }

        /// <summary>
        /// Read information about external device
        /// </summary>
        /// <param name="port"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public async Task<Format> GetFormat(InputPortName port, ChainLayer layer = ChainLayer.One)
        {
            int portNumber = port.AbsolutePortNumber(layer);
            return await InputMethods.GetFormat(Brick.Socket, portNumber);
        }

        /// <summary>
        /// Read information about external device
        /// </summary>
        /// <param name="port"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public async Task<Format> GetFormat(OutputPortName port, ChainLayer layer = ChainLayer.One)
        {
            int portNumber = port.AbsolutePortNumber(layer);
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
