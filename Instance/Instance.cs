using System;
using System.Collections.Generic;
using System.Linq;

namespace Monolith.Managers
{
    public class Instance
    {
        private string name;            


        public Instance()
        {
            name = GetType().Name;
            Engine.Tree.QueueAdd(this);
        }

        public string GetName()
        {
            return name;
        }


        public void QueueFree()
        {
            Engine.Tree.QueueRemove(this);
        }

        public void FreeImmediate()
        {
            Engine.Tree.RemoveNow(this);
        }

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