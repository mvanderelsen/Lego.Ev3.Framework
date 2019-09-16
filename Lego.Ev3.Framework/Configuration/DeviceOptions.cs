using System;

namespace Lego.Ev3.Framework.Configuration
{
    public class DeviceOptions : Options
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public DeviceType Type { get; set; }

        public ChainLayer Layer { get; set; } = ChainLayer.One;

        public string Port { get; set; }

        public string Mode { get; set; }
    }
}
