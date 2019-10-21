using Lego.Ev3.Framework.Core;
using Lego.Ev3.Framework.Firmware;
using Lego.Ev3.Framework.Internals;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Lego.Ev3.Framework
{
    /// <summary>
    /// A console to advanced brick functions
    /// </summary>
    public sealed class BrickConsole
    {
        /// <summary>
        /// Delegate for warnings received from brick
        /// </summary>
        /// <param name="value">list of warnings</param>
        public delegate void OnWarningReceived(IEnumerable<Warning> warnings);
        /// <summary>
        /// Warnings received event
        /// </summary>
        public event OnWarningReceived WarningReceived;

        public bool MonitorEvents { get; set; }

        private Warning Value { get; set; }

        internal BrickConsole()
        {
            MonitorEvents = true;
        }

        internal ushort BatchCommand(PayLoadBuilder payLoadBuilder, int index)
        {
            if (!MonitorEvents || WarningReceived == null) return 0; // no need to poll data
            return UIReadMethods.GetWarning_BatchCommand(payLoadBuilder, index);
        }

        internal bool SetValue(byte data)
        {
            Warning newValue = (Warning)data;

            bool hasChanged = (Value != newValue);
            if (hasChanged)
            {
                Value = newValue;
                if (Value != Warning.None)
                {
                    IEnumerable<Warning> warnings = Value.GetFlags();
                    if (MonitorEvents) WarningReceived?.Invoke(warnings);
                    foreach (Warning warning in warnings)
                    {
                        Brick.Logger.LogWarning($"Brick warning: {warning}");
                    }
                }
            }
            return hasChanged;
        }




        /// <summary>
        /// Gets the actual brick name
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetBrickName()
        {
            byte[] data = await BrickExplorer.DownloadFile("../sys/settings/BrickName");
            string name = Encoding.ASCII.GetString(data);
            return name.TrimEnd('\0', '\n');
        }


        /// <summary>
        /// Gets any warnings
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Warning>> GetWarnings()
        {
            return await UIReadMethods.GetWarnings(Brick.Socket);
        }


        /// <summary>
        /// Gets the brick IP address if assigned else string.Empty
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetIPAddress()
        {
            return await UIReadMethods.GetIPAddress(Brick.Socket);
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
            return await InputMethods.GetFormat(Brick.Socket, layer, portNumber);
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
            return await InputMethods.GetFormat(Brick.Socket, layer, portNumber);
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
