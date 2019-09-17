namespace Lego.Ev3.Framework
{
    /// <summary>
    /// All input ports on a brick
    /// </summary>
    public sealed class InputPorts
    {
        /// <summary>
        /// The brick layer
        /// </summary>
        internal ChainLayer Layer { get; private set; }

        /// <summary>
        /// Port 1
        /// </summary>
        public InputPort One { get; private set; }

        /// <summary>
        /// Port 2
        /// </summary>
        public InputPort Two { get; private set; }

        /// <summary>
        /// Port 3
        /// </summary>
        public InputPort Three { get; private set; }

        /// <summary>
        /// Port 4
        /// </summary>
        public InputPort Four { get; private set; }

        /// <summary>
        /// Construct all 4 input ports
        /// </summary>
        /// <param name="layer">The brick layer</param>
        internal InputPorts(ChainLayer layer)
        {
            Layer = layer;
            One = new InputPort(layer, InputPortName.One);
            Two = new InputPort(layer, InputPortName.Two);
            Three = new InputPort(layer, InputPortName.Three);
            Four = new InputPort(layer, InputPortName.Four);
        }
    }
}
