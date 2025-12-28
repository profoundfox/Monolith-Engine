using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monolith.Structs
{
    internal abstract class DrawCallBase : IDrawCall
    {
        public Vector2 Position;
        public Color Color;
        public float Rotation;
        public Vector2 Origin;
        public Vector2 Scale;
        public SpriteEffects Effects;
        public int Depth;
        public Effect Effect;
        public bool UseCamera;
        public SpriteBatchConfig SpriteBatchConfig;

        int IDrawCall.Depth => Depth;
        SpriteBatchConfig IDrawCall.SpriteBatchConfig => SpriteBatchConfig;
        bool IDrawCall.UseCamera => UseCamera;

        public abstract void Draw(SpriteBatch sb);
    }
}