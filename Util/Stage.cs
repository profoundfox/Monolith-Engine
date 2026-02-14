using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Helpers;
using Monolith.IO;
using Monolith.Managers;
using Monolith.Nodes;


namespace Monolith.Util
{
    public class Stage
    {

        public virtual void OnEnter() {}
        public virtual void PhysicsUpdate(float deltaTime) {}
        public virtual void ProcessUpdate(float deltaTime) {}
        public virtual void SubmitCall() {}
        public virtual void OnExit() {}
    }
}