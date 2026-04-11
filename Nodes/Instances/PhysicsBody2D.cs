
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Monolith.Geometry;
using Monolith.Tools;

namespace Monolith.Nodes
{
    ///<summary>
    /// The class for all bodies which posses phsyics and are required to be queued from the server.
    ///</summary>
    public class PhysicsBody2D : CollisionNode2D, IHashAble
    {
        public PhysicsBody2D() {}

        public override void OnEnter()
        {
            OnChildAdded += (node) =>
            {
                if (node is not CollisionShape2D)
                    return;
                
                Engine.Physics.RegisterBody(this);
                Engine.Physics.NotifyMoved(this);
            };

            OnTransformChanged += (transform) =>
            {   
                Engine.Physics.NotifyMoved(this);
            };
    

            base.OnEnter();

            if (!CollisionShapes.IsEmpty())
            {
                Engine.Physics.RegisterBody(this);
                Engine.Physics.NotifyMoved(this);
            }
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
