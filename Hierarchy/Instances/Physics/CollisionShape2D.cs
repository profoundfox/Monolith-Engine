using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Geometry;
using Monolith.Params;

namespace Monolith.Hierarchy
{
  public class CollisionShape2D : Node2D
  {
    public bool Disabled { get; set; }
    public bool OneWay { get; set; }
    public IShape2D Shape { get; set; }

    public int Width
    {
      get => Shape?.Size.Width ?? 0;
      set
      {
        if (Shape != null)
          Shape.Size = new Extent(value, Height);
      }
    }

    public int Height
    {
      get => Shape?.Size.Height ?? 0;
      set
      {
        if (Shape != null)
          Shape.Size = new Extent(Width, value);
      }
    }

    public CollisionShape2D() { }

    public override void OnEnter()
    {
      base.OnEnter();
    }

    public override void OnExit()
    {
      base.OnExit();
    }

    public override void PhysicsUpdate(float delta)
    {
      base.PhysicsUpdate(delta);
      //CheckOneWay();
    }

    private void CheckOneWay()
    {
      if (!OneWay || Shape == null)
        return;
    }

    public bool Intersects(CollisionShape2D other)
    {
      if (Disabled || other?.Shape == null || Shape == null)
        return false;

      return Shape.Intersect(other.Shape, Transform.Global.Position.ToPoint(), other.Transform.Global.Position.ToPoint());
    }

    public bool IntersectsAt(Vector2 offset, CollisionShape2D other)
    {
      if (Disabled || other?.Shape == null || other.Disabled || Shape == null)
        return false;

      return Shape.IntersectsAt(offset.ToPoint(), other.Shape, Transform.Global.Position.ToPoint(), other.Transform.Global.Position.ToPoint());
    }

    public bool Contains(Vector2 position)
    {
      if (!Disabled && Shape != null)
        return Shape.Contains(position.ToPoint(), Transform.Global.Position.ToPoint());

      return false;
    }


    public CollisionShape2D Clone()
    {
      IShape2D clonedShape = Shape?.Clone();

      return new CollisionShape2D()
      {
        Disabled = this.Disabled,
        OneWay = this.OneWay,
        Shape = clonedShape
      };
    }
  }
}
