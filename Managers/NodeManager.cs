using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Nodes;

namespace Monolith.Managers
{
    public static class NodeManager
    {
        private static readonly List<Node> allInstances = new();
        private static readonly Dictionary<string, List<Node>> allInstancesDetailed = new();

        private static readonly List<Node> pendingAdds = new();
        private static readonly List<Node> pendingRemovals = new();

        internal static void QueueAdd(Node node) => pendingAdds.Add(node);
        internal static void QueueRemove(Node node) => pendingRemovals.Add(node);

        private static void ApplyPendingChanges()
        {
            ApplyPendingAdds();
            ApplyPendingRemovals();
        }

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

        internal static void RemoveImmediate(Node node) => RemoveNode(node);

        public static void LoadObjects()
        {
            ApplyPendingChanges();
            foreach (var node in allInstances.ToList())
            {
                node.Load();
                ApplyPendingChanges();
            }
        }

        public static void UnloadObjects()
        {
            ApplyPendingChanges();
            foreach (var node in allInstances.ToList())
            {
                node.Unload();
                ApplyPendingChanges();
            }
        }

        public static void UpdateObjects(GameTime gameTime)
        {
            ApplyPendingChanges();
            foreach (var node in allInstances.ToList())
            {
                node.Update(gameTime);
                ApplyPendingChanges();
            }
        }

        public static void DrawObjects(SpriteBatch spriteBatch)
        {
            foreach (var node in allInstances)
                node.Draw(spriteBatch);
        }

        public static void DumpAllInstances()
        {
            UnloadObjects();
            allInstances.Clear();
            allInstancesDetailed.Clear();
        }

        public static Node GetNodeByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            return allInstancesDetailed.TryGetValue(name, out var nodes) ? nodes.FirstOrDefault() : null;
        }

        public static IReadOnlyList<Node> GetNodesByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return Array.Empty<Node>();
            return allInstancesDetailed.TryGetValue(name, out var nodes) ? nodes : Array.Empty<Node>();
        }

        public static IReadOnlyList<Node> AllInstances => allInstances.AsReadOnly();

        public static IReadOnlyDictionary<string, IReadOnlyList<Node>> AllInstancesDetailed =>
            allInstancesDetailed.ToDictionary(
                kvp => kvp.Key,
                kvp => (IReadOnlyList<Node>)kvp.Value.AsReadOnly()
            );
    }
}
