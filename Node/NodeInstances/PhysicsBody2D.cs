
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Monolith.Geometry;

namespace Monolith.Nodes
{
   
    public class PhysicsBody2D : Node2D, IHashAble
    {
        public CollisionShape2D CollisionShape { get => Get<CollisionShape2D>(); }

        public Rectangle Bounds => CollisionShape.Shape.BoundingBox;

        public PhysicsBody2D() {}

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