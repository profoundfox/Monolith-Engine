using System;
using System.Collections.Generic;
using System.Linq;
using Monolith.Nodes;

namespace Monolith.Managers
{
    public class TreeServer2D
    {
        private readonly List<Instance> instances = new();
        private readonly Dictionary<string, List<Instance>> byName = new();

        private readonly List<Instance> pendingAdd = new();
        private readonly List<Instance> pendingRemove = new();

        public T Create<T>() 
            where T : Instance, new()
        {
            var inst = new T();

            Flush();
            inst.OnEnter();

            return inst;
        }

        internal void QueueAdd(Instance instance) => pendingAdd.Add(instance);
        internal void QueueRemove(Instance instance) => pendingRemove.Add(instance);

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

        private void AddInternal(Instance instance)
        {
            instances.Add(instance);

            if(!string.IsNullOrEmpty(instance.GetName()))
            {
                if (!byName.TryGetValue(instance.GetName(), out var list))
                {
                    list = new List<Instance>();
                    byName[instance.GetName()] = list;
                }
                list.Add(instance);
            }
        }

        private void RemoveInternal(Instance instance)
        {
            instance.OnExit();
            
            instances.Remove(instance);

            if(!string.IsNullOrEmpty(instance.GetName()) 
                && byName.TryGetValue(instance.GetName(), out var list))
            {
                list.Remove(instance);
                if(list.Count == 0) 
                    byName.Remove(instance.GetName());
            }

            instance.OnExit();

            instance.ClearData();
        }

        internal void RemoveNow(Instance instance) => RemoveInternal(instance);

        /// <summary>
        /// Frees and removes all nodes immediately.
        /// </summary>
        public void DumpAllInstances()
        {
            foreach (var instance in instances.ToList())
            {
                RemoveNow(instance);
            }

            instances.Clear();
            byName.Clear();
        }

        internal void ProcesssUpdate(float delta)
        {
            Flush();
            foreach (var i in instances.ToList())
            {
                i.ProcessUpdate(delta);
                Flush();
            }
        }

        internal void PhysicsUpdate(float delta)
        {
            Flush();
            foreach (var i in instances.ToList())
            {
                i.PhysicsUpdate(delta);
                Flush();
            }
        }

        public void SubmitCalls()
        {
            foreach (var i in instances)
                i.SubmitCall();
        }

        public Instance Get(string name)
            => GetAll(name).FirstOrDefault();

        public IReadOnlyList<Instance> GetAll(string name)
            => string.IsNullOrEmpty(name)
                ? Array.Empty<Instance>()
                : byName.TryGetValue(name, out var list)
                    ? list
                    : Array.Empty<Instance>();

        public T Get<T>() where T : Instance
            => GetAll<T>().FirstOrDefault();

        public IReadOnlyList<T> GetAll<T>() where T : Instance
            => instances.OfType<T>().ToList();

        public IReadOnlyList<Instance> All => instances.AsReadOnly();

    }
}