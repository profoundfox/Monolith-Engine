using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ConstructEngine.Components;
using ConstructEngine.Region;
using ConstructEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ConstructEngine.Nodes
{
    public class Node
    {
        private static List<Node> allInstances = new();
        private static Dictionary<string, Node> allInstancesDetailed = new Dictionary<string, Node>();

        public static IReadOnlyList<Node> AllInstances => allInstances.AsReadOnly();
        public static IReadOnlyDictionary<string, Node> AllInstancesDetailed => new ReadOnlyDictionary<string, Node>(allInstancesDetailed);
        
        public Object Root {get; set;}
        public Type RootType {get; set;}
        public IRegionShape2D Shape {get; set;}
        public string Name { get; set; }
        public Dictionary<string, object> Values { get; set; }

        public Node()
        {
            allInstances.Add(this);
        }

        public virtual void Load() { }
        public virtual void Unload() { }
        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(SpriteBatch spriteBatch) { }

        public static void LoadObjects() => allInstances.ForEach(o => o.Load());
        public static void UnloadObjects() => allInstances.ForEach(o => o.Unload());
        public static void UpdateObjects(GameTime gameTime) => allInstances.ForEach(o => o.Update(gameTime));
        public static void DrawObjects(SpriteBatch spriteBatch) => allInstances.ForEach(o => o.Draw(spriteBatch));
        public static void DumpAllInstances()
        {
            UnloadObjects();
            allInstances.Clear();
        }
    }
}