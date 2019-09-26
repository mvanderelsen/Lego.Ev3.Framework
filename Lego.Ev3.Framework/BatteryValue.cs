using System;

namespace Lego.Ev3.Framework
{

    /// <summary>
    /// Data about the LEGO® MINDSTORMS® EV3 Brick Battery
    /// </summary>
    public sealed class BatteryValue : IEquatable<BatteryValue>, IComparable<BatteryValue>
    {
        public BatteryMode Mode { get; }

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

        private float Value { get { return Level + Temperature + Ampere + Voltage; } }

        internal BatteryValue(BatteryMode mode)
        {
            Mode = mode;
        }


        public override string ToString()
        {
            switch (Mode)
            {
                case BatteryMode.Level: return $"{Level}%";
            }
            return $"Voltage:{Voltage} Ampere:{Ampere} Temperature:{Temperature} Level:{Level}%";
        }

        #region operators
#pragma warning disable 1591

        public override bool Equals(object obj)
        {
            return Equals(obj as BatteryValue);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(BatteryValue other)
        {
            if (other == null) return false;
            return Value == other.Value;
        }

        public int CompareTo(BatteryValue other)
        {
            return Value.CompareTo(other.Value);
        }

        public static bool operator ==(BatteryValue obj, BatteryValue other)
        {
            return Equals(obj, other);
        }

        public static bool operator !=(BatteryValue obj, BatteryValue other)
        {
            return !Equals(obj, other);
        }

#pragma warning restore
        #endregion
    }
}
