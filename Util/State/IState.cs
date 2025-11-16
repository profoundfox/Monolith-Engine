using System;
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
}
