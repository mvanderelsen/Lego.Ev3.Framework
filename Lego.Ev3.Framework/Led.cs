using Lego.Ev3.Framework.Firmware;

namespace Lego.Ev3.Framework
{
    /// <summary>
    /// Led located in center of brick
    /// </summary>
    public sealed class Led
    {
        /// <summary>
        /// Current mode of the led
        /// default = Green
        /// </summary>
        public LedMode Mode { get; private set; }

        internal Led()
        {
            Mode = LedMode.Green;
        }


        /// <summary>
        /// Change the current led mode.
        /// </summary>
        /// <param name="mode">the mode to change led to</param>
        public async void ChangeMode(LedMode mode)
        {
            await UIWriteMethods.Led(Brick.Socket, (int)mode);
            Mode = mode;

        }


    }
}
