using System.Collections.Generic;
using Monolith.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace Monolith.Managers
{
    /// <summary>
    /// The manager responsible for batching, organizing, and drawing queued sprites
    /// across multiple draw layers in the correct order.
    /// </summary>
    public partial class DrawManager
    {
        /// <summary>
        /// A list of tilemaps that will be drawn alongside queued draw calls.
        /// </summary>
        public List<Tilemap> Tilemaps = new List<Tilemap>();

        public void DrawTilemaps(SpriteBatch spriteBatch)
        {
            foreach (Tilemap tilemap in Tilemaps)
            {
                tilemap.Draw();
            }
        }
    }
}