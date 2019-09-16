using System;

namespace Lego.Ev3.Framework.Core
{
    /// <summary>
    /// Data Type specifies byte length of returning data
    /// </summary>
    public enum DataType
    {
        /// <summary>
        /// No return
        /// </summary>
        NONE = -1,
        /// <summary>
        /// 1 byte
        /// </summary>
        DATA8 = 0,
        /// <summary>
        /// 2 bytes short
        /// </summary>
        DATA16 = 1,
        /// <summary>
        /// 4 bytes int
        /// </summary>
        DATA32 = 2,
        /// <summary>
        /// 4 bytes float (IEEE 754 single precision – 32 bits)
        /// </summary>
        DATAF = 3,
        /// <summary>
        /// 4 bytes as array
        /// </summary>
        DATA_A4 = 4,
    }

    internal static class DataTypeExtension
    {
        internal static ushort ByteLength(this DataType type)
        {
            switch (type)
            {
                case DataType.DATA8:
                    {
                        return 1;
                    }
                case DataType.DATA16:
                    {
                        return 2;
                    }
                case DataType.DATA32:
                    {
                        return 4;
                    }
                case DataType.DATAF:
                    {
                        return 4;
                    }
                case DataType.DATA_A4:
                    {
                        return 4;
                    }
                case DataType.NONE:
                    {
                        return 0;
                    }
                default:
                    {
                        throw new NotImplementedException("Data Type " + type + " must be implemented!");
                    }
            }
        }
    }
}
