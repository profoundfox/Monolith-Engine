using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Graphics;

namespace Monolith.Structs
{
    internal sealed class TextureDrawCall : DrawCallBase
    {
        public MTexture Texture;
        public Rectangle? SourceRectangle;

        public override void Draw(SpriteBatch sb)
        {
            if (Texture == null) return;

            Rectangle src = SourceRectangle ?? new Rectangle(0, 0, Texture.Width, Texture.Height);
            int minDepth = -100;
            int maxDepth = 100;
            float layerDepth = 1.0f - ((float)(Depth - minDepth) / (maxDepth - minDepth));

            sb.Draw(
                Texture.Texture,
                Position,
                src,
                Color,
                Rotation,
                Origin,
                Scale,
                Effects,
                layerDepth
            );
        }
    }

    public readonly struct TextureDrawParams
    {
        public MTexture Texture { get; init; }
        public Vector2 Position { get; init; }
        public Rectangle? SourceRectangle { get; init; }
        public Color Color { get; init; }
        public float Rotation { get; init; }
        public Vector2 Origin { get; init; }
        public Vector2 Scale { get; init; }
        public SpriteEffects Effects { get; init; }
        public int Depth { get; init; }
        public Effect Effect { get; init; }
        public bool UseCamera { get; init; }
        public SpriteBatchConfig SpriteBatchConfig { get; init; }

        public TextureDrawParams(
            MTexture texture,
            Vector2 position,
            Color? color = null,
            float rotation = 0f,
            Vector2? origin = null,
            Vector2? scale = null,
            Rectangle? sourceRectangle = null,
            SpriteEffects effects = SpriteEffects.None,
            int depth = 0,
            Effect effect = null,
            bool useCamera = true,
            SpriteBatchConfig? spriteBatchConfig = null)
        {
            if (texture == null || texture.Texture == null)
                throw new ArgumentNullException(nameof(texture));

            Texture = texture;
            Position = position;
            SourceRectangle = sourceRectangle;

            if (color != null)
                Color = color.Value;
            else
                Color = Color.White;

            Rotation = rotation;

            if (origin != null)
                Origin = origin.Value;
            else
                Origin = Vector2.Zero;

            if (scale != null)
                Scale = scale.Value;
            else
                Scale = Vector2.One;

            Effects = effects;
            Depth = depth;
            Effect = effect;
            UseCamera = useCamera;

            if (spriteBatchConfig != null)
                SpriteBatchConfig = spriteBatchConfig.Value;
            else
                SpriteBatchConfig = SpriteBatchConfig.Default;
        }
    }
}