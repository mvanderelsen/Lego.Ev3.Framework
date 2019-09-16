namespace Lego.Ev3.Framework.Firmware
{

	internal enum COM_GET_SUBCODE
	{
		  GET_ON_OFF    = 1,                    //!< Set, Get
		  GET_VISIBLE   = 2,                    //!< Set, Get
		  GET_RESULT    = 4,                    //!<      Get
		  GET_PIN       = 5,                    //!< Set, Get
		  SEARCH_ITEMS  = 8,                    //!<      Get
		  SEARCH_ITEM   = 9,                    //!<      Get
		  FAVOUR_ITEMS  = 10,                   //!<      Get
		  FAVOUR_ITEM   = 11,                   //!<      Get
		  GET_ID        = 12,
		  GET_BRICKNAME = 13,
		  GET_NETWORK   = 14,
		  GET_PRESENT   = 15,
		  GET_ENCRYPT   = 16,
		  CONNEC_ITEMS  = 17,
		  CONNEC_ITEM   = 18,
		  GET_INCOMING  = 19,
		  GET_MODE2     = 20,
	}

}
