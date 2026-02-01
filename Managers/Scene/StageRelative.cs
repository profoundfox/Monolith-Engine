using Microsoft.Xna.Framework;

namespace Monolith.Managers
{
    public partial class StageManager
    {
        private void LoadRelative()
        {
            Engine.Node.LoadNodes();
        }

        private void UpdateRelative(GameTime gameTime)
        {
            Engine.Node.UpdateNodes(gameTime);
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