using System;

namespace Lego.Ev3.Framework
{
    /// <summary>
    /// Brick warnings
    /// </summary>
    [Flags]
    public enum Warning
    {
        /// <summary>
        /// No Warning
        /// </summary>
        None = 0x00,
        /// <summary>
        /// High temperature 
        /// </summary>
        Temperature = 0x01,
        /// <summary>
        /// High current
        /// </summary>
        Current = 0x02,
        /// <summary>
        /// High voltage
        /// </summary>
        Voltage =0x04,
        /// <summary>
        /// High memory usage
        /// </summary>
        Memory =0x08,
        /// <summary>
        /// High Dsp state
        /// </summary>
        DSPState = 0x10,
        /// <summary>
        /// High ram usage
        /// </summary>
        RAM = 0x20,
        /// <summary>
        /// Low battery level
        /// </summary>
        Battery = 0x40,
        /// <summary>
        /// Busy
        /// </summary>
        Busy = 0x80,
        /// <summary>
        /// All warnings
        /// </summary>
        All = 0x3F
    }
}
