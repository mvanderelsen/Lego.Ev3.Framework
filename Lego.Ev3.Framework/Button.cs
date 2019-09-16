using System.Threading;
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
        /// Delegate for button click events
        /// </summary>
        /// <param name="button">The button that was clicked</param>
        public delegate void OnClicked(Button button);

        /// <summary>
        /// The button click event
        /// </summary>
        public event OnClicked Clicked;

        internal Button(ButtonType type)
        {
            Type = type;
        }


        internal void ClickBatchCommand(PayLoadBuilder payLoadBuilder, int index)
        {
            UIButtonMethods.Pressed_BatchCommand(payLoadBuilder, (int)Type, index);
        }

        internal void RaiseClickEvent()
        {
            if (Clicked != null)
            {
                if (Brick.Socket.SynchronizationContext == SynchronizationContext.Current) Clicked(this);
                else Brick.Socket.SynchronizationContext.Post(delegate { Clicked(this); }, null);
            }
        }

        internal bool OnClickSubscribed
        {
            get
            {
                return Clicked != null;
            }
        }


        /*

        TESTSHORTPRESS
        TESTLONGPRESS
        PRESSED
        GET_BUMBED

        */
    }
}
