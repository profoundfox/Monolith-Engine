using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace ConstructEngine.Util
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
        protected State ParentState { get; private set; }

        protected void RequestTransition(string newStateName)
        {
            TransitionRequested?.Invoke(this, newStateName);
        }

        public virtual void SetParent(State parent)
        {
            ParentState = parent;
        }

        public virtual void OnEnter() { }
        public virtual void Update(GameTime gameTime)
        {
            ParentState?.Update(gameTime);
        }
        public virtual void OnExit() { }
    }
}
