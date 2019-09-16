namespace Lego.Ev3.Framework.Firmware
{

	internal enum TYPE
	{
		  MODE_KEEP                     =  -1,  //!< Mode value that won't change mode in byte codes (convenient place to define)
		  TYPE_KEEP                     =   0,  //!< Type value that won't change type in byte codes
		  TYPE_NXT_TOUCH                =   1,  //!< Device is NXT touch sensor
		  TYPE_NXT_LIGHT                =   2,  //!< Device is NXT light sensor
		  TYPE_NXT_SOUND                =   3,  //!< Device is NXT sound sensor
		  TYPE_NXT_COLOR                =   4,  //!< Device is NXT color sensor
		  TYPE_NXT_ULTRASONIC           =   5,  //!< Device is NXT ultra sonic sensor
		  TYPE_NXT_TEMPERATURE          =   6,  //!< Device is NXT temperature sensor
		  TYPE_TACHO                    =   7,  //!< Device is EV3/NXT tacho motor
		  TYPE_MINITACHO                =   8,  //!< Device is EV3 mini tacho motor
		  TYPE_NEWTACHO                 =   9,  //!< Device is EV3 new tacho motor
		  TYPE_TOUCH                    =  16,  //!< Device is EV3 touch sensor
		  TYPE_COLOR                    =  29,  //!< Device is EV3 color sensor
		  TYPE_ULTRASONIC               =  30,  //!< Device is EV3 ultra sonic sensor
		  TYPE_GYRO                     =  32,  //!< Device is EV3 gyro sensor
		  TYPE_IR                       =  33,  //!< Device is EV3 IR sensor
		  TYPE_THIRD_PARTY_START        =  50,
		  TYPE_THIRD_PARTY_END          =  98,
		  TYPE_ENERGYMETER              =  99,  //!< Device is energy meter
		  TYPE_IIC_UNKNOWN              = 100,  //!< Device type is not known yet
		  TYPE_NXT_TEST                 = 101,  //!< Device is a NXT ADC test sensor
		  TYPE_NXT_IIC                  = 123,  //!< Device is NXT IIC sensor
		  TYPE_TERMINAL                 = 124,  //!< Port is connected to a terminal
		  TYPE_UNKNOWN                  = 125,  //!< Port not empty but type has not been determined
		  TYPE_NONE                     = 126,  //!< Port empty or not available
		  TYPE_ERROR                    = 127,  //!< Port not empty and type is invalid
	}

}
