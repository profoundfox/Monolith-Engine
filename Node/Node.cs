using System;
using System.Collections.Generic;
using System.Linq;
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
        
        public NodeConfig Config { get; }

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
            if (config.Name != null)
                Name = config.Name;
            else 
                Name = ToString();

            if (config.Parent != null)
                SetParent(config.Parent);
            
            Config = config;

            NodeManager.QueueAdd(this);
        }

        /// <summary>
        /// Sets the parent of this node, automatically updating the children lists.
        /// </summary>
        public void SetParent(Node newParent)
        {
            if (newParent == this)
                throw new Exception("Node attempted to become its own parent!");

            if (Parent == newParent) return;

            Parent?.children.Remove(this);
            Parent = newParent;
            Parent?.children.Add(this);
        }

        /// <summary>
        /// Checks if a node creates a cycle.
        /// A - B - A
        /// </summary>
        /// <param name="newParent"></param>
        /// <returns></returns>
        internal bool WouldCreateCycle(Node newParent)
        {
            var p = newParent;
            while (p != null)
            {
                if (p == this) return true;
                p = p.Parent;
            }
            return false;
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
        /// Gets the first found child of a specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Node GetFirstChildByT<T>()
        {
            return GetChildrenByT<T>().FirstOrDefault();
        }

        /// <summary>
        /// Gets all children of a specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IReadOnlyList<Node> GetChildrenByT<T>()
        {
            return children.OfType<T>().Cast<Node>().ToList();
        }
        
        
        /// <summary>
        /// Returns if the node has a node of a specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool HasChildByT<T>()
        {
            return GetFirstChildByT<T>() != null;
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
