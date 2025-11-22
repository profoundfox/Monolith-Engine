using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ConstructEngine.Directory;
using ConstructEngine.Nodes;


namespace ConstructEngine.Util
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

            Node.LoadObjects();

        }
        public virtual void Unload() {}
        public virtual void Update(GameTime gameTime) {}
        public virtual void Draw(SpriteBatch spriteBatch) {}
    }

}