using System;
using System.Collections.Generic;
using Monolith.Runtime;
using Monolith.Util;

namespace Monolith.Managers
{
  public sealed class TimeOwner : BaseObject
  {
    private readonly TimeSpan _fixedDelta;
    private TimeSpan _frameDelta;

    private TimeSpan _accumulator;
    private const int MaxSteps = 5;

    public float TimeScale { get; set; } = 1.0f;
    public float Alpha { get; private set; }

    public TimeOwner(TimeSpan fixedDelta)
    {
      _fixedDelta = fixedDelta;
    }

    private readonly List<DelayedAction> _delayedActions = new();

    public void After(TimeSpan delay, Action callback, bool ignoreTimeScale)
    {
      if (callback == null)
        return;

      _delayedActions.Add(new DelayedAction(delay, callback, ignoreTimeScale));
    }

    public int Update(TimeSpan rawDelta)
    {
      if (TimeScale == 0.0f)
      {
        _frameDelta = TimeSpan.Zero;
      }
      else
      {
        _frameDelta = TimeSpan.FromTicks(
            (long)(rawDelta.Ticks * TimeScale)
        );
      }

      _accumulator += _frameDelta;

      int steps = 0;

      while (_accumulator >= _fixedDelta && steps < MaxSteps)
      {
        _accumulator -= _fixedDelta;
        steps++;
      }

      if (steps == MaxSteps)
        _accumulator = TimeSpan.Zero;

      Alpha = _accumulator.Ticks / (float)_fixedDelta.Ticks;

      for (int i = _delayedActions.Count - 1; i >= 0; i--)
      {
        var action = _delayedActions[i];

        var delta = action.IgnoreTimeScale ? rawDelta : _frameDelta;

        action.Remaining -= delta;

        if (action.Remaining <= TimeSpan.Zero)
        {
          action.Callback?.Invoke();
          _delayedActions.RemoveAt(i);
        }
      }

      return steps;
    }

    public TimeContext GetContext()
    {
      return new TimeContext(_frameDelta, _fixedDelta, _accumulator, Alpha);
    }
  }
}
