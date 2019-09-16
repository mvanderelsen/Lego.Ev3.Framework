using Lego.Ev3.Framework.Firmware;
using Lego.Ev3.Framework.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework
{
    /// <summary>
    /// All Input ports on all layers/bricks
    /// </summary>
    internal class Input
    {
        /// <summary>
        /// List of all Input ports with port number index.
        /// </summary>
        public Dictionary<int, InputPort> Ports { get; private set; }

        public Input() 
        {
            //maximum of 16 ports 
            Ports = new Dictionary<int, InputPort>(16);
        }

        internal void AddPorts(IOPorts ports)
        {
            Ports.Add(ports.InputPort.One.Number, ports.InputPort.One);
            Ports.Add(ports.InputPort.Two.Number, ports.InputPort.Two);
            Ports.Add(ports.InputPort.Three.Number, ports.InputPort.Three);
            Ports.Add(ports.InputPort.Four.Number, ports.InputPort.Four);
        }

        #region Firmware Methods

        /// <summary>
        /// Apply new minimum and maximum raw value for device type to be used in scaling PCT and SI value
        /// </summary>
        /// <param name="type">Device type</param>
        /// <param name="mode">Device mode [0-7]</param>
        /// <param name="minimum">32 bit raw minimum value (Zero point)</param>
        /// <param name="maximum">32 bit raw maximum value (Full scale)</param>
        public async Task SetMinMax(DeviceType type, DeviceMode mode, int minimum, int maximum)
        {
            await InputMethods.SetMinMax(Brick.Socket, type, mode, minimum, maximum);
        }

        /// <summary>
        /// Apply the default minimum and maximum raw value for device type to be used in scaling PCT and SI value
        /// </summary>
        /// <param name="type">Device type</param>
        /// <param name="mode">Device mode [0-7]</param>
        public async Task SetDefaultMinMax(DeviceType type, DeviceMode mode)
        {
            await InputMethods.SetDefaultMinMax(Brick.Socket, type, mode);
        }


        /// <summary>
        /// Clear all device counters and values
        /// </summary>
        /// <returns></returns>
        public async Task Reset()
        {
            await InputMethods.Reset(Brick.Socket);
        }

        /// <summary>
        /// Stops all input devices
        /// </summary>
        public async Task Stop()
        {
            await InputMethods.Stop(Brick.Socket);
        }
        #endregion

    }
}
