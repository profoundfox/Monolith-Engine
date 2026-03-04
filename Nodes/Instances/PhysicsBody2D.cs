
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Monolith.Geometry;

namespace Monolith.Nodes
{
   
    public class PhysicsBody2D : CollisionNode2D, IHashAble
    {
        public PhysicsBody2D() {}

        public override void OnEnter()
        {
            base.OnEnter();

            OnChildAdded += (node) =>
            {
                Engine.Physics.RegisterBody(this);
                OnTransformChanged += (transform) =>
                {
                    Engine.Physics.NotifyMoved(this);
                };
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