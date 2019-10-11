using System;
using System.IO;
using System.Text;
namespace Lego.Ev3.Framework.Firmware
{
    /* LEGO® MINDSTORMS® EV3 Firmware Developer Kit
     * 
     * Parameter encoding
     * Parameter types and values for primitives, system calls and subroutine calls are encoded in the callers byte code stream as follows:
     * opAdd8 (ParCode1, ParCode2, ParCodeN)
     * Bits:
     * 
     *  7 6 5 4 3 2 1 0
     *  0 x x x x x x x     Short format
     *  . 0 x x x x x x     Constant
     *  . . 0 v v v v v     Positive value values range from 0-63
     *  . . 1 v v v v v     Negative value values range from 0-63
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
    internal class PayLoadBuilder : IDisposable
    {
        private const byte TERMINATOR = 0x1f;
        private readonly BinaryWriter _binaryWriter;
        private readonly MemoryStream _memoryStream;

        public PayLoadBuilder()
        {
            _memoryStream = new MemoryStream();
            _binaryWriter = new BinaryWriter(_memoryStream);
        }

        public void Raw(byte value)
        {
            _binaryWriter.Write(value);
        }

        public void Raw(ushort value)
        {
            _binaryWriter.Write(value);
        }

        public void Raw(int value)
        {
            _binaryWriter.Write(value);
        }

        public void Raw(uint value)
        {
            _binaryWriter.Write(value);
        }

        public void Raw(string value)
        {
            _binaryWriter.Write(Encoding.UTF8.GetBytes(value));
            _binaryWriter.Write((byte)PARAMETER_LONG_FOLLOW.ZERO_TERMINATED_STRING_00);
        }

        public void Raw(byte[] bytes)
        {
            _binaryWriter.Write(bytes);
        }

        public void LittleEndian(ushort value)
        {
            _binaryWriter.Write((byte)value);
            _binaryWriter.Write((byte)(value >> 8));
        }


        /// <summary>
        /// Adds value as short
        /// </summary>
        /// <param name="value">Must be between -31 and 31</param>
        public void SHORT(int value)
        {
            if (value < -31 || value > 31) throw new ArgumentOutOfRangeException(nameof(value), "a short value must be between -31 and 31");
            if (value < 0)
            {
                _binaryWriter.Write((byte)((byte)PAR.NEG | (byte)(value & TERMINATOR)));
            }
            else _binaryWriter.Write((byte)((byte)PAR.POS | (byte)(value & TERMINATOR)));
        }

        /// <summary>
        /// 8 bit value
        /// </summary>
        /// <param name="value"></param>
        public void PAR8(byte value)
        {
            _binaryWriter.Write((byte)PAR.PAR8);
            _binaryWriter.Write(value);
        }

        /// <summary>
        /// 8 bit value
        /// </summary>
        /// <param name="value"></param>
        public void PAR8(ushort value)
        {
            _binaryWriter.Write((byte)PAR.PAR8);
            _binaryWriter.Write((byte)value);
        }

        /// <summary>
        /// 8 bit value
        /// </summary>
        /// <param name="value"></param>
        public void PAR8(int value)
        {
            _binaryWriter.Write((byte)PAR.PAR8);
            _binaryWriter.Write((byte)value);
        }

        /// <summary>
        /// 16 bit  value
        /// </summary>
        /// <param name="value"></param>
        public void PAR16(ushort value)
        {
            _binaryWriter.Write((byte)PAR.PAR16);
            _binaryWriter.Write(value);
        }

        /// <summary>
        /// 16 bit  value
        /// </summary>
        /// <param name="value"></param>
        public void PAR16(Int32 value)
        {
            _binaryWriter.Write((byte)PAR.PAR16);
            _binaryWriter.Write((short)value);
        }

        /// <summary>
        /// 32 bit value
        /// </summary>
        /// <param name="value"></param>
        public void PAR32(int value)
        {
            _binaryWriter.Write((byte)PAR.PAR32);
            _binaryWriter.Write(value);
        }

        /// <summary>
        /// 32 bit value
        /// </summary>
        /// <param name="value"></param>
        public void PAR32(uint value)
        {
            _binaryWriter.Write((byte)PAR.PAR32);
            _binaryWriter.Write(value);
        }

        /// <summary>
        /// String value
        /// </summary>
        /// <param name="value"></param>
        public void PARS(string value)
        {
            _binaryWriter.Write((byte)PAR.PARS);
            _binaryWriter.Write(Encoding.UTF8.GetBytes(value));
            _binaryWriter.Write((byte)PARAMETER_LONG_FOLLOW.ZERO_TERMINATED_STRING_00);
        }

        public void GlobalIndex(int index)
        {
            if (index > 1024) throw new ArgumentException("Index cannot be greater than 1024", nameof(index));
            _binaryWriter.Write((byte)0xE1);
            _binaryWriter.Write((byte)index);
        }

        public void VARIABLE_SHORT(byte value, PARAMETER_VARIABLE_SCOPE scope)
        {
            byte b = (byte)((byte)PARAMETER_FORMAT.SHORT | (byte)PARAMETER_TYPE.VARIABLE | (byte)scope | (byte)(value & TERMINATOR));
            _binaryWriter.Write(b);
            //_binaryWriter.Write(value);
        }

        public void VARIABLE_PAR8(byte value, PARAMETER_VARIABLE_SCOPE scope)
        {
            byte b = (byte)((byte)PARAMETER_FORMAT.LONG | (byte)PARAMETER_TYPE.VARIABLE | (byte)scope | (byte)PARAMETER_LONG_VARIABLE_TYPE.VALUE | (byte)PARAMETER_LONG_FOLLOW.ONE_BYTE);
            _binaryWriter.Write(b);
            _binaryWriter.Write(value);
        }

        public void VARIABLE_PAR32(int value, PARAMETER_VARIABLE_SCOPE scope)
        {
            byte b = (byte)((byte)PARAMETER_FORMAT.LONG | (byte)PARAMETER_TYPE.VARIABLE | (byte)scope | (byte)PARAMETER_LONG_VARIABLE_TYPE.VALUE | (byte)PARAMETER_LONG_FOLLOW.FOUR_BYTES);
            _binaryWriter.Write(b);
            _binaryWriter.Write(value);
        }

        public byte[] ToBytes()
        {
            return _memoryStream.ToArray();
        }


        private bool disposed = false;


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (_binaryWriter != null)
                    {
                        _binaryWriter.Dispose();
                    }
                    if (_memoryStream != null)
                    {
                        _memoryStream.Dispose();
                    }
                }
            }
            disposed = true;
        }
    }
}
