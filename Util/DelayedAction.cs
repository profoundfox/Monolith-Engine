using System;

namespace Monolith.Util
{
  internal class DelayedAction
  {
      public TimeSpan Remaining;
      public Action Callback;
      public bool IgnoreTimeScale;

      public DelayedAction(TimeSpan remaining, Action callback, bool ignoreTimeScale)
      {
          Remaining = remaining;
          Callback = callback;
          IgnoreTimeScale = ignoreTimeScale;
      }
  }
}
