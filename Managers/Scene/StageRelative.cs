using Microsoft.Xna.Framework;

namespace Monolith.Managers
{
    public partial class StageManager
    {
        private void LoadRelative()
        {
            
        }

        private void ProcessUpdateRelative(float deltaTime)
        {
            Engine.Node.ProcessUpdateNodes(deltaTime);
        }

        private void PhysicsUpdateRelative(float deltaTime)
        {
            Engine.Node.PhysicsUpdateNodes(deltaTime);
        }

        private void SubmitCallRelative()
        {
            Engine.Node.SubmitCallNodes();
        }

        private void UnloadRelative()
        {
            Engine.Node.UnloadNodes();
        }
    }
}