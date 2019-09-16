namespace Lego.Ev3.Framework.Firmware
{

	internal enum MATHTYPE
	{
		  EXP                           = 1,    //!< e^x            r = expf(x)
		  MOD                           = 2,    //!< Modulo         r = fmod(x,y)
		  FLOOR                         = 3,    //!< Floor          r = floor(x)
		  CEIL                          = 4,    //!< Ceiling        r = ceil(x)
		  ROUND                         = 5,    //!< Round          r = round(x)
		  ABS                           = 6,    //!< Absolute       r = fabs(x)
		  NEGATE                        = 7,    //!< Negate         r = 0.0 - x
		  SQRT                          = 8,    //!< Squareroot     r = sqrt(x)
		  LOG                           = 9,    //!< Log            r = log10(x)
		  LN                            = 10,   //!< Ln             r = log(x)
		  SIN                           = 11,   //!<
		  COS                           = 12,   //!<
		  TAN                           = 13,   //!<
		  ASIN                          = 14,   //!<
		  ACOS                          = 15,   //!<
		  ATAN                          = 16,   //!<
		  MOD8                          = 17,   //!< Modulo DATA8   r = x % y
		  MOD16                         = 18,   //!< Modulo DATA16  r = x % y
		  MOD32                         = 19,   //!< Modulo DATA32  r = x % y
		  POW                           = 20,   //!< Exponent       r = powf(x,y)
		  TRUNC                         = 21,   //!< Truncate       r = (float)((int)(x * pow(y))) / pow(y)
	}

}
