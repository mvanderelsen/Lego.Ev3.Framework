
namespace Lego.Ev3.Framework
{
    /// <summary>
    /// LEGO® MINDSTORMS® EV3 Color Sensor device mode <see cref="Devices.DeviceMode"/>
    /// </summary>
    public enum ColorSensorMode
    {
        /// <summary>
        /// EV3-Color-Reflected
        /// </summary>
        Reflected = 0,
        /// <summary>
        /// EV3-Color-Ambient
        /// </summary>
        Ambient = 1,
        /// <summary>
        /// EV3-Color-Color
        /// </summary>
        Color = 2,
        /// <summary>
        /// EV3-Color-Reflected-Raw
        /// </summary>
        Reflected_RAW = 3,
        /// <summary>
        /// EV3-Color-RGB-Raw
        /// </summary>
        RGB_RAW = 4,
        /// <summary>
        /// EV3-Color-Calibration
        /// </summary>
        Calibration = 5
    }
}
