using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Params;

namespace Monolith.Graphics
{
  public sealed class TextureDrawCall : Layered, IDrawCall
  {
    public required MTexture Texture { get; init; }
    public Rectangle? SourceRectangle { get; init; }

    public CanvasParams Params { get; set; } = CanvasParams.Identity;

    public SpriteBatchParams BatchParams { get; init; } = SpriteBatchParams.Default;

    public void Draw(SpriteBatch sb)
    {
      if (Texture?.Texture == null)
        return;

      Rectangle src =
          SourceRectangle
          ?? Texture.SourceRectangle
          ?? new Rectangle(0, 0, Texture.Texture.Width, Texture.Texture.Height);

      sb.Draw(
          Texture.Texture,
          Params.Position,
          src,
          Params.Color,
          Params.Rotation,
          Params.Origin,
          Params.Scale,
          Params.Effects,
          InternalDepth
      );
    }
  }
}
