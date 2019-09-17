namespace Lego.Ev3.Framework
{
    /// <summary>
    /// All output ports on a brick
    /// </summary>
    public sealed class OutputPorts
    {
        /// <summary>
        /// The brick layer
        /// </summary>
        internal ChainLayer Layer { get; private set; }

        /// <summary>
        /// Port A
        /// </summary>
        public OutputPort A { get; private set; }

        /// <summary>
        /// Port B
        /// </summary>
        public OutputPort B { get; private set; }

        /// <summary>
        /// Port C
        /// </summary>
        public OutputPort C { get; private set; }

        /// <summary>
        /// Port D
        /// </summary>
        public OutputPort D { get; private set; }


        /// <summary>
        /// Construct all 4 output ports
        /// </summary>
        /// <param name="layer">The brick layer</param>
        internal OutputPorts(ChainLayer layer)
        {
            Layer = layer;
            A = new OutputPort(layer, OutputPortName.A);
            B = new OutputPort(layer, OutputPortName.B);
            C = new OutputPort(layer, OutputPortName.C);
            D = new OutputPort(layer, OutputPortName.D);
        }

    }
}
