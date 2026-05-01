using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Managers;
using Monolith.Params;

namespace Monolith.Hierarchy
{
  public class Camera2D : Node2D
  {
    [Export]
    public Vector2 Zoom { get; set; } = Vector2.One;

    [Export]
    public Rectangle Bounds
    {
      get
      {
        float width = Engine.Canvas.RenderTarget.Width / Zoom.X;
        float height = Engine.Canvas.RenderTarget.Height / Zoom.Y;

        float left = Transform.Global.Position.X - width * 0.5f;
        float top = Transform.Global.Position.Y - height * 0.5f;

        return new Rectangle(
            (int)left,
            (int)top,
            (int)width,
            (int)height
        );
      }
    }

    public Camera2D()
    {
      Engine.Canvas.SetMatrix(GetTransform());
    }

    /// <summary>
    /// Returns the camera transform matrix for SpriteBatch.Begin.
    /// Centers the camera so LocalPosition maps to the Canvas center.
    /// </summary>
    public Matrix GetTransform()
    {
      Vector2 CanvasCenter = new Vector2(Engine.Canvas.RenderTarget.Width / Zoom.X, Engine.Canvas.RenderTarget.Height / Zoom.Y) * 0.5f;

      Matrix transform = Matrix.CreateScale(Zoom.X, Zoom.Y, 1f)
          * Matrix.CreateRotationZ(Transform.Global.Rotation)
          * Matrix.CreateTranslation(new Vector3(-Transform.Global.Position, 0f))
          * Matrix.CreateTranslation(new Vector3(CanvasCenter, 0f));

      return transform;
    }

    public override void _Process(float delta)
    {
      base._Process(delta);

      Engine.Canvas.SetMatrix(GetTransform());
    }
  }
}
