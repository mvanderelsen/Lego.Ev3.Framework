namespace Lego.Ev3.Framework.Firmware
{

	internal enum FILETYPE
	{
		  FILETYPE_UNKNOWN              = 0x00,
		  TYPE_FOLDER                   = 0x01,
		  TYPE_SOUND                    = 0x02,
		  TYPE_BYTECODE                 = 0x03,
		  TYPE_GRAPHICS                 = 0x04,
		  TYPE_DATALOG                  = 0x05,
		  TYPE_PROGRAM                  = 0x06,
		  TYPE_TEXT                     = 0x07,
		  TYPE_SDCARD                   = 0x10,
		  TYPE_USBSTICK                 = 0x20,
		  TYPE_RESTART_BROWSER          = -1,
		  TYPE_REFRESH_BROWSER          = -2
	}

}
