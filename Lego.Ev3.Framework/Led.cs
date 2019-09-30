using Lego.Ev3.Framework.Firmware;
using System.Threading.Tasks;

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
        public async void SetValue(LedMode mode)
        {
            Mode = mode;
            await UIWriteMethods.Led(Brick.Socket, (int)mode);

        }

        /// <summary>
        /// Resets the led back to color green
        /// </summary>
        /// <returns></returns>
        public async Task Reset()
        {
            if (Mode != LedMode.Green)
            {
                Mode = LedMode.Green;
                await UIWriteMethods.Led(Brick.Socket, (int)Mode);
            }
        }
    }
}
