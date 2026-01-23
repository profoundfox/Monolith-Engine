using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monolith.Graphics
{
    public sealed class BitmapFontDrawCall : DrawCallBase
    {
        public required BitmapFont Font { get; init; }
        public string Text { get; init; }

        public override void Draw(SpriteBatch sb)
        {
            if (Font == null || string.IsNullOrEmpty(Text))
                return;

            Vector2 cursor = Position;

            foreach (char c in Text)
            {
                if (c == '\n')
                {
                    cursor.X = Position.X;
                    cursor.Y += Font.CharHeight * Scale.Y;
                    continue;
                }

                if (!Font.TryGetSource(c, out Rectangle source))
                    continue;

                sb.Draw(
                    Font.Texture,
                    cursor,
                    source,
                    Color,
                    Rotation,
                    Origin,
                    Scale,
                    Effects,
                    LayerDepth
                );

                cursor.X += Font.CharWidth * Scale.X;
            }
        }
    }
}
