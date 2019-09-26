using Lego.Ev3.Framework.Firmware;
using System.Collections.Generic;

namespace Lego.Ev3.Framework
{
    /// <summary>
    /// Port to all buttons on the main brick
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

        internal Buttons()
        {
            Up = new Button(ButtonType.Up);
            Ok = new Button(ButtonType.Ok);
            Down = new Button(ButtonType.Down);
            Right = new Button(ButtonType.Right);
            Left = new Button(ButtonType.Left);
            Back = new Button(ButtonType.Back);

        }


        #region internal Click Methods
        private List<Button> ClickButtonsInCommand;

        /// <summary>
        /// Store subscribed buttons in batch, buttons not always need to be polled
        /// and can be unsubscriped from click at any point in users program.
        /// </summary>
        /// <returns>byte length batchcommand</returns>
        internal ushort ClickBatchCommand(PayLoadBuilder payLoadBuilder, int index)
        {
            ushort byteLength = 0;
            ClickButtonsInCommand = new List<Button>();

            if (Up.OnClickSubscribed)
            {
                Up.ClickBatchCommand(payLoadBuilder, index);
                index++;
                byteLength++;
                ClickButtonsInCommand.Add(Up);
            }
            if (Ok.OnClickSubscribed)
            {
                Ok.ClickBatchCommand(payLoadBuilder, index);
                index++;
                byteLength++;
                ClickButtonsInCommand.Add(Ok);
            }
            if (Down.OnClickSubscribed)
            {
                Down.ClickBatchCommand(payLoadBuilder, index);
                index++;
                byteLength++;
                ClickButtonsInCommand.Add(Down);
            }
            if (Left.OnClickSubscribed)
            {
                Left.ClickBatchCommand(payLoadBuilder, index);
                index++;
                byteLength++;
                ClickButtonsInCommand.Add(Left);
            }
            if (Right.OnClickSubscribed)
            {
                Right.ClickBatchCommand(payLoadBuilder, index);
                index++;
                byteLength++;
                ClickButtonsInCommand.Add(Right);
            }
            if (Back.OnClickSubscribed)
            {
                Back.ClickBatchCommand(payLoadBuilder, index);
                index++;
                byteLength++;
                ClickButtonsInCommand.Add(Back);
            }

            if (byteLength == 0) ClickButtonsInCommand = null;
            return byteLength;
        }

        internal void ClickBatchCommandReturn(byte[] data)
        {
            if (data != null && ClickButtonsInCommand != null)
            {
                int c = ClickButtonsInCommand.Count;
                if (data.Length == c)
                {
                    for (int i = 0; i < c; i++)
                    {
                        bool clicked = data[i] == 1 ? true : false;
                        if (clicked)
                        {
                            Button button = ClickButtonsInCommand[i];
                            button.RaiseClickEvent();
                        }
                    }
                }
            }
            ClickButtonsInCommand = null;
        }
        #endregion

    }
}
