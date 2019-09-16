namespace Lego.Ev3.Framework.Core
{
    /// <summary>
    /// The Connection Type
    /// </summary>
    public enum ConnectionType
    {
        /// <summary>
        /// Fake
        /// </summary>
        CONN_UNKNOW = 0x6F,
        /// <summary>
        /// Daisy chained
        /// </summary>
        CONN_DAISYCHAIN = 0x75,
        /// <summary>
        /// NXT Color sensor
        /// </summary>
        CONN_NXT_COLOR = 0x76,
        /// <summary>
        /// NXT analog sensor
        /// </summary>
        CONN_NXT_DUMB = 0x77,
        /// <summary>
        /// NXT IIC sensor
        /// </summary>
        CONN_NXT_IIC = 0x78,
        /// <summary>
        /// EV3 input device with ID resistor
        /// </summary>
        CONN_INPUT_DUMB = 0x79,
        /// <summary>
        /// EV3 input UART sensor
        /// </summary>
        CONN_INPUT_UART = 0x7A,
        /// <summary>
        /// EV3 output device with ID resistor
        /// </summary>
        CONN_OUTPUT_DUMB = 0x7B,
        /// <summary>
        /// EV3 output device with communication
        /// </summary>
        CONN_OUTPUT_INTELLIGENT = 0x7C,
        /// <summary>
        /// EV3 Tacho motor with ID resistor
        /// </summary>
        CONN_OUTPUT_TACHO = 0x7D,
        /// <summary>
        /// Port empty or not available
        /// </summary>
        CONN_NONE = 0x7E,
        /// <summary>
        /// Port not empty and type is invalid
        /// </summary>
        CONN_ERROR = 0x7F,
    }
}
