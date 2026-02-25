using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using Monolith.Managers;

namespace Monolith.Instances
{
    public class Node : Instance
    {
        private readonly List<Node> children = new();
        private Node parent;

        public IReadOnlyList<Node> Children => children.AsReadOnly();

        /// <summary>
        /// Signal for when a parent is changed.
        /// </summary>
        public event Action<Node> OnParentChanged;

        /// <summary>
        /// Signal for when a child is added.
        /// </summary>
        public event Action<Node> OnChildAdded;

        /// <summary>
        /// Signal for when a child is removed.
        /// </summary>
        public event Action<Node> OnChildRemoved;
        
        public Node() {}

        public Node GetParent()
        {
            return parent;
        }

        public TParent GetParent<TParent>() where TParent : Node
        {
            return parent as TParent;
        }

        public void SetParent(Node newParent)
        {
            if (newParent == this)
                throw new Exception("Node attempted to become its own parent!");

            if (parent == newParent)
                return;

            if (WouldCreateCycle(newParent))
                throw new Exception("Parenting would create a cycle in the node tree.");

            parent?.children.Remove(this);
            parent = newParent;
            parent?.children.Add(this);

            OnParentChanged?.Invoke(newParent);
        }

        internal bool WouldCreateCycle(Node newParent)
        {
            var p = newParent;
            while (p != null)
            {
                if (p == this) return true;
                p = p.parent;
            }
            return false;
        }

        public void AddChild(Node child)
        {
            if (child == null || children.Contains(child))
                return;

            child.SetParent(this);

            OnChildAdded?.Invoke(child);
        }


        public T Get<T>() where T : Node
        {
            return children.OfType<T>().FirstOrDefault();
        }

        public IReadOnlyList<T> GetAll<T>() where T : Node
        {
            return children.OfType<T>().ToList();
        }

        public void RemoveChild(Node child)
        {
            if (child == null || !children.Contains(child))
                return;

            child.SetParent(null);

            OnChildRemoved?.Invoke(child);
        }

        internal override void ClearData()
        {
            base.ClearData();

            parent = null;
            children.Clear();
        }

        
        /// <summary>
        /// Called when the node enters the tree.
        /// </summary>
        public override void OnEnter()
        {
            base.OnEnter();
        }

        /// <summary>
        /// Called when the node exits the tree.
        /// </summary>
        public override void OnExit()
        {
            base.OnExit();
        }

        /// <summary>
        /// Called every frame.
        /// </summary>
        public override void ProcessUpdate(float delta)
        {
            base.ProcessUpdate(delta);
        }

        /// <summary>
        /// Called every physics tick.
        /// </summary>
        public override void PhysicsUpdate(float delta)
        {
            base.PhysicsUpdate(delta);
        }

        /// <summary>
        /// Called when submitting draw calls.
        /// </summary>
        public override void SubmitCall()
        {
            base.SubmitCall();
        }
    }
}