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

namespace ConstructEngine.Util
{
    public class Scene
    {
        public interface IScene
        {
            public void Initialize();
            public void Load();
            public void Unload();
            public void Update(GameTime gameTime);
            public void Draw(SpriteBatch spriteBatch);

        }
        
        
        
    }

}