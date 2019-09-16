namespace Lego.Ev3.Framework.Firmware
{

	internal enum DEVCMD
	{
		  DEVCMD_RESET        = 0x11,           //!< UART device reset
		  DEVCMD_FIRE         = 0x11,           //!< UART device fire   (ultrasonic)
		  DEVCMD_CHANNEL      = 0x12,           //!< UART device channel (IR seeker)
	}

}
