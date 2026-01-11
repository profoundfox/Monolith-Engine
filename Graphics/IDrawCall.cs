using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Graphics;
using Monolith.Structs;

namespace Monolith.Graphics
{
    internal interface IDrawCall
    {
        int Depth { get; }
        SpriteBatchConfig SpriteBatchConfig { get; }
        bool UseCamera { get; }
        void Draw(SpriteBatch sb);
    }
}