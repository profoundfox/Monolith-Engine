using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Geometry;
using Monolith.Helpers;
using Monolith.Managers;

namespace Monolith.Nodes
{
    public abstract class Node
    {
        private readonly List<Node> children = new();

        /// <summary>
        /// The parent node in the hierarchy, required to be set, can be null
        /// </summary>
        public Node Parent { get; private set; }

        /// <summary>
        /// The name of the node, required to be set.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Creates a new Node using a NodeConfig.
        /// </summary>
        public Node(NodeConfig config)
        {
            Name = config.Name ?? GetType().Name;

            if (config.Parent != null)
                SetParent(config.Parent);

            NodeManager.QueueAdd(this);
        }

        /// <summary>
        /// Sets the parent of this node, automatically updating the children lists.
        /// </summary>
        public void SetParent(Node newParent)
        {
            if (Parent == newParent) return;

            Parent?.children.Remove(this);
            Parent = newParent;
            Parent?.children.Add(this);
        }

        /// <summary>
        /// Adds a child to this node.
        /// </summary>
        public void AddChild(Node child)
        {
            if (child == null || children.Contains(child)) return;
            child.SetParent(this);
        }

        /// <summary>
        /// Removes a child from this node.
        /// </summary>
        public void RemoveChild(Node child)
        {
            if (child == null || !children.Contains(child)) return;
            child.SetParent(null);
        }

        /// <summary>
        /// Queues a node for removal from the main instance list and clears its data.
        /// </summary>
        public void QueueFree()
        {
            NodeManager.QueueRemove(this);
        }

        /// <summary>
        /// Removes the instance of this node immediately.
        /// </summary>
        public void FreeImmediate()
        {
            NodeManager.RemoveImmediate(this);
        }

        /// <summary>
        /// Clears the node's data to help with memory management.
        /// </summary>
        internal void ClearNodeData()
        {
            Parent = null;
            children.Clear();
            Name = null;
        }

        public virtual void Load() { }
        public virtual void Unload() { }
        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(SpriteBatch spriteBatch) { }

        /// <summary>
        /// All children of this node.
        /// </summary>
        public IReadOnlyList<Node> Children => children.AsReadOnly();
    }
}
