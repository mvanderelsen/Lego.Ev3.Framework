namespace Lego.Ev3.Framework.Firmware
{

	internal enum RESULT
	{
		  OK            = 0,                    //!< No errors to report
		  BUSY          = 1,                    //!< Busy - try again
		  FAIL          = 2,                    //!< Something failed
		  STOP          = 4,                    //!< Stopped
		  START         = 8                     //!< Start
	}

}
