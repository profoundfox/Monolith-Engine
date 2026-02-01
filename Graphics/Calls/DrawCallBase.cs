using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Attributes;
using Monolith.Graphics;

namespace Monolith.Graphics
{
    public abstract class DrawCallBase : IDrawCall
    {
        public Vector2 Position { get; init; } = Vector2.Zero;
        public Color Color { get; init; } = Color.White;
        public float Rotation { get; init; } = 0f;
        public Vector2 Origin { get; init; } = Vector2.Zero;
        public Vector2 Scale { get; init; } = Vector2.One;
        public SpriteEffects Effects { get; init; } = SpriteEffects.None;

        public int Depth { get; init; } = 0;
        public Effect Effect { get; init; } = null;
        public bool UseCamera { get; init; } = true;
        public SpriteBatchConfig SpriteBatchConfig { get; init; } = SpriteBatchConfig.Default;

        int IDrawCall.Depth => Depth;
        SpriteBatchConfig IDrawCall.SpriteBatchConfig => SpriteBatchConfig;

        protected float LayerDepth
        {
            get
            {
                const int min = -100;
                const int max = 100;
                return 1f - (Depth - min) / (float)(max - min);
            }
        }

        public abstract void Draw(SpriteBatch sb);
    }
}
