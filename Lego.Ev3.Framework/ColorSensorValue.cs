using System;

namespace Lego.Ev3.Framework
{
    /// <summary>
    /// Holds Color Sensor data
    /// </summary>
    public class ColorSensorValue
    {

        /// <summary>
        /// The color as detected by the sensor (Only in colormode else set to None)
        /// </summary>
        public ColorSensorColor Color { get; private set; }

        /// <summary>
        /// The int value depending on color mode
        /// </summary>
        public int Value { get; private set; }


        internal ColorSensorValue(ColorSensorColor color)
        {
            Color = color;
            Value = (int)color;
        }

        internal ColorSensorValue(int color)
        {
            Color = ColorSensorColor.None;
            Value = color;
        }


        /// <summary>
        /// Formatted string
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0} - {1}", Color, Value);
        }


        /// <summary>
        /// Determine if two colorvalues are not equal
        /// </summary>
        public static bool operator !=(ColorSensorValue color1, ColorSensorValue color2)
        {
            return !(color1.Value == color2.Value);
        }

        /// <summary>
        /// Determine if two colorvalues are equal
        /// </summary>
        public static bool operator ==(ColorSensorValue color1, ColorSensorValue color2)
        {
            if (Object.Equals(color1, null) && Object.Equals(color2, null)) return true;
            if (Object.Equals(color1, null) || Object.Equals(color2, null)) return false;
            return color1.Value == color2.Value;
        }


        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        /// GetHashCode override
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
