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

        /// <summary>
        /// Queues a node for addition to the main instance list.
        /// </summary>
        internal static void QueueAdd(Node node) => pendingAdds.Add(node);

        /// <summary>
        /// Queues a node for removal from the main instance list.
        /// </summary>
        internal static void QueueRemove(Node node) => pendingRemovals.Add(node);

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
                RemoveNode(node);

            pendingRemovals.Clear();
        }

        /// <summary>
        /// Removes the node.
        /// </summary>
        private static void RemoveNode(Node node)
        {
            allInstances.Remove(node);

            if (!string.IsNullOrEmpty(node.Name) && allInstancesDetailed.ContainsKey(node.Name))
            {
                allInstancesDetailed[node.Name].Remove(node);

                if (allInstancesDetailed[node.Name].Count == 0)
                    allInstancesDetailed.Remove(node.Name);
            }

            node.ClearNodeData();

            var childNodes = allInstances.Where(n => n.Root == node).ToList();
            foreach (var child in childNodes)
                QueueRemove(child);
        }

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
        /// Draws all nodes.
        /// </summary>
        public static void DrawObjects(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < allInstances.Count; i++)
                allInstances[i].Draw(spriteBatch);
        }

        /// <summary>
        /// Removes all node instances.
        /// </summary>
        public static void DumpAllInstances()
        {
            UnloadObjects();
            allInstances.Clear();
            allInstancesDetailed.Clear();
        }

        /// <summary>
        /// Gets the first node with the given name, or null if none exists.
        /// </summary>
        public static Node GetNodeByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            if (allInstancesDetailed.TryGetValue(name, out var nodes))
            {
                return nodes.FirstOrDefault();
            }

            return null;
        }

        /// <summary>
        /// Gets all nodes with the given name, returns an empty list if none exist.
        /// </summary>
        public static IReadOnlyList<Node> GetNodesByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return Array.Empty<Node>();

            if (allInstancesDetailed.TryGetValue(name, out var nodes))
            {
                return nodes;
            }

            return Array.Empty<Node>();
        }

        public static IReadOnlyList<Node> AllInstances => allInstances.AsReadOnly();

        public static IReadOnlyDictionary<string, IReadOnlyList<Node>> AllInstancesDetailed =>
            allInstancesDetailed.ToDictionary(
                kvp => kvp.Key,
                kvp => (IReadOnlyList<Node>)kvp.Value.AsReadOnly()
            );
    }
}
