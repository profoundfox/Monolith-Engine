
using Microsoft.Xna.Framework;
using Monolith.Managers;
using Monolith.Nodes;

namespace Monolith
{
    public class DebugTools
    {
        public static void DrawRegions()
        {
            foreach(Node node in NodeManager.AllInstances) node.DrawShapeHollow(Color.Red);
        }
    }
}