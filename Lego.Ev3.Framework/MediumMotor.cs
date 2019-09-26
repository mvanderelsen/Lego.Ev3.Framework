using Lego.Ev3.Framework.Devices;
using System.Threading.Tasks;

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
        /// Method allows two motors to synchronize and expose synchronized methods
        /// </summary>
        /// <param name="motor">The motor where this motor is synchronized with</param>
        /// <returns>a set of synchronized motors</returns>
        public SynchronizedMotors Synchronize(MediumMotor motor)
        {
            return new SynchronizedMotors(this, motor);
        }
    }
}
