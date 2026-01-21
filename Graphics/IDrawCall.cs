using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Graphics;
using Monolith.Attributes;

namespace Monolith.Graphics
{
    public interface IDrawCall
    {
        int Depth { get; }
        SpriteBatchConfig SpriteBatchConfig { get; }
        void Draw(SpriteBatch sb);
    }
}