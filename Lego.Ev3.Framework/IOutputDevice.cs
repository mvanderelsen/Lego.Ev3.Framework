
namespace Lego.Ev3.Framework
{
    /// <summary>
    /// Interface for devices that can be connected to a Output Port
    /// </summary>
    public interface IOutputDevice
    {
        /// <summary>
        /// gets or sets the id of the device
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Gets the device type of the output device
        /// </summary>
        DeviceType Type { get; }

    }
}
