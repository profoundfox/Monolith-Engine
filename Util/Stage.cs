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
        public void Update(GameTime gameTime);
        public void Draw(SpriteBatch spriteBatch);
    }

    public class Scene : IStage
    {
        public SceneConfig Config {get; set;}
        public Scene(SceneConfig config)
        {
            Config = config;
        }
        public virtual void OnEnter()
        {
            if (Config.DataPath != null)
                OgmoParser.FromFile(Config.DataPath, Config.TilemapTexturePath);
            
            Engine.Node.LoadNodes();
        }
        
        public virtual void OnExit()
        {
            Engine.Node.UnloadNodes();
        }
        public virtual void Update(GameTime gameTime)
        {
            Engine.Node.UpdateNodes(gameTime);
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Engine.Node.DrawNodes(spriteBatch);
        }
    }

}