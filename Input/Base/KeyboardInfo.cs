using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace Monolith.Input
{
    public class KeyboardInfo
    {
        public KeyboardState PreviousState { get; private set; }
        public KeyboardState CurrentState { get; private set; }

        public KeyboardInfo()
        {
            PreviousState = new KeyboardState();
            CurrentState = Keyboard.GetState();
        }
        public void Update()
        {
            PreviousState = CurrentState;
            CurrentState = Keyboard.GetState();
        }

        /// <summary>
        /// Returns the first currently pressed key, or Keys.None if no key is pressed.
        /// </summary>
        public Keys GetFirstKeyDown()
        {
            return CurrentState.GetPressedKeys().FirstOrDefault();
        }

        /// <summary>
        /// Returns the first currently released key, or Keys.None if no key is released.
        /// </summary>
        public Keys GetFirstKeyUp()
        {
            var pressed = CurrentState.GetPressedKeys();
            return Enum.GetValues(typeof(Keys))
                    .Cast<Keys>()
                    .FirstOrDefault(k => !pressed.Contains(k));
        }

        /// <summary>
        /// Returns the first key that was just pressed, or Keys.None if no key is pressed.
        /// </summary>
        public Keys GetFirstKeyJustPressed()
        {
            return CurrentState
                .GetPressedKeys()
                .FirstOrDefault(key => PreviousState.IsKeyUp(key));
        }

        /// <summary>
        /// Returns the first key that was just released, or Keys.None if no key is released.
        /// </summary>
        public Keys GetFirstKeyJustReleased()
        {
            return PreviousState
                .GetPressedKeys()
                .FirstOrDefault(key => CurrentState.IsKeyUp(key));
        }

        public bool IsKeyDown(Keys key)
        {
            return CurrentState.IsKeyDown(key);
        }

        public bool IsKeyUp(Keys key)
        {
            return CurrentState.IsKeyUp(key);
        }

        public bool WasKeyJustPressed(Keys key)
        {
            return CurrentState.IsKeyDown(key) && PreviousState.IsKeyUp(key);
        }

        public bool WasKeyJustReleased(Keys key)
        {
            return CurrentState.IsKeyUp(key) && PreviousState.IsKeyDown(key);
        }
    }
}