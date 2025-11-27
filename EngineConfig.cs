using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

using Monolith.Input;
using Monolith.Region;
using Gum.Forms.Controls;
using Microsoft.Xna.Framework.Input;

using System.Diagnostics;
using Monolith.Nodes;
using Microsoft.Xna.Framework;
using Monolith.Graphics;
using Monolith.IO;



namespace Monolith
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
        public string Title { get; init; } = "My Game";

        /// <summary>
        /// The Parent directory of assets.
        /// Default: "Content"
        /// </summary>
        public string RootContentDirectory { get; init; } = "Content";

        /// <summary>
        /// Internal render width in pixels (before scaling).
        /// Default: 640
        /// </summary>
        public int RenderWidth { get; init; } = 640;

        /// <summary>
        /// Internal render height in pixels (before scaling).
        /// Default: 360
        /// </summary>
        public int RenderHeight { get; init; } = 360;

        /// <summary>
        /// Whether the game starts in maximised mode.
        /// Default: true
        /// </summary>
        public bool Maximised {get; init; } = true;

        /// <summary>
        /// Whether the game starts in fullscreen mode.
        /// Default: true
        /// </summary>
        public bool Fullscreen { get; init; } = true;

        /// <summary>
        /// Wheter the game starts in debug mode.
        /// </summary>
        public bool DebugMode { get; init; } = false;

        /// <summary>
        /// Whether the image is scaled using only integer values
        /// to preserve pixel accuracy.
        /// Default: true
        /// </summary>
        public bool IntegerScaling { get; init; } = true;

        /// <summary>
        /// Whether the user can resize the window border.
        /// Default: true
        /// </summary>
        public bool AllowUserResizing { get; init; } = true;

        /// <summary>
        /// Removes the window border/title bar entirely.
        /// Default: true
        /// </summary>
        public bool IsBorderless { get; init; } = true;

        /// <summary>
        /// If true, the mouse is visible
        /// Default: false
        /// </summary>
        public bool IsMouseVisible { get; init; } = false;

        /// <summary>
        /// If true, the engine updates as fast as possible.
        /// If false, updates are fixed to 60 per second.
        /// Default: false
        /// </summary>
        public bool IsFixedTimeStep { get; init; } = false;

        /// <summary>
        /// If true, V-Sync is enabled to prevent tearing.
        /// Default: true
        /// </summary>
        public bool SynchronizeWithVerticalRetrace { get; init; } = true;

        /// <summary>
        /// If true, exit is called once the escape key is pressed.
        /// Default: false
        /// </summary>
        public bool ExitOnEscape { get; init; } = false;

        /// <summary>
        /// Path to the default font (optional).
        /// </summary>
        public string FontPath { get; init; } = null;

        /// <summary>
        /// Path to a Gum UI project (optional).
        /// </summary>
        public string GumProject { get; init; } = null;

        /// <summary>
        /// Given content provider.
        /// Default: ContentPipelineLoader
        /// </summary>
        public IContentProvider ContentProvider { get; init; } = new ContentPipelineLoader();

        /// <summary>
        /// The main character type.
        /// Example: typeof(PlayerCharacter)
        /// </summary>
        public Type MainCharacterType { get; init; }
    }

    /// <summary>
    /// Scene configuration
    /// </summary>
    public record class SceneConfig
    {
        public string DataPath {get; init; } = null;
        public string TilemapRegion {get; init; } = null;
        public string TilemapTexturePath {get; init; } = null;
    }

    public record class NodeConfig
    {
        /// <summary>
        /// The object that the node is instantiated within.
        /// </summary>
        public required object Parent { get; init; }

        /// <summary>
        /// The shape that the object has.
        /// </summary>
        public required IRegionShape2D Shape { get; init; }

        /// <summary>
        /// The name of the node.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Values that come from the ogmo level dictionary.
        /// </summary>
        public Dictionary<string, object> Values { get; init; }
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
