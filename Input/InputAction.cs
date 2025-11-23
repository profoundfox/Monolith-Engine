using Monolith.Input;
using Microsoft.Xna.Framework.Input;

namespace Monolith.Input
{
    public enum InputType
    {
        None,
        Key,
        Button,
        MouseButton
    }

    public class InputAction
    {
        public InputType Type { get;}
        public Keys Key { get; }
        public Buttons Button { get; }
        public MouseButton MouseButton { get; }

        public bool HasKey => Type == InputType.Key;
        public bool HasButton => Type == InputType.Button;
        public bool HasMouseButton => Type == InputType.MouseButton;

        public InputAction(Keys key)
        {
            Type = InputType.Key;
            Key = key;
        }

        public InputAction(Buttons button)
        {
            Type = InputType.Button;
            Button = button;
        }

        public InputAction(MouseButton mouseButton)
        {
            Type = InputType.MouseButton;
            MouseButton = mouseButton;
        }

        public InputAction Clone() => Type switch
        {
            InputType.Key => new InputAction(Key),
            InputType.Button => new InputAction(Button),
            InputType.MouseButton => new InputAction(MouseButton),
            _ => new InputAction(Keys.None)
        };
    }
}