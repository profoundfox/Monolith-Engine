
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Microsoft.Xna.Framework;
using Monolith.Geometry;
using Monolith.Nodes;

namespace Monolith.Util
{
    public class Stage
    {
        public virtual void OnEnter() {}
        public virtual void PhysicsUpdate(float deltaTime)
        {
            Engine.Tree.PhysicsUpdate(deltaTime);
            Engine.Node.PhysicsUpdate(deltaTime);
        }
        public virtual void ProcessUpdate(float deltaTime)
        {
            Engine.Tree.ProcesssUpdate(deltaTime);
            Engine.Node.ProcesssUpdate(deltaTime);
        }
        public virtual void SubmitCall()
        {
            Engine.Tree.SubmitCalls();
            Engine.Node.SubmitCalls();
        }
        public virtual void OnExit() {}
    }
}