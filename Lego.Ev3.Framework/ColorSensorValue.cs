using System;

namespace Lego.Ev3.Framework
{
    /// <summary>
    /// Holds Color Sensor data
    /// </summary>
    public sealed class ColorSensorValue : IEquatable<ColorSensorValue>, IComparable<ColorSensorValue>
    {
        /// <summary>
        /// Current Color Sensor Mode
        /// </summary>
        public ColorSensorMode Mode { get; }

        /// <summary>
        /// The color as detected by the sensor (Only in colormode else set to None)
        /// </summary>
        public ColorSensorColor Color { get; }

        /// <summary>
        /// The int value depending on color mode
        /// </summary>
        public int Value { get; }


        internal ColorSensorValue(ColorSensorColor color, ColorSensorMode mode)
        {
            Mode = mode;
            Color = color;
            Value = (int)color;
        }

        internal ColorSensorValue(int value, ColorSensorMode mode)
        {
            Mode = mode;
            Color = ColorSensorColor.None;
            Value = value;
        }


        /// <summary>
        /// Formatted string
        /// </summary>
        public override string ToString()
        {
            switch (Mode)
            {
                case ColorSensorMode.Color: return Color.ToString();
                default: return Value.ToString();
            }
        }


        #region operators
#pragma warning disable 1591

        public override bool Equals(object obj)
        {
            return Equals(obj as ColorSensorValue);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(ColorSensorValue other)
        {
            if (other == null) return false;
            return Value == other.Value;
        }

        public int CompareTo(ColorSensorValue other)
        {
            return Value.CompareTo(other.Value);
        }

        public static bool operator ==(ColorSensorValue obj, ColorSensorValue other)
        {
            return Equals(obj, other);
        }

        public static bool operator !=(ColorSensorValue obj, ColorSensorValue other)
        {
            return !Equals(obj, other);
        }

#pragma warning restore
        #endregion
    }
}
