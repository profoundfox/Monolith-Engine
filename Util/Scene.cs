using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net.Http.Headers;
using ConstructEngine.Components;
using ConstructEngine.Graphics;
using ConstructEngine.Area;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ConstructEngine.Components;
using ConstructEngine.Directory;
using Gum.Forms.Controls;
using ConstructEngine.UI;

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

        }
        public virtual void Unload() {}
        public virtual void Update(GameTime gameTime) {}
        public virtual void Draw(SpriteBatch spriteBatch) {}
    }

}