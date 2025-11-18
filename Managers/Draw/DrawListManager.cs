using ConstructEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace ConstructEngine.Managers
{
    /// <summary>
    /// The manager responsible for batching, organizing, and drawing queued sprites
    /// across multiple draw layers in the correct order.
    /// </summary>
    public partial class DrawManager
    {
        public void DrawTilemaps(SpriteBatch spriteBatch)
        {
            foreach (Tilemap tilemap in Tilemaps)
            {
                tilemap.Draw(spriteBatch, tilemap.LayerDepth);
            }
        }
    }
}