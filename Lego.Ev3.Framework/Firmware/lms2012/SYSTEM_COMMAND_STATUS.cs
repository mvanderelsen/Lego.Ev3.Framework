
namespace Lego.Ev3.Framework.Firmware
{
    /// <summary>
    /// System command status
    /// </summary>
    internal enum SYSTEM_COMMAND_STATUS
    {
        /// <summary>
        /// Success
        /// </summary>
        SUCCESS = 0x00,
        /// <summary>
        /// Unknown handle
        /// </summary>
        UNKNOWN_HANDLE = 0x01,
        /// <summary>
        /// Handle not ready
        /// </summary>
        HANDLE_NOT_READY = 0x02,
        /// <summary>
        /// Corrupt file
        /// </summary>
        CORRUPT_FILE = 0x03,
        /// <summary>
        /// No handles available
        /// </summary>
        NO_HANDLES_AVAILABLE = 0x04,
        /// <summary>
        /// No permission
        /// </summary>
        NO_PERMISSION = 0x05,
        /// <summary>
        /// Illegal path
        /// </summary>
        ILLEGAL_PATH = 0x06,
        /// <summary>
        /// File exists
        /// </summary>
        FILE_EXISTS = 0x07,
        /// <summary>
        /// end of file EOF
        /// </summary>
        END_OF_FILE = 0x08,
        /// <summary>
        /// Size error
        /// </summary>
        SIZE_ERROR = 0x09,
        /// <summary>
        /// Unknown error
        /// </summary>
        UNKNOWN_ERROR = 0x0A,
        /// <summary>
        /// Illegal filename 
        /// </summary>
        ILLEGAL_FILENAME = 0x0B,
        /// <summary>
        /// Illegal connection
        /// </summary>
        ILLEGAL_CONNECTION = 0x0C
    }
}
