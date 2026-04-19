using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Graphics;
using Monolith.Params;

namespace Monolith.Graphics
{
  public interface IDrawCall
  {
    CanvasParams Params { get; }

    SpriteBatchParams BatchParams { get; }

    int Depth { get; }

    void Draw(SpriteBatch sb);
  }
}
