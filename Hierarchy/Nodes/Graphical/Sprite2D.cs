using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Hierarchy;
using Monolith.Graphics;
using Monolith;
using System;
using Monolith.Params;

namespace Monolith.Hierarchy
{

  public class Sprite2D : Node2D
  {
    [Export]
    public MTexture Texture { get; set; }

    public Sprite2D() { }

    public override void SubmitCall()
    {
      if (Texture == null)
        return;

      Engine.Canvas.Call(new TextureDrawCall
      {
        Texture = Texture,
        Params = CanvasParams.Identity with
        {
          Position = Transform.Global.Position,
          Color = Visibility.Global.Modulate,
          Rotation = Transform.Global.Rotation,
          Origin = Texture.Center,
          Scale = Transform.Global.Scale,
          Effects = Material.Global.SpriteEffects,
        },
        Depth = Ordering.Global.Depth,
        BatchParams = SpriteBatchParams.Default with
        {
          Effect = Material.Global.Shader
        }
      });

    }
  }


}
