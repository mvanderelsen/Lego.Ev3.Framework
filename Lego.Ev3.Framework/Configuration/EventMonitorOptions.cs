namespace Lego.Ev3.Framework.Configuration
{
    public class EventMonitorOptions : Options
    {

        /// <summary>
        /// EventMonitor is enabled yes/no. Default: <c>true</c>
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Poll Interval in milliseconds. Default: <c>100</c>
        /// </summary>
        public int Interval { get; set; } = 100;
    }
}
