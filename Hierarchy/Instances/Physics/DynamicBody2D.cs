
using System;
using Microsoft.Xna.Framework;
using Monolith.Util;

namespace Monolith.Hierarchy
{
  public class DynamicBody2D : PhysicsBody2D
  {
    public Vector2 Velocity;

    public DynamicBody2D() { }

    public override void OnEnter()
    {
      base.OnEnter();
    }

    public override void PhysicsUpdate(float delta)
    {
      base.PhysicsUpdate(delta);

      LocalPosition += Velocity * delta;
    }

    public override void ProcessUpdate(float delta)
    {
      base.ProcessUpdate(delta);
    }

    public override void SubmitCall()
    {
      base.SubmitCall();
    }

    public override void OnExit()
    {
      base.OnEnter();
    }
  }
}
