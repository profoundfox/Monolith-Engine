using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monolith.Graphics
{
    public sealed class FontDrawCall : DrawCallBase
    {
        public IFont Font { get; init; }
        public string Text { get; init; }
        public override void Draw(SpriteBatch sb)
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
                LayerDepth
            );
        }

    }
}