using System;
using System.Collections.Generic;
using System.Linq;

namespace Monolith.Nodes
{
    public class Node
    {
        private readonly List<Node> children = new();

        /// <summary>
        /// The parent node in the hierarchy
        /// </summary>
        public Node Parent { get; private set; }

        /// <summary>
        /// The name of the node.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// All children of this node.
        /// </summary>
        public IReadOnlyList<Node> Children => children.AsReadOnly();

        /// <summary>
        /// Creates a new Node.
        /// </summary>
        public Node()
        {
            Name = GetType().Name;
            Engine.Node.QueueAdd(this);
        }

        /// <summary>
        /// Sets the parent of this node, automatically updating the children lists.
        /// </summary>
        public void SetParent(Node newParent)
        {
            if (newParent == this)
                throw new Exception("Node attempted to become its own parent!");

            if (Parent == newParent)
                return;

            if (WouldCreateCycle(newParent))
                throw new Exception("Parenting would create a cycle in the node tree.");

            Parent?.children.Remove(this);
            Parent = newParent;
            Parent?.children.Add(this);

            OnParentChanged();
        }

        /// <summary>
        /// Checks if a node creates a cycle.
        /// A - B - A
        /// </summary>
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
            if (child == null || children.Contains(child))
                return;

            child.SetParent(this);
        }

        /// <summary>
        /// Gets the first found child of a specified type.
        /// </summary>
        public Node GetFirstChildByT<T>() where T : Node
        {
            return children.OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Gets all children of a specified type.
        /// </summary>
        public IReadOnlyList<Node> GetChildrenByT<T>() where T : Node
        {
            return children.OfType<T>().ToList();
        }

        /// <summary>
        /// Returns if the node has a node of a specified type.
        /// </summary>
        public bool HasChildByT<T>() where T : Node
        {
            return GetFirstChildByT<T>() != null;
        }

        /// <summary>
        /// Removes a child from this node.
        /// </summary>
        public void RemoveChild(Node child)
        {
            if (child == null || !children.Contains(child))
                return;

            child.SetParent(null);
        }

        /// <summary>
        /// Queues a node for removal from the main instance list and clears its data.
        /// </summary>
        public void QueueFree()
        {
            Engine.Node.QueueRemove(this);
        }

        /// <summary>
        /// Removes the instance of this node immediately.
        /// </summary>
        public void FreeImmediate()
        {
            Engine.Node.RemoveNow(this);
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

        /// <summary>
        /// Called when the node enters the tree.
        /// </summary>
        public virtual void Load() { }

        /// <summary>
        /// Called when the node exits the tree.
        /// </summary>
        public virtual void Unload() { }

        /// <summary>
        /// Called every frame.
        /// </summary>
        public virtual void ProcessUpdate(float delta) { }

        /// <summary>
        /// Called every physics tick.
        /// </summary>
        public virtual void PhysicsUpdate(float delta) { }

        /// <summary>
        /// Called when submitting draw calls.
        /// </summary>
        public virtual void SubmitCall() { }

        /// <summary>
        /// Called when the parent changes.
        /// </summary>
        protected virtual void OnParentChanged() { }
    }
}