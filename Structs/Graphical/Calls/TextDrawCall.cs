using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Structs;

namespace Monolith.Structs
{
    public readonly struct TextDrawCall
    {
        public readonly SpriteFont Font;
        public readonly string Text;
        public readonly Vector2 Position;
        public readonly Color Color;
        public readonly float Rotation;
        public readonly Vector2 Origin;
        public readonly float Scale;
        public readonly SpriteEffects Effects;
        public readonly int Depth;
        public readonly SpriteBatchConfig SpriteBatchConfig;
        public readonly bool UseCamera;

        public TextDrawCall(
            SpriteFont font,
            string text,
            Vector2 position,
            Color color,
            int depth,
            SpriteBatchConfig config,
            bool useCamera = false,
            float rotation = 0f,
            Vector2 origin = default,
            float scale = 1f,
            SpriteEffects effects = SpriteEffects.None)
        {
            Font = font;
            Text = text;
            Position = position;
            Color = color;
            Depth = depth;
            SpriteBatchConfig = config;
            UseCamera = useCamera;
            Rotation = rotation;
            Origin = origin;
            Scale = scale;
            Effects = effects;
        }
    }
}