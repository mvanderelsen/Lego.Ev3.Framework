namespace Lego.Ev3.Framework.Firmware
{

	internal enum OBJSTAT
	{
		  RUNNING = 0x0010,                     //!< Object code is running
		  WAITING = 0x0020,                     //!< Object is waiting for final trigger
		  STOPPED = 0x0040,                     //!< Object is stopped or not triggered yet
		  HALTED  = 0x0080,                     //!< Object is halted because a call is in progress
	}

}
