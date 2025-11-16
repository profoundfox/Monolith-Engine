using System.Collections.Generic;
using ConstructEngine.Components.Entity;
using ConstructEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ConstructEngine.Objects
{
    public interface IObject
    {
        void Load();
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
    }

    public class ConstructObject : IObject
    {
        public static List<IObject> ObjectList { get; private set; } = new();
        public static Dictionary<string, IObject> ObjectDict { get; private set; } = new();

        public Rectangle Rectangle { get; set; }
        public SceneManager CurrentSceneManager { get; set; }
        public string Name { get; set; }
        public Entity Player { get; set; }
        public Dictionary<string, object> Values { get; set; }

        public ConstructObject(Entity player = null)
        {
            Player = player;
            ObjectList.Add(this);
        }

        public virtual void Load()
        {
            ObjectDict[Name] = this;
        }
        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(SpriteBatch spriteBatch) { }

        public static void LoadObjects() => ConstructObject.ObjectList.ForEach(o => o.Load());
        public static void UpdateObjects(GameTime gameTime) => ConstructObject.ObjectList.ForEach(o => o.Update(gameTime));
        public static void DrawObjects(SpriteBatch spriteBatch) => ConstructObject.ObjectList.ForEach(o => o.Draw(spriteBatch));
    }
}