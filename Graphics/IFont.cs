using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monolith.Graphics
{
    public interface IFont
    {
        /// <summary>
        /// Measures the size of the given text.
        /// </summary>
        /// <param name="text">Text to measure.</param>
        /// <returns>Width and height as a Vector2.</returns>
        Vector2 MeasureString(string text);

        /// <summary>
        /// Draws text at the given position with color.
        /// </summary>
        void DrawString(SpriteBatch spriteBatch, string text, Vector2 position, Color color);

        /// <summary>
        /// Draws text with full options: rotation, origin, scale, sprite effects, and layer depth.
        /// </summary>
        void DrawString(SpriteBatch spriteBatch, string text, Vector2 position, Color color,
                        float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth);
    }
}