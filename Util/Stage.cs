using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.IO;
using Monolith.Managers;
using Monolith.Nodes;


namespace Monolith.Util
{
    public interface IStage
    {
        public void Load();
        public void Unload();
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
        public virtual void Load()
        {
            if (Config.DataPath != null)
                OgmoParser.FromFile(Config.DataPath, Config.TilemapTexturePath, Config.TilemapRegion);
            
            Engine.Node.LoadNodes();
        }
        
        public virtual void Unload()
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