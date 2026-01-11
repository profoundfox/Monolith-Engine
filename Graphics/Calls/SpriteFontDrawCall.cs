using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monolith.Graphics
{
    public sealed class SpritFontDrawCall : DrawCallBase
    {
        public SpriteFont Font { get; init; }
        public string Text { get; init; }

        public override void Draw(SpriteBatch sb)
        {
            if (Font == null || string.IsNullOrEmpty(Text))
                return;

            sb.DrawString(
                Font,
                Text,
                Position,
                Color,
                Rotation,
                Origin,
                Scale,
                Effects,
                LayerDepth
            );
        }
    }
}
