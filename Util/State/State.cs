using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace Monolith.Util
{
    public interface IState
    {
        event Action<IState, string> TransitionRequested;
        void OnEnter();
        
        void Update(GameTime gameTime);
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
        public virtual void Update(GameTime gameTime) {}
        public virtual void OnExit() { }
    }
}
