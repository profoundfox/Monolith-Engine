using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Monolith.Geometry;
using Monolith.Managers;

namespace Monolith.Hierarchy
{
  public class StaticBody2D : PhysicsBody2D
  {
    public StaticBody2D() { }

    public override void _EnterTree()
    {
      base._EnterTree();
    }

    public override void _PhysicsUpdate(float delta)
    {
      base._PhysicsUpdate(delta);
    }

  }

}
