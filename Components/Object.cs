using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using ConstructEngine.Components.Entity;
using ConstructEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ConstructEngine.Objects
{
    public class ConstructObject : ConstructObject.IObject
    {
        public static List<ConstructObject> ObjectList { get; set; } = new();
        
        public Rectangle Rectangle { get; set; }
        
        public SceneManager CurrentSceneManager { get; set; }
        
        public string Name { get; set; }
        
        public Entity Player { get; set; }
        
        public Dictionary<string, object> Values { get; set; }
        
        public ConstructObject()
        {
            ObjectList.Add(this);
        }

        public virtual void Load() {}
        
        public virtual void Update(GameTime gameTime) {}
        
        public virtual void Draw(SpriteBatch spriteBatch) {}
        
        public interface IObject
        {
            void Load();
            void Update(GameTime gameTime);
        }

        public static void LoadObjects() => ConstructObject.ObjectList.ForEach(o => o.Load());
        public static void DrawObjects(SpriteBatch spriteBatch) => ConstructObject.ObjectList.ForEach(o => o.Draw(spriteBatch));
        public static void UpdateObjects(GameTime gameTime) => ConstructObject.ObjectList.ForEach(o => o.Update(gameTime));
    }
}