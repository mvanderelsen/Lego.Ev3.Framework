namespace Lego.Ev3.Framework.Firmware
{

	internal enum SLOT
	{
		  GUI_SLOT                      = 0,    //!< Program slot reserved for executing the user interface
		  USER_SLOT                     = 1,    //!< Program slot used to execute user projects, apps and tools
		  CMD_SLOT                      = 2,    //!< Program slot used for direct commands coming from c_com
		  TERM_SLOT                     = 3,    //!< Program slot used for direct commands coming from c_ui
		  DEBUG_SLOT                    = 4,    //!< Program slot used to run the debug ui
		  CURRENT_SLOT                  = -1
	}

}
