using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Graphics;
using Monolith.Attributes;

namespace Monolith.Graphics
{
    public interface IDrawCall
    {
        Vector2 Position { get; }
        Color Color { get; }
        float Rotation { get; }
        Vector2 Origin { get; }
        Vector2 Scale { get; }
        SpriteEffects Effects { get; }

        int Depth { get; }
        SpriteBatchConfig SpriteBatchConfig { get; }

        void Draw(SpriteBatch sb);
    }
}