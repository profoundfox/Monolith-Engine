using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monolith.Util;

namespace Monolith.Managers
{
  public class TimerManager
  {
      private readonly List<ITimer> timers = new();

      public void Add(ITimer timer) => timers.Add(timer);

      public void WaitFrames(int frames, Action callback)
          => Add(new FrameTimer(frames, callback));

      public void Wait(TimeSpan time, Action callback)
          => Add(new IntervalTimer(time, callback, false, false));

      public void WaitUnscaled(TimeSpan time, Action callback)
          => Add(new IntervalTimer(time, callback, false, true));

      public void Repeat(TimeSpan interval, Action callback)
          => Add(new IntervalTimer(interval, callback, true, false));

      public void RepeatUnscaled(TimeSpan interval, Action callback)
          => Add(new IntervalTimer(interval, callback, true, true));

      public Action WaitCancelable(TimeSpan time, Action callback)
      {
          var timer = new IntervalTimer(time, callback, false, false);
          Add(timer);
          return () => timer.Cancelled = true;
      }

      public void PhysicsUpdate(TimeSpan scaledDelta, TimeSpan unscaledDelta)
      {
          for (int i = timers.Count - 1; i >= 0; i--)
          {
              if (timers[i].Update(scaledDelta, unscaledDelta))
                  timers.RemoveAt(i);
          }
      }

      public void ClearAll() => timers.Clear();
  }
}
