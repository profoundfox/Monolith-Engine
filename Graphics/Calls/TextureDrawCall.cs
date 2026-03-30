using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Attributes;

namespace Monolith.Graphics
{
    public sealed class TextureDrawCall : Layered, IDrawCall
    {
        public required MTexture Texture { get; init; }
        public Rectangle? SourceRectangle { get; init; }

        public Vector2 Position { get; init; } = Vector2.Zero;
        public Color Color { get; init; } = Color.White;
        public float Rotation { get; init; } = 0f;
        public Vector2 Origin { get; init; } = Vector2.Zero;
        public Vector2 Scale { get; init; } = Vector2.One;
        public SpriteEffects Effects { get; init; } = SpriteEffects.None;

        public SpriteBatchConfig SpriteBatchConfig { get; init; } = SpriteBatchConfig.Default;

        public void Draw(SpriteBatch sb)
        {
            if (Texture?.Texture == null)
                return;

            Rectangle src =
                SourceRectangle
                ?? Texture.SourceRectangle
                ?? new Rectangle(0, 0, Texture.Texture.Width, Texture.Texture.Height);

            sb.Draw(
                Texture.Texture,
                Position,
                src,
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