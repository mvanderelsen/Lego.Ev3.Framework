using System;
using Lego.Ev3.Framework.Devices;
namespace Lego.Ev3.Framework
{
    /// <summary>
    /// LEGO® MINDSTORMS® EV3 Medium Motor
    /// </summary>
    public sealed class MediumMotor: Motor
    {
        /// <summary>
        /// Constructs new LEGO® MINDSTORMS® EV3 Medium Motor
        /// </summary>
        public MediumMotor()
            : this(Polarity.Forward)
        {}

        /// <summary>
        /// Constructs new LEGO® MINDSTORMS® EV3 Medium Motor
        /// </summary>
        /// <param name="polarity">The initial polarity, default forward</param>
        public MediumMotor(Polarity polarity)
            : base(DeviceType.MediumMotor, polarity)
        {
        }

        /// <summary>
        /// Method allows two motors to sync and expose synchronized methods
        /// </summary>
        /// <param name="motor">The motor where this motor is synced with</param>
        /// <returns>a set of synced motors</returns>
        public SynchronizedMotors Sync(MediumMotor motor)
        {
            return new SynchronizedMotors(this, motor);
        }
    }
}
