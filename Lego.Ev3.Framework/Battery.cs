using Lego.Ev3.Framework.Core;
using Lego.Ev3.Framework.Firmware;
using System.Threading;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework
{

    /// <summary>
    /// LEGO® MINDSTORMS® EV3 Brick Battery
    /// </summary>
    public sealed class Battery
    {
        /// <summary>
        /// Delegate for input changed on battery
        /// </summary>
        /// <param name="value">The value of the battery</param>
        public delegate void OnValueChanged(BatteryValue value);
        /// <summary>
        /// Battery value changed event
        /// </summary>
        public event OnValueChanged ValueChanged;

        /// <summary>
        /// Gets or sets if this battery is automatically polled and fires onchange events
        /// If false value must be read manually
        /// </summary>
        public bool MonitorEvents { get; set; }

        /// <summary>
        /// Sets or gets the Battery Mode
        /// </summary>
        public BatteryMode Mode { get; set; }

        /// <summary>
        /// The current battery value if null call GetValue first
        /// </summary>
        public BatteryValue Value { get; private set; }

        internal Battery()
        {
            Mode = BatteryMode.All;
            MonitorEvents = true;
        }

        /// <summary>
        /// Gets all battery information
        /// </summary>
        /// <returns></returns>
        public async Task<BatteryValue> GetValue()
        {
            BatteryValue value = await UIReadMethods.GetBatteryValue(Brick.Socket);
            Value = value;
            return value;
        }


        internal bool SetValue(byte[] data)
        {
            BatteryValue newValue = null;
            switch (Mode)
            {
                case BatteryMode.All:
                    {
                        newValue = UIReadMethods.BatteryValue(data, 0);
                        break;
                    }
                case BatteryMode.Level:
                    {
                        newValue = new BatteryValue(Mode) { Level = UIReadMethods.BatteryValueLevel(data,0) };
                        break;
                    }
            }

            bool hasChanged = (Value != newValue);
            if (hasChanged)
            {
                Value = newValue;
                if (ValueChanged != null && MonitorEvents)
                {
                    if (Brick.Socket.SynchronizationContext == SynchronizationContext.Current) ValueChanged(Value);
                    else Brick.Socket.SynchronizationContext.Post(delegate { ValueChanged(Value); }, null);
                }

            }
            return hasChanged;
        }

        internal ushort BatchCommand(PayLoadBuilder payLoadBuilder, int index)
        {
            ushort byteLength = 0;
            if (!MonitorEvents || ValueChanged == null) return byteLength; // no need to poll data
            switch (Mode)
            {
                case BatteryMode.All: return UIReadMethods.GetBatteryValue_BatchCommand(payLoadBuilder, index);
                case BatteryMode.Level:
                    {
                        DataType type = UIReadMethods.GetBatteryLevel_BatchCommand(payLoadBuilder, index);
                        return type.ByteLength();
                    }

            }
            return byteLength;
        }
    }
}
