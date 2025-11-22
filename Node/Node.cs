using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using ConstructEngine.Components;
using ConstructEngine.Region;
using ConstructEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ConstructEngine.Nodes
{
    public abstract class Node
    {
        private static readonly List<Node> allInstances = new();
        private static readonly Dictionary<string, Node> allInstancesDetailed = new();

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
        /// Queues a node for removal from the main instance list.
        /// </summary>
        public void QueeFree() => pendingRemovals.Add(this);

        /// <summary>
        /// Applies queued additions and removals before or after each lifecycle step.
        /// </summary>
        private static void ApplyPendingChanges()
        {
            if (pendingAdds.Count > 0)
            {
                allInstances.AddRange(pendingAdds);
                pendingAdds.Clear();
            }

            if (pendingRemovals.Count > 0)
            {
                foreach (var n in pendingRemovals)
                    allInstances.Remove(n);
                pendingRemovals.Clear();
            }
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
        public static IReadOnlyDictionary<string, Node> AllInstancesDetailed 
            => new ReadOnlyDictionary<string, Node>(allInstancesDetailed);
    }
}
