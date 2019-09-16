using System.Threading.Tasks;
using System;
using Lego.Ev3.Framework.Devices;

namespace Lego.Ev3.Framework
{
    /// <summary>
    /// Port to all Input/Output Ports on all bricks in chain
    /// </summary>
    internal class IOPort
    {
        /// <summary>
        /// All input ports on all bricks
        /// </summary>
        public Input Input { get; private set; }

        /// <summary>
        /// All output ports on all bricks
        /// </summary>
        public Output Output { get; private set; }

        public IOPort() 
        {
            Input = new Input();
            Output = new Output();
        }

        public void AddPorts(IOPorts ports)
        {
            Input.AddPorts(ports);
            Output.AddPorts(ports);
        }

        

        /// <summary>
        /// Resets all counters and values
        /// </summary>
        /// <returns></returns>
        public async Task Reset()
        {
            await Task.WhenAll(Output.Reset(), Input.Reset());
        }

        /// <summary>
        /// Stops all devices
        /// </summary>
        public async Task Stop()
        {
            await Task.WhenAll(Output.Stop(), Input.Stop());
        }

        public IOutputDevice FindOutputDevice(string id)
        {
            foreach (int port in Output.Ports.Keys)
            {
                OutputPort outputPort = Output.Ports[port];
                if (outputPort.Device != null && outputPort.Device.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase)) return outputPort.Device;
            }
            return null;
        }

        public IInputDevice FindInputDevice(string id)
        {
            foreach (int port in Input.Ports.Keys)
            {
                InputPort onputPort = Input.Ports[port];
                if (onputPort.Device != null && onputPort.Device.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase)) return onputPort.Device;
            }
            return null;
        }

    }
}
