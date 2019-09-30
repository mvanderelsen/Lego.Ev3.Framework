namespace Lego.Ev3.Framework
{

    /// <summary>
    /// LEGO® MINDSTORMS® EV3 Gyroscope Sensor device mode
    /// </summary>
    public enum GyroscopeSensorMode
    {
        /// <summary>
        /// Angle
        /// </summary>
        Angle = 0,
        /// <summary>
        /// Rate of movement
        /// </summary>
        Rate = 1,
        /// <summary>
        /// Unknown
        /// </summary>
        Fast = 2,        // TOOD: ??
        /// <summary>
        /// Unknown
        /// </summary>
        RateAndAngle = 3,      // TODO: ??
        /// <summary>
        /// Calibrate
        /// </summary>
        Calibration = 4
    }
}