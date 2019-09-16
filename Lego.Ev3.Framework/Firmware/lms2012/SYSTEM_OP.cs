
namespace Lego.Ev3.Framework.Firmware
{
    /// <summary>
    /// System Opcode
    /// </summary>
    internal enum SYSTEM_OP
    {
        /// <summary>
        /// Begin file download
        /// </summary>
        BEGIN_DOWNLOAD = 0x92,
        /// <summary>
        /// Continue file download
        /// </summary>
        CONTINUE_DOWNLOAD = 0x93,
        /// <summary>
        /// Begin file upload
        /// </summary>
        BEGIN_UPLOAD = 0x94,
        /// <summary>
        /// Continue file upload
        /// </summary>
        CONTINUE_UPLOAD = 0x95,
        /// <summary>
        /// Begin get bytes from a file (while writing to the file)
        /// </summary>
        BEGIN_GETFILE = 0x96, 
        /// <summary>
        /// Continue get byte from a file (while writing to the file)
        /// </summary>
        CONTINUE_GETFILE = 0x97, 
        /// <summary>
        /// Close file handle
        /// </summary>
        CLOSE_FILEHANDLE = 0x98,
        /// <summary>
        /// List files
        /// </summary>
        LIST_FILES = 0x99,
        /// <summary>
        /// Continue list files
        /// </summary>
        CONTINUE_LIST_FILES = 0x9A,
        /// <summary>
        /// Create directory
        /// </summary>
        CREATE_DIR = 0x9B,
        /// <summary>
        /// Delete
        /// </summary>
        DELETE_FILE = 0x9C,
        /// <summary>
        /// List handles
        /// </summary>
        LIST_OPEN_HANDLES = 0x9D,
        /// <summary>
        /// Write to mailbox
        /// </summary>
        WRITEMAILBOX = 0x9E,
        /// <summary>
        /// Transfer trusted pin code to brick
        /// </summary>
        BLUETOOTHPIN = 0x9F,
        /// <summary>
        /// Restart the brick in Firmware update mode
        /// </summary>
        ENTERFWUPDATE = 0xA0 
    }
}
