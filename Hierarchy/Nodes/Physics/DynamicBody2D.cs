
using System;
using Microsoft.Xna.Framework;
using Monolith.Params;
using Monolith.Util;

namespace Monolith.Hierarchy
{
  public class DynamicBody2D : PhysicsBody2D
  {
    [Export]
    public Vector2 Velocity;

    public DynamicBody2D() { }

    public override void _EnterTree()
    {
      base._EnterTree();
    }

    public override void _PhysicsUpdate(float delta)
    {
      base._PhysicsUpdate(delta);

      LocalPosition += Velocity * delta;
    }

    public override void _Process(float delta)
    {
      base._Process(delta);
    }

    public override void _SubmitCall()
    {
      base._SubmitCall();
    }

    public override void _ExitTree()
    {
      base._EnterTree();
    }
  }
}
