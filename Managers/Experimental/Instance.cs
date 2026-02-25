using System;
using System.Collections.Generic;
using System.Linq;

namespace Monolith.Managers
{
    public class Instance
    {
        private readonly List<Instance> children = new();
        private Instance parent;
        private string name;

        public IReadOnlyList<Instance> Children => children.AsReadOnly();

        /// <summary>
        /// Signal for when a parent is changed.
        /// </summary>
        public event Action<Instance> OnParentChanged;

        /// <summary>
        /// Signal for when a child is added.
        /// </summary>
        public event Action<Instance> OnChildAdded;

        /// <summary>
        /// Signal for when a child is removed.
        /// </summary>
        public event Action<Instance> OnChildRemoved;

        public Instance()
        {
            name = GetType().Name;
            Engine.Tree.QueueAdd(this);
        }

        public string GetName()
        {
            return name;
        }

        public Instance GetParent()
        {
            return parent;
        }

        public TParent GetParent<TParent>() where TParent : Instance
        {
            return parent as TParent;
        }

        public void SetParent(Instance newParent)
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

        internal bool WouldCreateCycle(Instance newParent)
        {
            var p = newParent;
            while (p != null)
            {
                if (p == this) return true;
                p = p.parent;
            }
            return false;
        }

        public void AddChild(Instance child)
        {
            if (child == null || children.Contains(child))
                return;

            child.SetParent(this);

            OnChildAdded?.Invoke(child);
        }

        public T Get<T>() where T : Instance
        {
            return children.OfType<T>().FirstOrDefault();
        }

        public IReadOnlyList<T> GetAll<T>() where T : Instance
        {
            return children.OfType<T>().ToList();
        }

        public void RemoveChild(Instance child)
        {
            if (child == null || !children.Contains(child))
                return;

            child.SetParent(null);

            OnChildRemoved?.Invoke(child);
        }

        public void QueueFree()
        {
            Engine.Tree.QueueRemove(this);
        }

        public void FreeImmediate()
        {
            Engine.Tree.RemoveNow(this);
        }

        internal void ClearNodeData()
        {
            parent = null;
            children.Clear();
            name = null;
        }


        public virtual void OnEnter() {}

        public virtual void ProcessUpdate(float dt) {}

        public virtual void PhysicsUpdate(float dt) {}

        public virtual void SubmitCall() {}

        public virtual void OnExit() {}
    }
}