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

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void PhysicsUpdate(float delta)
        {
            base.PhysicsUpdate(delta);
        }

    }

}
