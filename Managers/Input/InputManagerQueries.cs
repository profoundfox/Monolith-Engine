

using System.Numerics;
using Monolith.Input;

namespace Monolith.Managers
{   
    public partial class InputManager
    {

        /// <summary>
        /// Checks if an action is currently pressed.
        /// </summary>
        public bool IsActionPressed(string actionName)
        {
            if (!Binds.TryGetValue(actionName, out var actions))
                return false;

            foreach (var action in actions)
            {
                if (action.HasKey && Keyboard.IsKeyDown(action.Key)) return true;
                if (action.HasButton && CurrentGamePad.IsButtonDown(action.Button)) return true;
                if (action.HasMouseButton && Mouse.IsButtonDown(action.MouseButton)) return true;
            }

            return false;
        }

        public InputAction GetCurrentActionPressed()
        {
            foreach (var kvp in Binds)
            {
                foreach (var action in kvp.Value)
                {
                    if (action.HasKey && Keyboard.IsKeyDown(action.Key))
                        return action;

                    if (action.HasButton && CurrentGamePad.IsButtonDown(action.Button))
                        return action;

                    if (action.HasMouseButton && Mouse.IsButtonDown(action.MouseButton))
                        return action;
                }
            }

            return null;
        }


        /// <summary>
        /// Checks if an action was just pressed this frame.
        /// </summary>
        public bool IsActionJustPressed(string actionName)
        {
            if (!Binds.TryGetValue(actionName, out var actions))
                return false;

            foreach (var action in actions)
            {
                if (action.HasKey && Keyboard.WasKeyJustPressed(action.Key)) return true;
                if (action.HasButton && CurrentGamePad.WasButtonJustPressed(action.Button)) return true;
                if (action.HasMouseButton && Mouse.WasButtonJustPressed(action.MouseButton)) return true;
            }

            return false;
        }

        public InputAction GetCurrentActionJustPressed()
        {
            foreach (var kvp in Binds)
            {
                foreach (var action in kvp.Value)
                {
                    if (action.HasKey && Keyboard.WasKeyJustPressed(action.Key))
                        return action;

                    if (action.HasButton && CurrentGamePad.WasButtonJustPressed(action.Button))
                        return action;

                    if (action.HasMouseButton && Mouse.WasButtonJustPressed(action.MouseButton))
                        return action;
                }
            }

            return null;
        }
        

        /// <summary>
        /// Checks if an action was just released this frame.
        /// </summary>
        public bool IsActionJustReleased(string actionName)
        {
            if (!Binds.TryGetValue(actionName, out var actions))
                return false;

            foreach (var action in actions)
            {
                if (action.HasKey && Keyboard.WasKeyJustReleased(action.Key)) return true;
                if (action.HasButton && CurrentGamePad.WasButtonJustReleased(action.Button)) return true;
                if (action.HasMouseButton && Mouse.WasButtonJustReleased(action.MouseButton)) return true;
            }

            return false;
        }

        public InputAction GetCurrentActionJustReleased()
        {
            foreach (var kvp in Binds)
            {
                foreach (var action in kvp.Value)
                {
                    if (action.HasKey && Keyboard.WasKeyJustReleased(action.Key))
                        return action;

                    if (action.HasButton && CurrentGamePad.WasButtonJustReleased(action.Button))
                        return action;

                    if (action.HasMouseButton && Mouse.WasButtonJustReleased(action.MouseButton))
                        return action;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a signle axis value based on two actions.
        /// </summary>
        public int GetAxis(string negativeAction, string positiveAction)
        {
            bool negativePressed = IsActionPressed(negativeAction);
            bool positivePressed = IsActionPressed(positiveAction);

            if (negativePressed && positivePressed) return 0;
            if (negativePressed) return -1;
            if (positivePressed) return 1;
            return 0;
        }

        public Vector2 GetAxis(string xNegativeAction, string xPositiveAction, string yNegativeAction, string yPositiveAction)
        {
            int xAxis = GetAxis(xNegativeAction, xPositiveAction);
            int yAxis = GetAxis(yNegativeAction, yPositiveAction);
            
            return new Vector2(xAxis, yAxis);
        }
    }
}
