namespace Lego.Ev3.Framework
{
    /// <summary>
    /// All Input and Output Ports on a brick
    /// </summary>
    public class IOPorts
    {
        /// <summary>
        /// The Layer
        /// </summary>
        public ChainLayer Layer { get; private set; }

        /// <summary>
        /// The Output Ports
        /// </summary>
        public OutputPorts OutputPort { get; private set; }

        /// <summary>
        /// The Input Ports
        /// </summary>
        public InputPorts InputPort { get; private set; }

        /// <summary>
        /// Constructs a ports 
        /// </summary>
        /// <param name="layer">The brick layer</param>
        internal IOPorts(ChainLayer layer) 
        {
            Layer = layer;
            OutputPort = new OutputPorts(layer);
            InputPort = new InputPorts(layer);
        }
    }
}
