using Lego.Ev3.Framework.Devices;
using Lego.Ev3.Framework.Firmware;
using Lego.Ev3.Framework.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework
{
    /// <summary>
    /// LEGO® MINDSTORMS® EV3 Touch Sensor
    /// </summary>
    public sealed class TouchSensor : InputDevice
    {
        /// <summary>
        /// Delegate for input changed on sensor
        /// </summary>
        /// <param name="sensor">The sensor that changed</param>
        /// <param name="value">The value of the sensor</param>
        public delegate void OnInputChanged(TouchSensor sensor, int value);
        /// <summary>
        /// Sensor value changed event
        /// </summary>
        public event OnInputChanged InputChanged;

        /// <summary>
        /// The Touchsensor mode
        /// </summary>
        public TouchSensorMode Mode { get; private set; }

        /// <summary>
        /// The value
        /// </summary>
        public int Value { get; private set; }


        /// <summary>
        /// constructs a LEGO® MINDSTORMS® EV3 Touch Sensor
        /// </summary>
        public TouchSensor()
            :this(TouchSensorMode.Touch)
        {
        }

        /// <summary>
        /// constructs a LEGO® MINDSTORMS® EV3 Touch Sensor
        /// </summary>
        /// <param name="mode">The initial mode default Touch</param>
        public TouchSensor(TouchSensorMode mode)
            : base(DeviceType.TouchSensor)
        {
            Mode = mode;
            DeviceMode = (DeviceMode)mode;
            Value = 0;
        }

        /// <summary>
        /// Clear all counters on init
        /// </summary>
        protected sealed override async Task Initialize()
        {
            await Clear();
        }

        internal override DataType BatchCommand(PayLoadBuilder payLoadBuilder, int index)
        {
            //Documentation is wrong should be reverse getChanges = Bump and Get_bump = Touch 
           if(Mode == TouchSensorMode.Bump) return InputMethods.GetChangesValue_BatchCommand(payLoadBuilder, Layer, PortNumber, index);
           return InputMethods.GetBumpsValue_BatchCommand(payLoadBuilder, Layer, PortNumber, index);
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
                if (MonitorEvents) InputChanged?.Invoke(this, newValue);
            }
            return hasChanged;
        }

        /// <summary>
        /// Clear changes and bumps and sets value to 0
        /// </summary>
        /// <returns></returns>
        public async Task Clear()
        {
            await InputMethods.ClearChanges(Socket, PortNumber);
            Value = 0;
        }

        /// <summary>
        /// Sets new mode, clears changes and bumps and sets value to 0
        /// </summary>
        /// <param name="mode">the mode</param>
        /// <returns></returns>
        public async Task SetMode(TouchSensorMode mode)
        {
            Mode = mode;
            DeviceMode = (DeviceMode)mode;
            await Clear();
        }

        /// <summary>
        /// Gets the value depending on the current mode
        /// </summary>
        /// <returns>value</returns>
        public async Task<int> GetValue()
        {
            //Documentation is wrong should be reverse getChanges = Bump and Get_bump = Touch 
            int value  = (Mode == TouchSensorMode.Bump) ? await InputMethods.GetChanges(Socket, PortNumber) : await InputMethods.GetBumps(Socket, PortNumber);
            Value = value;
            return value;
        }

    }
}
