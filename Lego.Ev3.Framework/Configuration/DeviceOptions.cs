using System;

namespace Lego.Ev3.Framework.Configuration
{

    /// <summary>
    /// Device option
    /// </summary>
    public class DeviceOptions : Options
    {
        /// <summary>
        /// Id of the device. Default <c>System.Guid</c>
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Device Type
        /// </summary>
        public DeviceType Type { get; set; }

        /// <summary>
        /// Chainlayer for daisy chain. If one brick leave to one. Default <c>One</c>
        /// </summary>
        public ChainLayer Layer { get; set; } = ChainLayer.One;


        /// <summary>
        /// Port Name A-D or One-Four. Required
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// Device mode as string. see the device for possible modes. If null default mode of the specified device is used.
        /// </summary>
        public string Mode { get; set; }
    }
}
