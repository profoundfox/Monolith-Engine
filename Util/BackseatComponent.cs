using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monolith.Util
{
    public class BackseatComponent
    {
        public static List<BackseatComponent> BackseatComponentList = new();

        public BackseatComponent()
        {
            BackseatComponentList.Add(this);
        }
        
        public virtual void Load() {}
        
        public virtual void Update(GameTime gameTime) {}
        
        public virtual void Draw(SpriteBatch spriteBatch) {}

    }
}