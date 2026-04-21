using System;
using System.Collections.Generic;
using Monolith.Util;

namespace Monolith.Runtime
{
  public abstract class Loop : Instance
  {

    private readonly Queue<Action> continuations = new();

    internal void Update(TimeContext context, int steps)
    {
      for (int i = 0; i < steps; i++)
      {
        PhysicsUpdate(context.FixedDelta);
      }
      
      ProcessUpdate(context.FrameDelta);

      while (continuations.Count > 0)
      {
        continuations.Dequeue()?.Invoke();
      }
    }

    internal void Post(Action action)
    {
      if (action == null) return;
      continuations.Enqueue(action);
    }

    public virtual void PhysicsUpdate(TimeSpan delta) {}
    public virtual void ProcessUpdate(TimeSpan delta) {}
  }
}
