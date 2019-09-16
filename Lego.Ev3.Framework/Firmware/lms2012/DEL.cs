namespace Lego.Ev3.Framework.Firmware
{

	internal enum DEL
	{
		  DEL_NONE      = 0,                    //!< No delimiter at all
		  DEL_TAB       = 1,                    //!< Use tab as delimiter
		  DEL_SPACE     = 2,                    //!< Use space as delimiter
		  DEL_RETURN    = 3,                    //!< Use return as delimiter
		  DEL_COLON     = 4,                    //!< Use colon as delimiter
		  DEL_COMMA     = 5,                    //!< Use comma as delimiter
		  DEL_LINEFEED  = 6,                    //!< Use line feed as delimiter
		  DEL_CRLF      = 7,                    //!< Use return+line feed as delimiter
	}

}
