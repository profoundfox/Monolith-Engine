using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monolith.Graphics
{
    public sealed class Visual2D
    {
        /// <summary>
        /// Optional shader/effect override.
        /// </summary>
        public Effect Shader { get; set; } = null;

        /// <summary>
        /// SpriteBatch sprite effects (flip, etc).
        /// </summary>
        public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;

        /// <summary>
        /// Color modulation (tint).
        /// </summary>
        public Color Modulate { get; set; } = Color.White;

        /// <summary>
        /// Horizontal flip (convenience).
        /// </summary>
        public bool FlipH
        {
            get => (SpriteEffects & SpriteEffects.FlipHorizontally) != 0;
            set
            {
                SpriteEffects = value
                    ? SpriteEffects | SpriteEffects.FlipHorizontally
                    : SpriteEffects & ~SpriteEffects.FlipHorizontally;
            }
        }

        /// <summary>
        /// Vertical flip (convenience).
        /// </summary>
        public bool FlipY
        {
            get => (SpriteEffects & SpriteEffects.FlipVertically) != 0;
            set
            {
                SpriteEffects = value
                    ? SpriteEffects | SpriteEffects.FlipVertically
                    : SpriteEffects & ~SpriteEffects.FlipVertically;
            }
        }

        /// <summary>
        /// Whether this visual should be drawn.
        /// </summary>
        public bool Visible { get; set; } = true;

        /// <summary>
        /// Layer depth for SpriteBatch.
        /// </summary>
        public float LayerDepth { get; set; } = 0f;
    }
}
