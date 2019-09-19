
namespace Lego.Ev3.Framework.Devices
{
    /// <summary>
    /// Interface for devices that can be connected to a Input Port
    /// </summary>
    public interface IInputDevice
    {
        /// <summary>
        /// gets or sets the id of the device
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Gets the device type of the input device
        /// </summary>
        DeviceType Type { get; }

    }
}
