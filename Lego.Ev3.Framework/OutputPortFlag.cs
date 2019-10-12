using System;

namespace Lego.Ev3.Framework
{
    /// <summary>
    /// Output port flag
    /// Bitfield: 4bits
    /// 0100 = Port C
    /// 0101 = Port A and Port C
    /// Etc.
    /// </summary>
    [Flags]
    public enum OutputPortFlag
    {
        /// <summary>
        /// Output port A
        /// </summary>
        A = 0x01,
        /// <summary>
        /// Output port B
        /// </summary>
        B = 0x02,
        /// <summary>
        /// Output port C
        /// </summary>
        C = 0x04,
        /// <summary>
        /// Output port D
        /// </summary>
        D = 0x08,
        /// <summary>
        /// Output ports A,B,C and D simultaneously
        /// </summary>
        All = 0x0F
    }
}
