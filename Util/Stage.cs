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

        public void LoadMap(string path, string texturePath)
        {
            var tMaps = OgmoParser.LoadTilemapFromJson(path, texturePath);
            
            Engine.Node.LoadNodes(tMaps); 
        }

        public void LoadNodes(string path)
        {
            var nodes = OgmoParser.LoadNodes(path);

            Engine.Node.LoadNodes(nodes); 
        }

        public virtual void SubmitNodes() {}
        public virtual void OnEnter()
        {
            Engine.Node.LoadNodes();
        }
        public virtual void PhysicsUpdate(float deltaTime) {}
        public virtual void ProcessUpdate(float deltaTime) {}
        public virtual void SubmitCall() {}
        public virtual void OnExit() {}
    }
}