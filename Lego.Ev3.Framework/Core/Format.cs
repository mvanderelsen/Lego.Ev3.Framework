namespace Lego.Ev3.Framework.Core
{
    /// <summary>
    /// Format return type of method opInput_Device CMD: GET_FORMAT
    /// </summary>
    public struct Format
    {
        /// <summary>
        /// Number of data sets
        /// </summary>
        public int NumberOfDataSets { get; }

        /// <summary>
        /// DataType [0-3], (0: 8-bit, 1: 16-bit, 2: 32-bit, 3: Float point)
        /// </summary>
        public DataType DataType { get; }

        /// <summary>
        /// Number of modes [1-8]
        /// </summary>
        public int NumberOfModes { get; }

        /// <summary>
        /// Number of modes visible within port view app [1-8]
        /// </summary>
        public int VisibleNumberOfModes { get; }

        internal Format(int numberOfDataSets, DataType dataType, int numberOfModes, int visibleNumberOfModes)
        {
            NumberOfDataSets = numberOfDataSets;
            DataType = dataType;
            NumberOfModes = numberOfModes;
            VisibleNumberOfModes = visibleNumberOfModes;
        }

    }
}
