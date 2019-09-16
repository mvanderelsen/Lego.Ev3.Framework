namespace Lego.Ev3.Framework
{

    /// <summary>
    /// Information about the LEGO® MINDSTORMS® EV3 Brick Battery
    /// </summary>
    public sealed class BatteryInfo
    {
        /// <summary>
        /// Battery voltage[V]
        /// </summary>
        public float Voltage { get; internal set; }

        /// <summary>
        /// Battery current[A]
        /// </summary>
        public float Ampere { get; internal set; }

        /// <summary>
        /// Battery temperature rise[C]
        /// </summary>
        public float Temperature { get; internal set; }

        /// <summary>
        /// Battery level in percentage[0 - 100]
        /// </summary>
        public int Level { get; internal set; }

        internal BatteryInfo()
        {

        }


        public override string ToString()
        {
            return $"Battery Voltage:{Voltage} Ampere:{Ampere} Temperature:{Temperature} Level:{Level}%";
        }
    }
}
