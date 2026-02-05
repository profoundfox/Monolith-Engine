using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.IO;
using Monolith.Managers;
using Monolith.Nodes;


namespace Monolith.Util
{
    public interface IStage
    {
        public void OnEnter();
        public void OnExit();
        public void PhysicsUpdate(float deltaTime);
        public void ProcessUpdate(float deltaTime);
        public void SubmitCall();
    }
}