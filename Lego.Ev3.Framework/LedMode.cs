namespace Lego.Ev3.Framework
{
    /// <summary>
    /// Mode of the led on the brick
    /// </summary>
    public enum LedMode
    {
        /// <summary>
        /// Led off
        /// </summary>
        Off = 0x00,
        /// <summary>
        /// Led green
        /// </summary>
        Green = 0x01,
        /// <summary>
        /// Led red
        /// </summary>
        Red = 0x02,
        /// <summary>
        /// Led orange
        /// </summary>
        Orange = 0x03,
        /// <summary>
        /// Led green flashing
        /// </summary>
        GreenFlashing = 0x04,
        /// <summary>
        /// Led red flashing
        /// </summary>
        RedFlashing = 0x05,
        /// <summary>
        /// Led orange flashing
        /// </summary>
        OrangeFlashing = 0x06,
        /// <summary>
        /// Led green pulse
        /// </summary>
        GreenPulse = 0x07,
        /// <summary>
        /// Led red pulse
        /// </summary>
        RedPulse = 0x08,
        /// <summary>
        /// Led orange pulse
        /// </summary>
        OrangePulse = 0x09
    }
}
