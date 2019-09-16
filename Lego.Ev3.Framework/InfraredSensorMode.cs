
namespace Lego.Ev3.Framework
{
    /// <summary>
    /// LEGO® MINDSTORMS® EV3 Infrared Sensor device mode <see cref="Devices.DeviceMode"/>
    /// 0 = EV3-IR-Proximity
    /// 1 = EV3-IR-Seeker
    /// 2 = EV3-IR-Remote
    /// 3 = EV3-IR-Remote-Advanced
    /// 4 = Not utilized
    /// 5 = EV3-IR-Calibration
    /// </summary>
    public enum InfraredSensorMode
    {
        /// <summary>
        /// Proximity
        /// </summary>
        Proximity = 0,
        /// <summary>
        /// Seeker
        /// </summary>
        Seeker = 1,
        /// <summary>
        /// Remote
        /// </summary>
        Remote = 2,
        /// <summary>
        /// Remote Advanced
        /// </summary>
        RemoteAdvanced = 3,
        /// <summary>
        /// Do Not Use
        /// </summary>
        NotUtilized = 4,
        /// <summary>
        /// Do Not Use
        /// </summary>
        Calibration = 5
    }
}
