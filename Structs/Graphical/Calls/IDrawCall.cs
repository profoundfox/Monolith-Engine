using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Graphics;

namespace Monolith.Structs
{
    internal interface IDrawCall
    {
        int Depth { get; }
        SpriteBatchConfig SpriteBatchConfig { get; }
        bool UseCamera { get; }
        void Draw(SpriteBatch sb);
    }
}