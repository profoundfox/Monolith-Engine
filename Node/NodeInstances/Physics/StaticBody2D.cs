using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Monolith.Geometry;
using Monolith.Managers;

namespace Monolith.Nodes
{
    public record class StaticBodyConfig : PhysicsBodyConfig {}
    
    public class StaticBody2D : PhysicsBody2D
    {
        public StaticBody2D(StaticBodyConfig cfg) : base(cfg) {}

        public override void PhysicsUpdate(float delta)
        {
            base.PhysicsUpdate(delta);
        }

    }

}