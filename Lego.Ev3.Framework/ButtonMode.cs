namespace Lego.Ev3.Framework
{
    /// <summary>
    /// Button Mode
    /// </summary>
    public enum ButtonMode
    {
        /// <summary>
        /// Button event fires on short press
        /// </summary>
        ShortPress = 0x01,
        /// <summary>
        /// Button event fires on long press
        /// </summary>
        LongPress = 0x02,
        /// <summary>
        /// Button event fires on press
        /// </summary>
        Press = 0x09,
        /// <summary>
        /// Button event fires on click
        /// </summary>
        Click = 0x0E,
    }
}
