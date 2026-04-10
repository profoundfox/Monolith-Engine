using System;
using System.Collections.Generic;
using System.Linq;

namespace Monolith
{   
    /// <summary>
    /// The absolute absract class for an object that has a life cycle.
    /// </summary>
    public abstract class Instance
    {
        private string name;            

        public Instance()
        {
            name = GetType().Name;
            Engine.Tree.QueueAdd(this);
        }

        /// <summary>
        /// Returns the name of this instance.
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return name;
        }

        /// <summary>
        /// Queues this instance to be freed.
        /// </summary>
        public void QueueFree()
        {
            Engine.Tree.QueueRemove(this);
        }
        
        /// <summary>
        /// Immediatley removes this instance.
        /// </summary>
        public void FreeImmediate()
        {
            Engine.Tree.RemoveNow(this);
        }

        /// <summary>
        /// Clear this instance's data.
        /// </summary>
        internal virtual void ClearData()
        {
            name = null;
        }

        public virtual void OnEnter() {}

        public virtual void ProcessUpdate(float dt) {}

        public virtual void PhysicsUpdate(float dt) {}

        public virtual void SubmitCall() {}

        public virtual void OnExit() {}
    }
}
