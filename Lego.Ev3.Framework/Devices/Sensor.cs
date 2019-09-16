
namespace Lego.Ev3.Framework.Devices
{
    /// <summary>
    /// A base class with all sensor common functions
    /// Call methods only after connection to the brick is made.
    /// </summary>
    public abstract class Sensor: InputDevice 
    {
        /// <summary>
        /// Constructs Sensor  <see cref="Device(DeviceType)"/>
        /// Mode is automatically set through method calls
        /// </summary>
        /// <param name="type">The Device type of the device</param>
        protected Sensor(DeviceType type) : base(type) { }
    }
}
