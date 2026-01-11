using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Graphics;

namespace Monolith.Graphics
{
    public sealed class TextureDrawCall : DrawCallBase
    {
        public MTexture Texture { get; init; }
        public Rectangle? SourceRectangle { get; init; }

        public override void Draw(SpriteBatch sb)
        {
            if (Texture?.Texture == null)
                return;

            Rectangle src = SourceRectangle ??
                new Rectangle(0, 0, Texture.Width, Texture.Height);

            sb.Draw(
                Texture.Texture,
                Position,
                src,
                Color,
                Rotation,
                Origin,
                Scale,
                Effects,
                DepthToLayer(Depth)
            );
        }

        private static float DepthToLayer(int depth)
        {
            const int min = -100;
            const int max = 100;
            return 1f - (depth - min) / (float)(max - min);
        }
    }
}
