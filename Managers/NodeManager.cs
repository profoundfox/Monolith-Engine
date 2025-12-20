using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monlith.Nodes;
using Monolith.Attributes;
using Monolith.Nodes;

namespace Monolith.Managers
{
    public static class NodeManager
    {
        private static readonly List<Node> allInstances = new();
        private static readonly Dictionary<string, List<Node>> allInstancesDetailed = new();

        private static readonly List<Node> pendingAdds = new();
        private static readonly List<Node> pendingRemovals = new();

        internal static void QueueAdd(Node node)
        {
            pendingAdds.Add(node);
        }
        internal static void QueueRemove(Node node) => pendingRemovals.Add(node);

        private static void ApplyPendingChanges()
        {
            ApplyPendingAdds();
            ApplyPendingRemovals();
        }

        private static void ApplyPendingAdds()
        {
            if (pendingAdds.Count == 0) return;

            var toAdd = pendingAdds.ToList();
            pendingAdds.Clear();

            foreach (var node in toAdd)
            {
                allInstances.Add(node);

                if (!allInstancesDetailed.ContainsKey(node.Name))
                    allInstancesDetailed[node.Name] = new List<Node>();

                allInstancesDetailed[node.Name].Add(node);

                AutoAssignChildNodes(node);
                node.Load();
            }

            if (pendingAdds.Count > 0)
                ApplyPendingAdds();
        }



        private static void ApplyPendingRemovals()
        {
            if (pendingRemovals.Count == 0) return;

            var toRemove = new List<Node>(pendingRemovals);
            foreach (var node in toRemove)
                RemoveNode(node);

            pendingRemovals.Clear();
        }

        /// <summary>
        /// Returns the children of a node.
        /// </summary>
        public static List<Node> GetNodeChildren(Node node) => node.Children.ToList();

        /// <summary>
        /// Returns the parent of a node.
        /// </summary>
        public static Node GetNodeParent(Node node) => node.Parent;

        /// <summary>
        /// Sets all the nodes in a node's config to be children of that node.
        /// </summary>
        /// <param name="parent"></param>
        private static void AutoAssignChildNodes(Node parent)
        {
            if (parent.Config == null) return;

            var properties = parent.Config.GetType().GetProperties();

            foreach (var prop in properties)
            {
                if (prop.GetCustomAttribute<NodeRefferenceAttribute>() != null)
                    continue;

                var value = prop.GetValue(parent.Config);

                if (value is Node child)
                {
                    if (child == parent) continue;
                    if (child.Parent == null && !child.WouldCreateCycle(parent))
                        child.SetParent(parent);
                }

                if (value is IEnumerable<Node> list)
                {
                    foreach (var c in list)
                    {
                        if (c == parent) continue;
                        if (c.Parent == null && !c.WouldCreateCycle(parent))
                            c.SetParent(parent);
                    }
                }
            }
        }


        /// <summary>
        /// Removes a specified node.
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

            node.Parent?.RemoveChild(node);

            foreach (var child in node.Children.ToList())
                RemoveNode(child);

            node.ClearNodeData();
        }

        /// <summary>
        /// Removes the node without waiting.
        /// </summary>
        /// <param name="node"></param>
        internal static void RemoveImmediate(Node node) => RemoveNode(node);

        /// <summary>
        /// Loads all the nodes.
        /// </summary>
        public static void LoadNodes()
        {
            ApplyPendingChanges();

            foreach (var node in allInstances.ToList())
            {
                AutoAssignChildNodes(node);
            }

            foreach (var node in allInstances.ToList())
            {
                node.Load();
                ApplyPendingChanges();
            }

            Console.WriteLine("Nodes Loaded!");
        }


        /// <summary>
        /// Unloads all the nodes
        /// </summary>
        public static void UnloadNodes()
        {
            ApplyPendingChanges();
            foreach (var node in allInstances.ToList())
            {
                node.Unload();
                ApplyPendingChanges();
            }
        }

        /// <summary>
        /// Updates all the nodes.
        /// </summary>
        /// <param name="gameTime"></param>
        public static void UpdateNodes(GameTime gameTime)
        {
            ApplyPendingChanges();
            foreach (var node in allInstances.ToList())
            {
                node.Update(gameTime);
                ApplyPendingChanges();
            }
        }

        /// <summary>
        /// Draws all the nodes.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public static void DrawNodes(SpriteBatch spriteBatch)
        {
            foreach (var node in allInstances)
                node.Draw(spriteBatch);
        }

        /// <summary>
        /// Dumps all instances of nodes.
        /// </summary>
        public static void DumpAllInstances()
        {
            UnloadNodes();
            allInstances.Clear();
            allInstancesDetailed.Clear();
        }

        /// <summary>
        /// Gets a node based of the name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Node GetFirstNodeByName(string name)
        {
            return GetNodesByName(name).FirstOrDefault();
        }

        /// <summary>
        /// Gets all nodes based of the name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IReadOnlyList<Node> GetNodesByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return Array.Empty<Node>();
            return allInstancesDetailed.TryGetValue(name, out var nodes) ? nodes : Array.Empty<Node>();
        }  

        /// <summary>
        /// Gets the first node based of type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetFirstNodeByT<T>() where T : Node
        {
            return GetNodesByT<T>().FirstOrDefault();
        }

        /// <summary>
        /// Gets a list of nodes based of type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IReadOnlyList<T> GetNodesByT<T>() where T : Node
        {
            return allInstancesDetailed
                .SelectMany(kvp => kvp.Value)
                .OfType<T>()
                .ToList();
        }

        /// <summary>
        /// All instances of nodes.
        /// </summary>
        public static IReadOnlyList<Node> AllInstances => allInstances.AsReadOnly();

        /// <summary>
        /// Detailed instances of all nodes.
        /// </summary>
        public static IReadOnlyDictionary<string, IReadOnlyList<Node>> AllInstancesDetailed =>
            allInstancesDetailed.ToDictionary(
                kvp => kvp.Key,
                kvp => (IReadOnlyList<Node>)kvp.Value.AsReadOnly()
            );
    }
}
