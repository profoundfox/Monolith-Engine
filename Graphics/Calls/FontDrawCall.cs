using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Attributes;
using Monolith.Graphics;

namespace Monolith.Graphics
{
    public sealed class FontDrawCall : Layered, IDrawCall
    {
        public required IFont Font { get; init; }
        public string Text { get; init; }

        public Vector2 Position { get; init; } = Vector2.Zero;
        public Color Color { get; init; } = Color.White;
        public float Rotation { get; init; } = 0f;
        public Vector2 Origin { get; init; } = Vector2.Zero;
        public Vector2 Scale { get; init; } = Vector2.One;
        public SpriteEffects Effects { get; init; } = SpriteEffects.None;

        public SpriteBatchConfig SpriteBatchConfig { get; init; } = SpriteBatchConfig.Default;

        public void Draw(SpriteBatch sb)
        {
            if (Font == null || string.IsNullOrEmpty(Text))
                return;
            
            Font.DrawString(
                sb,
                Text,
                Position,
                Color,
                Rotation,
                Origin,
                Scale,
                Effects,
                InternalDepth
            );
        }

    }
}