using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using ConstructEngine.Helpers;
using ConstructEngine.Managers;
using ConstructEngine.Region;
using ConstructEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ConstructEngine.Nodes
{
    public abstract class Node
    {
        private static readonly List<Node> allInstances = new();
        private static readonly Dictionary<string, List<Node>> allInstancesDetailed = new();

        private static readonly List<Node> pendingAdds = new();
        private static readonly List<Node> pendingRemovals = new();


        /// <summary>
        /// The object that the node is instantiated within.
        /// </summary>
        public object Root { get; internal set; }

        /// <summary>
        /// The type of the root object.
        /// </summary>
        public Type RootType { get =>  Root?.GetType(); }

        /// <summary>
        /// The shape that the object has.
        /// </summary>
        public IRegionShape2D Shape { get; set; }

        /// <summary>
        /// The name of the node, defaults to the class name of the node.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The location of the node.
        /// </summary>
        public Point Location { get => Shape.Location; set => Shape.Location = value; }

        /// <summary>
        /// Values that come from the ogmo level dictionary, can be ignored if user is not using ogmo.
        /// </summary>
        public Dictionary<string, object> Values { get; internal set; } = new();

        /// <summary>
        /// Creates a new Node using a NodeConfig.
        /// </summary>
        public Node(NodeConfig config)
        {
            Root = config.Root;
            Shape = config.Shape;
            Name = config.Name;
            Values = config.Values;

            QueueAdd(this);
        }

        /// <summary>
        /// Queues a node for addition to the main instance list.
        /// </summary>
        private static void QueueAdd(Node node) => pendingAdds.Add(node);

        /// <summary>
        /// Queues a node for removal from the main instance list and clears its data.
        /// </summary>
        public void QueeFree() 
        {
            ClearNodeData();
            pendingRemovals.Add(this);
        }

        /// <summary>
        /// Clears the node's data to help with memory management.
        /// </summary>
        private void ClearNodeData()
        {
            Root = null;
            Shape = null;
            Name = null;
            Values?.Clear();
            Values = null;
        }

        /// <summary>
        /// Applies queued changes.
        /// </summary>
        private static void ApplyPendingChanges()
        {
            ApplyPendingAdds();
            ApplyPendingRemovals();
        }

        /// <summary>
        /// Applies pending additions.
        /// </summary>
        private static void ApplyPendingAdds()
        {
            if (pendingAdds.Count == 0) return;

            foreach (var node in pendingAdds)
            {
                allInstances.Add(node);

                if (!allInstancesDetailed.ContainsKey(node.Name))
                    allInstancesDetailed[node.Name] = new List<Node>();

                allInstancesDetailed[node.Name].Add(node);
            }

            pendingAdds.Clear();
        }


        /// <summary>
        /// Applies pending removals.
        /// </summary>
        private static void ApplyPendingRemovals()
        {
            if (pendingRemovals.Count == 0) return;

            var toRemove = new List<Node>(pendingRemovals);

            foreach (var node in toRemove)
            {
                RemoveNode(node);
            }

            pendingRemovals.Clear();
        }

        /// <summary>
        /// Removes the node.
        /// </summary>
        /// <param name="node"></param>
        private static void RemoveNode(Node node)
        {
            allInstances.Remove(node);

            if (!string.IsNullOrEmpty(node.Name) && allInstancesDetailed.ContainsKey(node.Name))
            {
                allInstancesDetailed[node.Name].Remove(node);

                if (allInstancesDetailed[node.Name].Count == 0)
                    allInstancesDetailed.Remove(node.Name);
            }

            var childNodes = allInstances.Where(n => n.Root == node).ToList();
            foreach (var child in childNodes)
                child.QueeFree();
        }



        public virtual void Load() { }
        public virtual void Unload() { }
        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(SpriteBatch spriteBatch) { }

        /// <summary>
        /// Invokes load on all tracked nodes.
        /// </summary>
        public static void LoadObjects()
        {
            ApplyPendingChanges();

            for (int i = 0; i < allInstances.Count; i++)
            {
                allInstances[i].Load();
                ApplyPendingChanges();
            }
        }

        /// <summary>
        /// Invokes unload on all tracked nodes.
        /// </summary>
        public static void UnloadObjects()
        {
            ApplyPendingChanges();

            for (int i = 0; i < allInstances.Count; i++)
            {
                allInstances[i].Unload();
                ApplyPendingChanges();
            }
        }

        /// <summary>
        /// Updates all nodes using a shared GameTime object.
        /// </summary>
        public static void UpdateObjects(GameTime gameTime)
        {
            ApplyPendingChanges();

            for (int i = 0; i < allInstances.Count; i++)
            {
                allInstances[i].Update(gameTime);
                ApplyPendingChanges();
            }
        }

        /// <summary>
        /// Draws all nodes using the given SpriteBatch.
        /// </summary>
        public static void DrawObjects(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < allInstances.Count; i++)
            {
                allInstances[i].Draw(spriteBatch);
            }
        }  

        /// <summary>
        /// Draws the shape with a filled texture.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="layerDepth"></param>
        /// <param name="layer"></param>
        public void DrawShape(Color color, float layerDepth = 0.1f, DrawLayer layer = DrawLayer.Middleground)
        {
            DrawHelper.DrawRegionShape(Shape, color, layerDepth, layer);
        }

        /// <summary>
        /// Draws the shape with a hollow texture.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="thickness"></param>
        /// <param name="layerDepth"></param>
        /// <param name="layer"></param>
        public void DrawShapeHollow(Color color, int thickness = 2, float layerDepth = 0.1f, DrawLayer layer = DrawLayer.Middleground)
        {
            DrawHelper.DrawRegionShapeHollow(Shape, color, thickness, layerDepth, layer);
        }


        /// <summary>
        /// Removes all node instances.
        /// </summary>
        public static void DumpAllInstances()
        {
            UnloadObjects();
            allInstances.Clear();
        }

        /// <summary>
        /// A list of all instances of nodes.
        /// </summary>
        public static IReadOnlyList<Node> AllInstances => allInstances.AsReadOnly();

        /// <summary>
        /// A dictionary of all of the instances as well as the name of said instance to make searching easier.
        /// </summary>
        public static IReadOnlyDictionary<string, IReadOnlyList<Node>> AllInstancesDetailed
        => allInstancesDetailed.ToDictionary(
            kvp => kvp.Key,
            kvp => (IReadOnlyList<Node>)kvp.Value.AsReadOnly()
        );

    }
}
