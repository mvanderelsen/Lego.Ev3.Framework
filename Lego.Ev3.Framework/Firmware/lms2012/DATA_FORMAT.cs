namespace Lego.Ev3.Framework.Firmware
{

	internal enum DATA_FORMAT
	{
		  DATA_8        = 0x00,                 //!< DATA8  (don't change)
		  DATA_16       = 0x01,                 //!< DATA16 (don't change)
		  DATA_32       = 0x02,                 //!< DATA32 (don't change)
		  DATA_F        = 0x03,                 //!< DATAF  (don't change)
		  DATA_S        = 0x04,                 //!< Zero terminated string
		  DATA_A        = 0x05,                 //!< Array handle
		  DATA_V        = 0x07,                 //!< Variable type
		  DATA_PCT      = 0x10,                 //!< Percent (used in opINPUT_READEXT)
		  DATA_RAW      = 0x12,                 //!< Raw     (used in opINPUT_READEXT)
		  DATA_SI       = 0x13,                 //!< SI unit (used in opINPUT_READEXT)
	}

}
