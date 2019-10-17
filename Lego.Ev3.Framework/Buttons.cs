using Lego.Ev3.Framework.Firmware;
using System.Collections.Generic;

namespace Lego.Ev3.Framework
{
    /// <summary>
    /// LEGO® MINDSTORMS® EV3 Brick Buttons
    /// </summary>
    public sealed class Buttons
    {

        /// <summary>
        /// Up button
        /// </summary>
        public Button Up { get; }

        /// <summary>
        /// Ok or Enter button located in center of brick's buttons
        /// </summary>
        public Button Ok { get; }

        /// <summary>
        /// Down button
        /// </summary>
        public Button Down { get; }

        /// <summary>
        /// Right button
        /// </summary>
        public Button Right { get; }

        /// <summary>
        /// Left button
        /// </summary>
        public Button Left { get; }

        /// <summary>
        /// Back button located bottom left of screen
        /// Use this button with care.
        /// </summary>
        public Button Back { get; }

        /// <summary>
        /// Mode of the button
        /// </summary>
        public ButtonMode Mode { get; private set; }

        /// <summary>
        /// Delegate for any button event
        /// </summary>
        public delegate void OnClicked();

        /// <summary>
        /// Any button click event
        /// </summary>
        public event OnClicked Clicked;

        internal Buttons()
        {
            Up = new Button(ButtonType.Up);
            Ok = new Button(ButtonType.Ok);
            Down = new Button(ButtonType.Down);
            Right = new Button(ButtonType.Right);
            Left = new Button(ButtonType.Left);
            Back = new Button(ButtonType.Back);

            Mode = ButtonMode.Click;
        }


        /// <summary>
        /// Sets the mode for all buttons
        /// </summary>
        /// <param name="mode"></param>
        public void SetMode(ButtonMode mode)
        {
            Mode = mode;
            Up.SetMode(mode);
            Ok.SetMode(mode);
            Down.SetMode(mode);
            Right.SetMode(mode);
            Left.SetMode(mode);
            Back.SetMode(mode);
        }

        #region internal Click Methods
        private List<ButtonType> _eventButtons;

        /// <summary>
        /// Store subscribed buttons in batch, buttons not always need to be polled
        /// and can be unsubscribed from click at any point in users program.
        /// </summary>
        /// <returns>byte length batchcommand</returns>
        internal ushort BatchCommand(PayLoadBuilder payLoadBuilder, int index)
        {
            ushort byteLength = 0;
            _eventButtons = new List<ButtonType>();
            if (Clicked != null)
            {
                UIButtonMethods.BatchCommand(payLoadBuilder, ButtonType.Any, Mode, index);
                _eventButtons.Add(ButtonType.Any);
                byteLength++;
            }
            else
            {
                if (Up.OnClickSubscribed)
                {
                    Up.BatchCommand(payLoadBuilder, index);
                    index++;
                    byteLength++;
                    _eventButtons.Add(Up.Type);
                }
                if (Ok.OnClickSubscribed)
                {
                    Ok.BatchCommand(payLoadBuilder, index);
                    index++;
                    byteLength++;
                    _eventButtons.Add(Ok.Type);
                }
                if (Down.OnClickSubscribed)
                {
                    Down.BatchCommand(payLoadBuilder, index);
                    index++;
                    byteLength++;
                    _eventButtons.Add(Down.Type);
                }
                if (Left.OnClickSubscribed)
                {
                    Left.BatchCommand(payLoadBuilder, index);
                    index++;
                    byteLength++;
                    _eventButtons.Add(Left.Type);
                }
                if (Right.OnClickSubscribed)
                {
                    Right.BatchCommand(payLoadBuilder, index);
                    index++;
                    byteLength++;
                    _eventButtons.Add(Right.Type);
                }
                if (Back.OnClickSubscribed)
                {
                    Back.BatchCommand(payLoadBuilder, index);
                    index++;
                    byteLength++;
                    _eventButtons.Add(Back.Type);
                }
            }

            if (byteLength == 0) _eventButtons = null;
            return byteLength;
        }

        internal void BatchCommandReturn(byte[] data)
        {
            if (data != null && _eventButtons != null)
            {
                int c = _eventButtons.Count;
                if (data.Length == c)
                {
                    for (int i = 0; i < c; i++)
                    {
                        bool clicked = data[i] == 1 ? true : false;
                        if (clicked)
                        {
                            ButtonType type = _eventButtons[i];
                            switch (type) 
                            {
                                case ButtonType.Any: 
                                    {
                                        Clicked?.Invoke();
                                        break;
                                    }
                                case ButtonType.Back: 
                                    {
                                        Back.RaiseClickEvent();
                                        break;
                                    }
                                case ButtonType.Down:
                                    {
                                        Down.RaiseClickEvent();
                                        break;
                                    }
                                case ButtonType.Left:
                                    {
                                        Left.RaiseClickEvent();
                                        break;
                                    }
                                case ButtonType.Ok:
                                    {
                                        Ok.RaiseClickEvent();
                                        break;
                                    }
                                case ButtonType.Right:
                                    {
                                        Right.RaiseClickEvent();
                                        break;
                                    }
                                case ButtonType.Up:
                                    {
                                        Up.RaiseClickEvent();
                                        break;
                                    }
                            }
                        }
                    }
                }
            }
            _eventButtons = null;
        }
        #endregion

    }
}
