namespace Lego.Ev3.Framework.Configuration
{
    public class PowerUpSelfTestOptions : Options
    {
        /// <summary>
        /// PowerUpSelfTest is enabled yes/no. Default: <c>true</c>
        /// </summary>
        public bool Enabled { get; set; } = true;


        /// <summary>
        /// Beep if result status of the test is OK
        /// All devices are properly connected. Default: <c>true</c>
        /// </summary>
        public bool BeepOnOK { get; set; } = true;



        /// <summary>
        /// Beep twice if result status of the test is OK
        /// and devices where found and automatically connected to the brick
        /// AutoConnectDevices must be set to true. Default: <c>true</c>
        /// </summary>
        public bool BeepOnAutoConnect { get; set; } = true;


        /// <summary>
        /// Beep three times if devices where supposed to be connected but where not found.
        /// Will also fire when input device is connected to output port or vice versa. Default: <c>true</c>
        /// </summary>
        public bool BeepOnError { get; set; } = true;


        /// <summary>
        /// Set to true if the test will connect unset devices from code automatically when found on brick. Default: <c>false</c>
        /// </summary>
        public bool AutoConnectDevices { get; set; } = false;


        /// <summary>
        /// If errors found disconnects the brick from socket. Default: <c>true</c>
        /// </summary>
        public bool DisconnectOnError { get; set; } = true;

    }
}
