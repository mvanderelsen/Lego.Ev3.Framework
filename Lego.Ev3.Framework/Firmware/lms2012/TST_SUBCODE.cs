namespace Lego.Ev3.Framework.Firmware
{

	internal enum TST_SUBCODE
	{
		  TST_OPEN                      = 10,   //!< MUST BE GREATER OR EQUAL TO "INFO_SUBCODES"
		  TST_CLOSE                     = 11,
		  TST_READ_PINS                 = 12,
		  TST_WRITE_PINS                = 13,
		  TST_READ_ADC                  = 14,
		  TST_WRITE_UART                = 15,
		  TST_READ_UART                 = 16,
		  TST_ENABLE_UART               = 17,
		  TST_DISABLE_UART              = 18,
		  TST_ACCU_SWITCH               = 19,
		  TST_BOOT_MODE2                = 20,
		  TST_POLL_MODE2                = 21,
		  TST_CLOSE_MODE2               = 22,
		  TST_RAM_CHECK                 = 23,
	}

}
