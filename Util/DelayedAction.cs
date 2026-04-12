using System;

namespace Monolith.Util
{
  internal class DelayedAction
  {
      public TimeSpan Remaining;
      public Action Callback;

      public DelayedAction(TimeSpan remaining, Action callback)
      {
          Remaining = remaining;
          Callback = callback;
      }
  }
}
