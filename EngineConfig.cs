using System.Collections.Generic;
using ConstructEngine.Input;
using Microsoft.Xna.Framework.Input;


namespace ConstructEngine
{
    /// <summary>
    /// Global engine configuration container.
    /// </summary>
    public record class EngineConfig
    {
        /// <summary>
        /// Title of the game window.
        /// Default: "My Game"
        /// </summary>
        public string Title = "My Game";

        /// <summary>
        /// The root directory of assets.
        /// Default: "Content"
        /// </summary>
        public string RootContentDirectory = "Content";

        /// <summary>
        /// Internal render width in pixels (before scaling).
        /// Default: 640
        /// </summary>
        public int VirtualWidth = 640;

        /// <summary>
        /// Internal render height in pixels (before scaling).
        /// Default: 360
        /// </summary>
        public int VirtualHeight = 360;

        /// <summary>
        /// Whether the game starts in fullscreen mode.
        /// Default: true
        /// </summary>
        public bool Fullscreen = true;

        /// <summary>
        /// Whether the image is scaled using only integer values
        /// to preserve pixel accuracy.
        /// Default: true
        /// </summary>
        public bool IntegerScaling = true;

        /// <summary>
        /// Whether the user can resize the window border.
        /// Default: true
        /// </summary>
        public bool AllowUserResizing = true;

        /// <summary>
        /// Removes the window border/title bar entirely.
        /// Default: true
        /// </summary>
        public bool IsBorderless = true;

        /// <summary>
        /// If true, the mouse is visible
        /// Default: false
        /// </summary>
        public bool IsMouseVisible = false;

        /// <summary>
        /// If true, the engine updates as fast as possible.
        /// If false, updates are fixed to 60 per second.
        /// Default: false
        /// </summary>
        public bool IsFixedTimeStep = false;

        /// <summary>
        /// If true, V-Sync is enabled to prevent tearing.
        /// Default: true
        /// </summary>
        public bool SynchronizeWithVerticalRetrace = true;

        /// <summary>
        /// If true, exit is called once the escape key is pressed.
        /// Default: false
        /// </summary>
        public bool ExitOnEscape = false;

        /// <summary>
        /// Path to the default font (optional).
        /// </summary>
        public string FontPath = null;

        /// <summary>
        /// Path to a Gum UI project (optional).
        /// </summary>
        public string GumProject = null;

        /// <summary>
        /// Returns a config instance with default values.
        /// </summary>
        public static EngineConfig BaseConfig => new EngineConfig();
    }



    public static class DefaultInput
    {
        public static Dictionary<string, List<InputAction>> Binds = new()
        {
            {"MoveLeft", new List<InputAction> { new InputAction(Keys.Left), new InputAction(Buttons.DPadLeft) }},
            {"MoveRight", new List<InputAction> { new InputAction(Keys.Right), new InputAction(Buttons.DPadRight) }},
            {"Jump", new List<InputAction> { new InputAction(Keys.Z), new InputAction(Buttons.A) }},
            {"Attack", new List<InputAction> { new InputAction(Keys.X), new InputAction(Buttons.Y), new InputAction(MouseButton.Left) }},
            {"Pause", new List<InputAction> { new InputAction(Keys.Escape), new InputAction(Buttons.Start) }},
            {"Back", new List<InputAction> { new InputAction(Keys.X), new InputAction(Buttons.B) }}
        };
    }


}
