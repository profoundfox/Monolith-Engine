using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Geometry;
using Monolith.Params;

namespace Monolith.Hierarchy
{
  public class CollisionShape2D : Node2D
  {
    [Export]
    public bool Disabled { get; set; }
    [Export]
    public bool OneWay { get; set; }
    [Export]
    public IShape2D Shape { get; set; }

    [Export]
    public int Width
    {
      get => Shape?.Size.Width ?? 0;
      set
      {
        if (Shape != null)
          Shape.Size = new Extent(value, Height);
      }
    }

    [Export]
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

    public override void _EnterTree()
    {
      base._EnterTree();
    }

    public override void _ExitTree()
    {
      base._ExitTree();
    }

    public override void _PhysicsUpdate(float delta)
    {
      base._PhysicsUpdate(delta);
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
