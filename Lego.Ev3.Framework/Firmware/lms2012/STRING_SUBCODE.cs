namespace Lego.Ev3.Framework.Firmware
{

	internal enum STRING_SUBCODE
	{
		  GET_SIZE            = 1,    // VM       get string size
		  ADD                 = 2,    // VM       add two strings
		  COMPARE             = 3,    // VM       compare two strings
		  DUPLICATE           = 5,    // VM       duplicate one string to another
		  VALUE_TO_STRING     = 6,
		  STRING_TO_VALUE     = 7,
		  STRIP               = 8,
		  NUMBER_TO_STRING    = 9,
		  SUB                 = 10,
		  VALUE_FORMATTED     = 11,
		  NUMBER_FORMATTED    = 12,
	}

}
