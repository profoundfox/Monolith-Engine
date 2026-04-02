
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Monolith.Geometry;

namespace Monolith.Nodes
{
   
    public class CollisionNode2D : Node2D
    {
        public CollisionShape2D CollisionShape { get => Get<CollisionShape2D>(); }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(
                    (int)GlobalPosition.X,
                    (int)GlobalPosition.Y,
                    (int)CollisionShape.Width,
                    (int)CollisionShape.Height
                );
            }
        }

        public CollisionNode2D() {}

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
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