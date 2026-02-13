
using System;

namespace Monolith.Nodes
{
    public record class PhysicsBodyConfig : SpatialNodeConfig
    {
        public CollisionShape2D CollisionShape { get; set; }
        public bool OneWay { get; set; }
        public bool Disabled { get; set; }
        public override Type NodeType => typeof(PhysicsBody2D);
    }
    
    public class PhysicsBody2D : Node2D
    {
        public CollisionShape2D CollisionShape { get; set; }

        public PhysicsBody2D(PhysicsBodyConfig cfg) : base(cfg)
        {
            CollisionShape = cfg.CollisionShape;
            CollisionShape.OneWay = cfg.OneWay;
            CollisionShape.Disabled = cfg.Disabled;
        }

        public override void Load()
        {
            base.Load();
        }

        public override void Unload()
        {
            base.Unload();
        }

        public override void PhysicsUpdate(float delta)
        {
            base.PhysicsUpdate(delta);
        }

        public override void ProcessUpdate(float delta)
        {
            base.ProcessUpdate(delta);
        }

        public override void SubmitCall()
        {
            base.SubmitCall();
        }
    }
}