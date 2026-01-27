using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

using Monolith.Input;
using Monolith.Geometry;
using Microsoft.Xna.Framework.Input;

using System.Diagnostics;
using Monolith.Nodes;
using Microsoft.Xna.Framework;
using Monolith.Graphics;
using Monolith.IO;
using Monolith.Helpers;
using Microsoft.Xna.Framework.Graphics;



namespace Monolith
{
    /// <summary>
    /// Scene configuration
    /// </summary>
    public record class SceneConfig
    {
        public string DataPath {get; init; } = null;
        public Rectangle TilemapRegion {get; init; } = default;
        public string TilemapTexturePath {get; init; } = null;
    }
}
