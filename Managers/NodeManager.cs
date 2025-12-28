using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Nodes;

namespace Monolith.Managers
{
    public sealed class NodeManager : InstanceManager<Node>
    {
        protected override void ApplyPending()
        {
            base.ApplyPending();

            foreach (var node in allInstances)
                AutoAssignChildNodes(node);
        }

        public IReadOnlyList<Node> GetNodeChildren(Node node)
            => node.Children.ToList();

        public Node GetNodeParent(Node node)
            => node.Parent;

        public Node GetFirstNodeByName(string name)
            => GetByName(name).FirstOrDefault();

        public IReadOnlyList<T> GetNodesByType<T>() where T : Node
            => allInstances.OfType<T>().ToList();

        public T GetFirstNodeByType<T>() where T : Node
            => allInstances.OfType<T>().FirstOrDefault();

        public void RemoveRecursive(Node node)
        {
            foreach (var child in node.Children.ToList())
                RemoveRecursive(child);

            node.Parent?.RemoveChild(node);
            QueueRemove(node);
        }

        public void RemoveImmediate(Node node)
        {
            RemoveRecursive(node);
            ApplyPending();
        }

        public void DumpAllNodes()
        {
            UnloadAll();
            allInstances.Clear();
            byName.Clear();
        }

        private void AutoAssignChildNodes(Node parent)
        {
            if (parent.Config == null)
                return;

            var properties = parent.Config
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                if (prop.GetCustomAttribute<NodeRefferenceAttribute>() != null)
                    continue;

                var value = prop.GetValue(parent.Config);

                if (value is Node child)
                {
                    TryAssignChild(parent, child);
                }
                else if (value is IEnumerable<Node> children)
                {
                    foreach (var c in children)
                        TryAssignChild(parent, c);
                }
            }
        }

        private static void TryAssignChild(Node parent, Node child)
        {
            if (child == null)
                return;

            if (child == parent)
                return;

            if (child.Parent != null)
                return;

            if (child.WouldCreateCycle(parent))
                return;

            child.SetParent(parent);
        }
    }
}
