
using System;
using System.Numerics;
using Monolith.Util;

namespace Monolith.Nodes
{
    public class DynamicBody2D : PhysicsBody2D
    {
        public Vector2 Velocity;

        public DynamicBody2D() {}

        public override void Load()
        {
            base.Load();
        }

        public override void PhysicsUpdate(float delta)
        {
            base.PhysicsUpdate(delta);

            Position += Velocity * delta;
        }

        public override void ProcessUpdate(float delta)
        {
            base.ProcessUpdate(delta);
        }

        public override void SubmitCall()
        {
            base.SubmitCall();
        }

        public override void Unload()
        {
            base.Unload();
        }
    }
}