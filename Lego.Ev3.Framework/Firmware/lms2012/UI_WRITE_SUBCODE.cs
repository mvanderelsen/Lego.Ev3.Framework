namespace Lego.Ev3.Framework.Firmware
{

	internal enum UI_WRITE_SUBCODE
	{
		  WRITE_FLUSH   = 1,
		  FLOATVALUE    = 2,
		  STAMP         = 3,
		  PUT_STRING    = 8,
		  VALUE8        = 9,
		  VALUE16       = 10,
		  VALUE32       = 11,
		  VALUEF        = 12,
		  ADDRESS       = 13,
		  CODE          = 14,
		  DOWNLOAD_END  = 15,
		  SCREEN_BLOCK  = 16,
		  ALLOW_PULSE   = 17,
		  SET_PULSE     = 18,
		  TEXTBOX_APPEND = 21,
		  SET_BUSY      = 22,
		  SET_TESTPIN   = 24,
		  INIT_RUN      = 25,
		  UPDATE_RUN    = 26,
		  LED           = 27,
		  POWER         = 29,
		  GRAPH_SAMPLE  = 30,
		  TERMINAL      = 31,
	}

}
