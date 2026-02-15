using System;
using System.Collections.Generic;
using System.IO.Pipelines;
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
        private readonly List<Node> nodes = new();
        private readonly Dictionary<string, List<Node>> byName = new();

        private readonly List<Node> pendingAdd = new();
        private readonly List<Node> pendingRemove = new();

        public T Create<T>() 
            where T : Node, new()
        {
            var node = new T();

            Flush();
            node.Load();

            return node;
        }

        internal void QueueAdd(Node node) => pendingAdd.Add(node);
        internal void QueueRemove(Node node) => pendingRemove.Add(node);

        private void Flush()
        {
            if(pendingAdd.Count == 0 && pendingRemove.Count == 0)
                return;
            
            foreach(var n in pendingAdd)
                AddInternal(n);

            pendingAdd.Clear();

            foreach (var n in pendingRemove.ToList())
                RemoveInternal(n);
            
            pendingRemove.Clear();
        }

        private void AddInternal(Node node)
        {
            nodes.Add(node);

            if(!string.IsNullOrEmpty(node.Name))
            {
                if (!byName.TryGetValue(node.Name, out var list))
                {
                    list = new List<Node>();
                    byName[node.Name] = list;
                }
                list.Add(node);
            }
        }

        private void RemoveInternal(Node node)
        {
            node.Unload();
            
            nodes.Remove(node);

            if(!string.IsNullOrEmpty(node.Name) 
                && byName.TryGetValue(node.Name, out var list))
            {
                list.Remove(node);
                if(list.Count == 0) 
                    byName.Remove(node.Name);
            }

            node.Parent?.RemoveChild(node);

            foreach (var child in node.Children.ToList())
                RemoveInternal(child);

            node.ClearNodeData();
        }

        internal void RemoveNow(Node node) => RemoveInternal(node);

        /// <summary>
        /// Frees and removes all nodes immediately.
        /// </summary>
        public void DumpAllInstances()
        {
            foreach (var node in nodes.ToList())
            {
                RemoveNow(node);
            }

            nodes.Clear();
            byName.Clear();
        }


        private static void TrySetParent(Node parent, Node child)
        {
            if (child == null || child == parent)
                return;

            if (child.Parent == null && !child.WouldCreateCycle(parent))
                child.SetParent(parent);
        }

        public void Load(Node node)
        {
            Flush();
            node.Load();
            Flush();
        }

        internal void ProcesssUpdate(float delta)
        {
            Flush();
            foreach (var n in nodes.ToList())
            {
                n.ProcessUpdate(delta);
                Flush();
            }
        }

        internal void PhysicsUpdate(float delta)
        {
            Flush();
            foreach (var n in nodes.ToList())
            {
                n.PhysicsUpdate(delta);
                Flush();
            }
        }

        public void SubmitCalls()
        {
            foreach (var n in nodes)
                n.SubmitCall();
        }

        public Node Get(string name)
            => GetAll(name).FirstOrDefault();

        public IReadOnlyList<Node> GetAll(string name)
            => string.IsNullOrEmpty(name)
                ? Array.Empty<Node>()
                : byName.TryGetValue(name, out var list)
                    ? list
                    : Array.Empty<Node>();

        public T Get<T>() where T : Node
            => GetAll<T>().FirstOrDefault();

        public IReadOnlyList<T> GetAll<T>() where T : Node
            => nodes.OfType<T>().ToList();

        public IReadOnlyList<Node> All => nodes.AsReadOnly();
    }
}