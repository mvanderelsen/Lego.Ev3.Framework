namespace Lego.Ev3.Framework
{

    /// <summary>
    /// LEGO® MINDSTORMS® EV3 Ultrasonic Sensor device mode <see cref="Devices.DeviceMode"/>
    /// </summary>
    public enum UltrasonicSensorMode
    {
        /// <summary>
        /// Values in centimeter units
        /// </summary>
        Centimeter = 0,
        /// <summary>
        /// Values in inch units
        /// </summary>
        Inch = 1,
        /// <summary>
        /// Listen mode
        /// </summary>
        Listen = 2,
        /// <summary>
        /// Unknown
        /// </summary>
        CentimeterSI = 3,
        /// <summary>
        /// Unknown
        /// </summary>
        InchSI = 4,
        /// <summary>
        /// Unknown
        /// </summary>
        CentimeterDC = 5,  // TODO: DC?
        /// <summary>
        /// Unknown
        /// </summary>
        InchDC	= 7	// TODO: DC?
    }
}
