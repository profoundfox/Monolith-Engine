using Microsoft.Xna.Framework.Input;

namespace Monolith.Input
{
    public class ButtonArrays
    {
        public static readonly Buttons[] AllGamePadButtons = new[]
        {
            Buttons.A,
            Buttons.B,
            Buttons.X,
            Buttons.Y,
            Buttons.Back,
            Buttons.Start,
            Buttons.LeftShoulder,
            Buttons.RightShoulder,
            Buttons.LeftStick,
            Buttons.RightStick,
            Buttons.BigButton,
            Buttons.DPadUp,
            Buttons.DPadDown,
            Buttons.DPadLeft,
            Buttons.DPadRight
        };

        public static readonly MouseButton[] AllMouseButtons = new[]
        {
            MouseButton.Left,
            MouseButton.Middle,
            MouseButton.None,
            MouseButton.Right,
            MouseButton.XButton1,
            MouseButton.XButton2,
        };

    }
}