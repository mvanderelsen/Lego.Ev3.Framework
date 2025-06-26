namespace Lego.Ev3.Framework.Core
{
    /// <summary>
    /// A device
    /// </summary>
    public readonly struct PortInfo
    {
        /// <summary>
        /// Port number in chain
        /// 0-15 InputPorts (4 per brick eg. 0-3 are input ports on brick one)
        /// 16-31 OutputPorts (4 per brick eg. 16-19 are ouput ports on brick one)
        /// </summary>
        public int Number { get; }

        /// <summary>
        /// The device type if a device is connected to port
        /// </summary>
        public DeviceType? Device { get; }

        /// <summary>
        /// The port status. Should read OK if device is connected. If no device connected returns Empty
        /// </summary>
        public PortStatus Status { get; }

        internal PortInfo(int port, DeviceType device)
        {
            Number = port;
            Device = device;
            Status = PortStatus.OK;
        }

        internal PortInfo(int port, int status)
        {
            Number = port;
            Status = (PortStatus)status;
            Device = null;
        }

        /// <summary>
        /// Formats an entry to string
        /// </summary>
        /// <returns>formatted entry string</returns>
        public override string ToString()
        {
            return $"Port: {Number:00} Device: {Device} Status: {Status}";
        }
    }
}
