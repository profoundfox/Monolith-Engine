using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monolith.Graphics
{
    /// <summary>
    /// Represents a texture or a rectangular region within a texture.
    /// Can be used for full textures or subregions (sprites) of a texture.
    /// </summary>
    public class MTexture
    {
        /// <summary>
        /// The source texture.
        /// </summary>
        public Texture2D Texture { get; private set; }

        /// <summary>
        /// The rectangular region within the texture. If null, the full texture is used.
        /// </summary>
        public Rectangle? SourceRectangle { get; private set; }

        /// <summary>
        /// Width of the texture region or full texture.
        /// </summary>
        public int Width => SourceRectangle?.Width ?? Texture.Width;

        /// <summary>
        /// Height of the texture region or full texture.
        /// </summary>
        public int Height => SourceRectangle?.Height ?? Texture.Height;

        /// <summary>
        /// Creates a wrapper for a full texture.
        /// </summary>
        public MTexture(Texture2D texture)
        {
            Texture = texture ?? throw new ArgumentNullException(nameof(texture));
            SourceRectangle = null; // Full texture by default
        }

        /// <summary>
        /// Creates a wrapper for a specific region within a texture.
        /// </summary>
        public MTexture(Texture2D texture, Rectangle region)
        {
            Texture = texture ?? throw new ArgumentNullException(nameof(texture));
            SourceRectangle = region;
        }

        /// <summary>
        /// Convenience constructor using x, y, width, height.
        /// </summary>
        public MTexture(Texture2D texture, int x, int y, int width, int height)
            : this(texture, new Rectangle(x, y, width, height)) { }

        /// <summary>
        /// Draws the texture at the specified position.
        /// </summary>
        public void Draw(Vector2 position, Color color, float layerDepth = 0f)
        {
            Draw(position, color, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, layerDepth);
        }

        public void Draw(Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            Draw(position, color, rotation, origin, new Vector2(scale, scale), effects, layerDepth);
        }

        public void Draw(Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            Engine.DrawManager.Draw(new Managers.DrawParams
            (
                texture: Texture,
                position: position,
                source: SourceRectangle,
                color: color,
                rotation: rotation,
                origin: origin,
                scale: scale,
                effects: effects,
                layerDepth: layerDepth
            ));
        }
    }
}
