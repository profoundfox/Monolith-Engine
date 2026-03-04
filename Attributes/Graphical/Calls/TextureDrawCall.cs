using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Graphics;

namespace Monolith.Attributes
{
    public sealed class TextureDrawCall : DrawCallBase
    {
        public required MTexture Texture { get; init; }
        public Rectangle? SourceRectangle { get; init; }

        public override void Draw(SpriteBatch sb)
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
                LayerDepth
            );

            Console.WriteLine(src);

        }
    }
}
