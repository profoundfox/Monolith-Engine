using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Monolith.Nodes;

namespace Monolith.Managers
{
    /// <summary>
    /// Manages Nodes in the scene graph: addition, removal, lookup, and hierarchy.
    /// Lifecycle is now handled by a separate LifecycleManager.
    /// </summary>
    public class NodeManager
    {
        private readonly List<Node> allInstances = new();
        private readonly Dictionary<string, List<Node>> allInstancesDetailed = new();

        private readonly List<Node> pendingAdds = new();
        private readonly List<Node> pendingRemovals = new();

        public NodeManager()
        {
            Engine.Stage.OnStageUpdated += ApplyPendingChanges;
        }

        /// <summary>
        /// Queue a node to be added to the manager.
        /// </summary>
        internal void QueueAdd(Node node)
        {
            pendingAdds.Add(node);
        }

        /// <summary>
        /// Queue a node to be removed from the manager.
        /// </summary>
        internal void QueueRemove(Node node) => pendingRemovals.Add(node);

        private void ApplyPendingChanges()
        {
            ApplyPendingAdds();
            ApplyPendingRemovals();
        }

        private void ApplyPendingAdds()
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
            }

            if (pendingAdds.Count > 0)
                ApplyPendingAdds();
        }

        private void ApplyPendingRemovals()
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
        public List<Node> GetNodeChildren(Node node) => node.Children.ToList();

        /// <summary>
        /// Returns the parent of a node.
        /// </summary>
        public Node GetNodeParent(Node node) => node.Parent;

        /// <summary>
        /// Sets all the nodes in a node's config to be children of that node.
        /// </summary>
        private void AutoAssignChildNodes(Node parent)
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
        private void RemoveNode(Node node)
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
        /// Removes the node immediately.
        /// </summary>
        internal void RemoveImmediate(Node node) => RemoveNode(node);

        /// <summary>
        /// Dumps all instances of nodes.
        /// </summary>
        public void DumpAllInstances()
        {
            allInstances.Clear();
            allInstancesDetailed.Clear();
        }

        /// <summary>
        /// Gets the first node based on name.
        /// </summary>
        public Node GetFirstNodeByName(string name)
        {
            return GetNodesByName(name).FirstOrDefault();
        }

        /// <summary>
        /// Gets all nodes based on name.
        /// </summary>
        public IReadOnlyList<Node> GetNodesByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return Array.Empty<Node>();
            return allInstancesDetailed.TryGetValue(name, out var nodes) ? nodes : Array.Empty<Node>();
        }

        /// <summary>
        /// Gets the first node based on type.
        /// </summary>
        public T GetFirstNodeByT<T>() where T : Node
        {
            return GetNodesByT<T>().FirstOrDefault();
        }

        /// <summary>
        /// Gets a list of nodes based on type.
        /// </summary>
        public IReadOnlyList<T> GetNodesByT<T>() where T : Node
        {
            return allInstancesDetailed
                .SelectMany(kvp => kvp.Value)
                .OfType<T>()
                .ToList();
        }

        /// <summary>
        /// All instances of nodes.
        /// </summary>
        public IReadOnlyList<Node> AllInstances => allInstances.AsReadOnly();

        /// <summary>
        /// Detailed instances of all nodes.
        /// </summary>
        public IReadOnlyDictionary<string, IReadOnlyList<Node>> AllInstancesDetailed =>
            allInstancesDetailed.ToDictionary(
                kvp => kvp.Key,
                kvp => (IReadOnlyList<Node>)kvp.Value.AsReadOnly()
            );
    }
}
