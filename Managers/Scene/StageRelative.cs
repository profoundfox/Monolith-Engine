using Microsoft.Xna.Framework;

namespace Monolith.Managers
{
    public partial class StageManager
    {
        private void ProcessUpdateRelative(float deltaTime)
        {
            Engine.Node.ProcesssUpdate(deltaTime);
        }

        private void PhysicsUpdateRelative(float deltaTime)
        {
            Engine.Node.PhysicsUpdate(deltaTime);
        }

        private void SubmitCallRelative()
        {
            Engine.Node.SubmitCalls();
        }
    }
}