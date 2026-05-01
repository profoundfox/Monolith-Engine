using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace Monolith.Util
{
  public interface IState
  {
    event Action<IState, string> TransitionRequested;
    void _EnterTree();

    void Update(float delta);
    void _ExitTree();
  }

  public abstract class State : BaseObject, IState
  {
    public event Action<IState, string> TransitionRequested;

    protected void RequestTransition(string newStateName)
    {
      TransitionRequested?.Invoke(this, newStateName);
    }

    public virtual void _EnterTree() { }
    public virtual void Update(float delta) { }
    public virtual void _ExitTree() { }
  }
}
