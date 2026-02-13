
using System.Numerics;
using Monolith.Util;

namespace Monolith.Nodes
{
    public record class DynamicBodyConfig : PhysicsBodyConfig {}
    public class DynamicBody2D : Node2D
    {
        public Vector2 Velocity;

        public DynamicBody2D(DynamicBodyConfig cfg) : base(cfg) {}

        public override void Load()
        {
            base.Load();
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

        public override void Unload()
        {
            base.Unload();
        }
    }
}