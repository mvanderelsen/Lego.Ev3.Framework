using Lego.Ev3.Framework.Devices;
using Lego.Ev3.Framework.Firmware;
using Lego.Ev3.Framework.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework
{
    /// <summary>
    /// LEGO® MINDSTORMS® EV3 Infrared Sensor
    /// </summary>
    public class InfraredSensor : Sensor
    {
        /// <summary>
        /// Delegate for input changed on sensor
        /// </summary>
        /// <param name="sensor">The sensor that changed</param>
        /// <param name="value">The value of the sensor</param>
        public delegate void OnInputChanged(InfraredSensor sensor, int value);
        /// <summary>
        /// Sensor value changed event
        /// </summary>
        public event OnInputChanged InputChanged;

        /// <summary>
        /// The value
        /// </summary>
        public int Value { get; private set; }

        /// <summary>
        /// Current Sensor Mode
        /// </summary>
        public InfraredSensorMode Mode { get; private set; }

        /// <summary>
        /// constructs a LEGO® MINDSTORMS® EV3 Infrared Sensor
        /// </summary>
        public InfraredSensor()
            : base(DeviceType.InfraredSensor)
        {
            Mode = InfraredSensorMode.Proximity;
        }

        /// <summary>
        /// Init
        /// </summary>
        protected sealed override Task Initialize()
        {
            return Task.FromResult(true);
        }

        internal override DataType BatchCommand(PayLoadBuilder payLoadBuilder, int index)
        {
            return InputMethods.GetReadyRaw_BatchCommand(payLoadBuilder, Layer, PortNumber, 0, (int)Mode, 1, index);
        }

        /// <summary>
        /// Override to set the value from EventMonitor
        /// </summary>
        protected sealed override bool SetValue(object value)
        {

            int newValue = Convert.ToInt32(value);
            bool hasChanged = (Value != newValue);
            if (hasChanged)
            {
                Value = newValue;
                if (InputChanged != null && MonitorEvents)
                {
                    if (Brick.Socket.SynchronizationContext == SynchronizationContext.Current) InputChanged(this, Value);
                    else Brick.Socket.SynchronizationContext.Post(delegate { InputChanged(this, Value); }, null);
                }

            }
            return hasChanged;
        }
    }
}
