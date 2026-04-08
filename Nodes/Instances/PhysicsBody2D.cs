
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Monolith.Geometry;
using Monolith.Tools;

namespace Monolith.Nodes
{
   
    public class PhysicsBody2D : CollisionNode2D, IHashAble
    {
        public PhysicsBody2D() {}

        public override void OnEnter()
        {
            base.OnEnter();

            if (!CollisionShapes.IsEmpty())
                Engine.Physics.RegisterBody(this);

            OnChildAdded += (node) =>
            {
                if (node is CollisionShape2D)
                    Engine.Physics.RegisterBody(this);
            };

            OnTransformChanged += (transform) =>
            {   
                Engine.Physics.NotifyMoved(this);
            };
        }

        public override void OnExit()
        {
            Engine.Physics.UnregisterBody(this);
            
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