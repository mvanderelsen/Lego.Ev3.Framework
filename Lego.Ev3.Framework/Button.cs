using Lego.Ev3.Framework.Firmware;

namespace Lego.Ev3.Framework
{

    /// <summary>
    /// LEGO® MINDSTORMS® EV3 Brick Button
    /// </summary>
    public sealed class Button
    {
        /// <summary>
        /// Gets the type of the Button
        /// </summary>
        public ButtonType Type { get; }

        /// <summary>
        /// Mode of the button
        /// </summary>
        public ButtonMode Mode { get; private set; }

        /// <summary>
        /// Delegate for button events
        /// </summary>
        /// <param name="button">The button that was clicked, pressed depending on mode</param>
        public delegate void OnClicked(Button button);

        /// <summary>
        /// The button click event
        /// </summary>
        public event OnClicked Clicked;

        internal Button(ButtonType type)
        {
            Type = type;
            Mode = ButtonMode.Click;
        }

        /// <summary>
        /// Set the button mode
        /// </summary>
        /// <param name="mode">The desired button mode</param>
        public void SetMode(ButtonMode mode) 
        {
            Mode = mode;
        }

        internal void BatchCommand(PayLoadBuilder payLoadBuilder, int index)
        {
            UIButtonMethods.BatchCommand(payLoadBuilder, Type, Mode, index);
        }

        internal void RaiseClickEvent()
        {
            Clicked?.Invoke(this);
        }

        internal bool OnClickSubscribed
        {
            get
            {
                return Clicked != null;
            }
        }
    }
}
