using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework.Firmware
{




    /// <summary>
    /// Parameter Format
    /// </summary>
    /// <remarks>
    ///<code>
    /** LEGO® MINDSTORMS® EV3 Firmware Developer Kit
    * 
    * Parameter encoding
    * Parameter types and values for primitives, system calls and subroutine calls are encoded in the callers byte code stream as follows:
    * opAdd8 (ParCode1, ParCode2, ParCodeN)
    * Bits:
    * 
    *  7 6 5 4 3 2 1 0
    *  0 x x x x x x x     Short format
    *  . 0 x x x x x x     Constant
    *  . . 0 v v v v v     Positive value values range from 0-31
    *  . . 1 v v v v v     Negative value values range from 0-31
    *  . 1 x x x x x x     Variable
    *  . . 0 i i i i i     Local index
    *  . . 1 i i i i i     Global index
    *  
    *  1 t t t - b b b     Long format
    *  . 0 . .   . . .     Constant
    *  . . 0 .   . . .     Value
    *  . . 1 .   . . .     Label
    *  . 1 . .   . . .     Variable
    *  . . 0 .   . . .     Local
    *  . . 1 .   . . .     Global
    *  . . . 0   . . .     Value
    *  . . . 1   . . .     Handle
    *  . . . .   0 0 0     Zero terminated string 
    *  . . . .   0 0 1     1 byte to follow
    *  . . . .   0 1 0     2 bytes to follow
    *  . . . .   0 1 1     4 bytes to follow
    *  . . . .   1 0 0     Zero terminated string 
    *  
    * 
    * For subroutine parameters additional information about number of parameters, direction and size are needed and placed in front of the called code.
    * Parameters MUST be sorted as follows: Largest (4 Bytes) parameters is first and smallets (1 Byte) is last in the list.
    * OffsetToInstructions => NoOfPars, ParType1, ParType2, ParTypeN
    * Bits:
    * 7 6 5 4 3 2 1 0
    * i o - - - b b b  Long format
    * 1 x              Parameter in
    * x 1              Parameter out
    * . . - - - 0 0 0  8 bits
    * . . - - - 0 0 1  16 bits
    * . . - - - 0 1 0  32 bits
    * . . - - - 0 1 1  Float
    * . . - - - 1 0 0  Zero terminated string (Next byte tells allocated size)
    * 
    * 
    */
    ///</code>
    /// </remarks>
    [Flags]
    internal enum PARAMETER_FORMAT : byte
    {
        SHORT = 0x00,
        LONG = 0x80,
    }

    [Flags]
    internal enum PARAMETER_TYPE : byte
    {
        CONSTANT = 0x00,
        VARIABLE = 0x80,
    }

    [Flags]
    internal enum PARAMETER_SHORT_SIGN : byte
    {
        POSITIVE = 0x00,
        NEGATIVE = 0x20,
    }

    [Flags]
    internal enum PARAMETER_LONG_CONSTANT_TYPE : byte
    {
        VALUE = 0x00,
        LABEL = 0x20,
    }

    [Flags]
    internal enum PARAMETER_VARIABLE_SCOPE : byte
    {
        LOCAL = 0x00,
        GLOBAL = 0x20,
    }

    [Flags]
    internal enum PARAMETER_LONG_VARIABLE_TYPE : byte
    {
        VALUE = 0x00,
        HANDLE = 0x10,
    }

    [Flags]
    internal enum PARAMETER_LONG_FOLLOW : byte
    {
        ZERO_TERMINATED_STRING_00 = 0x00,
        ONE_BYTE = 0x01,
        TWO_BYTES = 0x02,
        FOUR_BYTES = 0x03,
        ZERO_TERMINATED_STRING = 0x04,
    }


    [Flags]
    internal enum SUB_PARAMETER_FORMAT : byte
    {
        IN = 0x80,
        OUT = 0x01,
    }

    [Flags]
    internal enum SUB_PARAMETER_LONG_FOLLOW : byte
    {
        ONE_BYTE = 0x00,
        TWO_BYTES = 0x01,
        FOUR_BYTES = 0x02,
        FLOAT = 0x03,
        ZERO_TERMINATED_STRING = 0x04,
    }


    /// <summary>
    /// Combined Parameter VALUE: PARAMETER_TYPE.CONSTANT 
    /// </summary>
    internal enum PAR
    {
        /// <summary>
        /// DATA8  parameter
        /// PARAMETER_FORMAT.LONG | PARAMETER_TYPE.CONSTANT | PARAMETER_LONG_CONSTANT_TYPE.VALUE | PARAMETER_LONG_FOLLOW.ONE_BYTE
        /// </summary>
        PAR8 = 0x81,
        /// <summary>
        /// DATA16  parameter
        /// PARAMETER_FORMAT.LONG | PARAMETER_TYPE.CONSTANT | PARAMETER_LONG_CONSTANT_TYPE.VALUE | PARAMETER_LONG_FOLLOW.TWO_BYTES
        /// </summary>
        PAR16 = 0x82,
        /// <summary>
        /// DATA32  parameter
        /// PARAMETER_FORMAT.LONG | PARAMETER_TYPE.CONSTANT | PARAMETER_LONG_CONSTANT_TYPE.VALUE | PARAMETER_LONG_FOLLOW.FOUR_BYTES
        /// </summary>
        PAR32 = 0x83,
        /// <summary>
        /// DATAS parameter
        /// PARAMETER_FORMAT.LONG | PARAMETER_TYPE.CONSTANT | PARAMETER_LONG_CONSTANT_TYPE.VALUE | PARAMETER_LONG_FOLLOW.ZERO_TERMINATED_STRING
        /// </summary>
        PARS = 0x84,
        /// <summary>
        /// Short positive (0-31)
        /// PARAMETER_FORMAT.SHORT | PARAMETER_TYPE.CONSTANT | PARAMETER_SHORT_SIGN.POSITIVE
        /// </summary>
        POS = 0x00,
        /// <summary>
        /// Short negative (0-31)
        /// PARAMETER_FORMAT.SHORT | PARAMETER_TYPE.CONSTANT | PARAMETER_SHORT_SIGN.NEGATIVE
        /// </summary>
        NEG = 0x20
    }

    
}
