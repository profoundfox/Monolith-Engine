using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace Monolith.Util
{
    public interface IState
    {
        event Action<IState, string> TransitionRequested;
        void OnEnter();
        
        void Update(float delta);
        void OnExit();
    }
    
    public abstract class State : IState
    {
        public event Action<IState, string> TransitionRequested;

        protected void RequestTransition(string newStateName)
        {
            TransitionRequested?.Invoke(this, newStateName);
        }

        public virtual void OnEnter() { }
        public virtual void Update(float delta) {}
        public virtual void OnExit() { }
    }
}
