using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.IO;
using Monolith.Managers;
using Monolith.Nodes;


namespace Monolith.Util
{
    public interface IScene
    {
        public void Initialize();
        public void Load();
        public void Unload();
        public void Update(GameTime gameTime);
        public void Draw(SpriteBatch spriteBatch);
    }

    public class Scene : IScene
    {
        public SceneConfig Config {get; set;}
        public Scene(SceneConfig config)
        {
            Config = config;
        }
        public virtual void Initialize() {}
        public virtual void Load()
        {
            if (Config.DataPath != null)
                OgmoParser.FromFile(Config.DataPath, Config.TilemapTexturePath, Config.TilemapRegion);
            
            Engine.Node.LoadAll();
        }
        public virtual void Unload() {}
        public virtual void Update(GameTime gameTime) {}
        public virtual void Draw(SpriteBatch spriteBatch) {}
    }

}