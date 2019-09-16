namespace Lego.Ev3.Framework.Firmware
{

	internal enum WARNING
	{
		  WARNING_TEMP      = 0x01,
		  WARNING_CURRENT   = 0x02,
		  WARNING_VOLTAGE   = 0x04,
		  WARNING_MEMORY    = 0x08,
		  WARNING_DSPSTAT   = 0x10,
		  WARNING_RAM       = 0x20,
		  WARNING_BATTLOW   = 0x40,
		  WARNING_BUSY      = 0x80,
		  WARNINGS          = 0x3F
	}

}
