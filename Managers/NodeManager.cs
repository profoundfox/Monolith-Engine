using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Authentication.ExtendedProtection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Attributes;
using Monolith.Nodes;

namespace Monolith.Managers
{
    public class NodeManager
    {
        private readonly List<Node> allInstances = new();
        private readonly Dictionary<string, List<Node>> allInstancesDetailed = new();

        private readonly List<Node> pendingAdds = new();
        private readonly List<Node> pendingRemovals = new();

        internal void QueueAdd(Node node)
        {
            pendingAdds.Add(node);
        }
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
                node.Load();
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
        /// Sets all the nodes in a node's config to be children of that node.
        /// </summary>
        /// <param name="parent"></param>
        private void AutoAssignChildNodes(Node parent)
        {
            if (parent.InitialConfig == null) return;

            var properties = parent.InitialConfig.GetType().GetProperties();

            foreach (var prop in properties)
            {
                if (prop.GetCustomAttribute<NodeRefferenceAttribute>() != null)
                    continue;

                var value = prop.GetValue(parent.InitialConfig);

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
        /// Removes the node without waiting.
        /// </summary>
        /// <param name="node"></param>
        internal void RemoveImmediate(Node node) => RemoveNode(node); 

        

        public void LoadNode(Node node)
        {
            ApplyPendingChanges();
            AutoAssignChildNodes(node);

            node.Load();
            ApplyPendingChanges();
        }

        /// <summary>
        /// Loads all the nodes.
        /// </summary>
        public void LoadNodes()
        {
            LoadNodes(allInstances.ToList());
        }

        /// <summary>
        /// Loads a specific list of nodes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nodes"></param>
        public void LoadNodes<T>(List<T> nodes) where T : Node
        {
            ApplyPendingChanges();

            foreach (var n in nodes)
            {
                AutoAssignChildNodes(n);
            }

            foreach (var n in nodes)
            {
                n.Load();
                ApplyPendingChanges();
            }
        }


        /// <summary>
        /// Unloads all the nodes
        /// </summary>
        public void UnloadNodes()
        {
            ApplyPendingChanges();
            foreach (var node in allInstances.ToList())
            {
                node.Unload();
                ApplyPendingChanges();
            }
        }

        /// <summary>
        /// Updates all the nodes' process function.
        /// </summary>
        /// <param name="gameTime"></param>
        internal void ProcessUpdateNodes(float delta)
        {
            ApplyPendingChanges();
            foreach (var node in allInstances.ToList())
            {
                node.ProcessUpdate(delta);
                ApplyPendingChanges();
            }
        }
        /// <summary>
        /// Updates all the nodes' physics function.
        /// </summary>
        /// <param name="gameTime"></param>
        internal void PhysicsUpdateNodes(float delta)
        {
            ApplyPendingChanges();
            foreach (var node in allInstances.ToList())
            {
                node.PhysicsUpdate(delta);
                ApplyPendingChanges();
            }
        }

        /// <summary>
        /// Draws all the nodes.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void SubmitCallNodes()
        {
            foreach (var node in allInstances)
                node.SubmitCall();
        }

        /// <summary>
        /// Dumps all instances of nodes.
        /// </summary>
        public void DumpAllInstances()
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
        public Node GetFirstNodeByName(string name)
        {
            return GetNodesByName(name).FirstOrDefault();
        }

        /// <summary>
        /// Gets all nodes based of the name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IReadOnlyList<Node> GetNodesByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return Array.Empty<Node>();
            return allInstancesDetailed.TryGetValue(name, out var nodes) ? nodes : Array.Empty<Node>();
        }  

        /// <summary>
        /// Gets the first node based of type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetFirstNodeByT<T>() where T : Node
        {
            return GetNodesByT<T>().FirstOrDefault();
        }

        /// <summary>
        /// Gets a list of nodes based of type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
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
