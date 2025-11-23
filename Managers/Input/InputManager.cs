using System;
using System.Collections.Generic;
using System.Linq;
using Monolith.Helpers;
using Monolith.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Monolith.Managers
{
    public partial class InputManager
    {
        public KeyboardInfo Keyboard { get; private set; }
        public MouseInfo Mouse { get; private set; }
        public GamePadInfo[] GamePads { get; private set; }
        public GamePadInfo CurrentGamePad { get; private set; }
        public Dictionary<string, List<InputAction>> Binds { get; private set; } = new Dictionary<string, List<InputAction>>();
        public Dictionary<string, List<InputAction>> InitialBinds { get; private set; } = new Dictionary<string, List<InputAction>>();


        public InputManager()
        {
            Keyboard = new KeyboardInfo();
            Mouse = new MouseInfo();

            GamePads = new GamePadInfo[4];
            for (int i = 0; i < 4; i++)
                GamePads[i] = new GamePadInfo((PlayerIndex)i);

            CurrentGamePad = GamePads[0];
        }

        /// <summary>
        /// Updates the keyboard, mouse, and all gamepads.
        /// Also updates the currently active gamepad.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            Keyboard.Update();
            Mouse.Update();

            foreach (var pad in GamePads)
                pad.Update(gameTime);

            CurrentGamePad = GamePads[0];
        }
    }
}
