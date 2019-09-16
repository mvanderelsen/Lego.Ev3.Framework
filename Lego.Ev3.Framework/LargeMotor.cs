using System;
using Lego.Ev3.Framework.Devices;

namespace Lego.Ev3.Framework
{
    /// <summary>
    /// LEGO® MINDSTORMS® EV3 Large Motor
    /// </summary>
    public sealed class LargeMotor: Motor
    {
         /// <summary>
        /// Constructs new LEGO® MINDSTORMS® EV3 Large Motor
        /// </summary>
        public LargeMotor() 
            : this(Polarity.Forward)
        {}

        /// <summary>
        /// Constructs new LEGO® MINDSTORMS® EV3 Large Motor
        /// </summary>
        /// <param name="polarity">The initial polarity, default forward</param>
        public LargeMotor(Polarity polarity)
            : base(DeviceType.LargeMotor, polarity)
        {
        }

        /// <summary>
        /// Method allows two motors to sync and expose synchronized methods
        /// </summary>
        /// <param name="motor">The motor where this motor is synced with</param>
        /// <returns>a set of synced motors</returns>
        public SyncMotors Sync(LargeMotor motor)
        {
            return new SyncMotors(this, motor);
        }
    }
}
