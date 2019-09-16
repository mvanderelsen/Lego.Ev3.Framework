using Lego.Ev3.Framework.Devices;
using Lego.Ev3.Framework.Firmware;
using Lego.Ev3.Framework.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework
{
    /// <summary>
    /// LEGO® MINDSTORMS® EV3 Color Sensor
    /// TODO: other color modes
    /// </summary>
    public sealed class ColorSensor : InputDevice
    {
        /// <summary>
        /// Delegate for input changed on sensor
        /// </summary>
        /// <param name="sensor">The sensor that changed</param>
        /// <param name="value">The value of the sensor</param>
        public delegate void OnInputChanged(ColorSensor sensor, ColorSensorValue value);
        /// <summary>
        /// Sensor value changed event
        /// </summary>
        public event OnInputChanged InputChanged;


        /// <summary>
        /// The device mode
        /// </summary>
        public ColorSensorMode Mode { get; private set; }

        /// <summary>
        /// The value
        /// </summary>
        public ColorSensorValue Value { get; private set; }

        /// <summary>
        /// constructs a LEGO® MINDSTORMS® EV3 Color Sensor
        /// </summary>
        public ColorSensor() : base(DeviceType.ColorSensor)
        {
            Mode = ColorSensorMode.Color;
            Value = new ColorSensorValue(ColorSensorColor.None);
        }
        ///// <summary>
        ///// constructs a LEGO® MINDSTORMS® EV3 Color Sensor
        ///// </summary>
        //public ColorSensor(ColorSensorMode mode)
        //    : base(DeviceType.ColorSensor)
        //{
        //    Mode = mode;
        //    Value = new ColorSensorValue(ColorSensorColor.None);
        //}

        /// <summary>
        /// Init
        /// </summary>
        protected sealed override Task Initialize()
        {

            return Task.FromResult(true);
        }


        internal override DataType BatchCommand(PayLoadBuilder codeBuilder, int index)
        {
            //Maybe no need to switch mode after first call to this method
            //Could perhaps use GetRaw instead??
            return InputMethods.GetReadyRaw_BatchCommand(codeBuilder, Layer, PortNumber, 0, (int)Mode, 1, index);
        }

        /// <summary>
        /// Override to set the value from EventMonitor
        /// </summary>
        protected sealed override bool SetValue(object value)
        {
            ColorSensorValue newValue = ConvertToSensorValue(value);

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


        private ColorSensorValue ConvertToSensorValue(object value)
        {
            if (value == null) return null;
            ColorSensorValue colorSensorValue;
            switch (Mode)
            {
                case ColorSensorMode.Color:
                    {
                        colorSensorValue = new ColorSensorValue(((ColorSensorColor)(int)value));
                        break;
                    }
                default: //TODO test other colormodes!!
                    {
                        colorSensorValue = new ColorSensorValue((int)value);
                        break;
                    }
            }
            return colorSensorValue;
        }

        /// <summary>
        /// Gets the value depending on the current mode
        /// </summary>
        /// <returns>value</returns>
        public async Task<ColorSensorValue> GetValue()
        {
            int raw = await InputMethods.GetReadyRaw(Brick.Socket, PortNumber, 0, (int)Mode);
            ColorSensorValue value = ConvertToSensorValue(raw);
            Value = value;
            return value;
        }

    }
}
