using System;

namespace Monolith.Util
{
  public class FrameTimer : ITimer
  {
      public int FramesLeft { get; private set; }

      public Action Callback { get; set; }

      public bool Cancelled { get; set; }

      public FrameTimer(int frames, Action callback)
      {
          if (frames <= 0)
              throw new ArgumentOutOfRangeException(nameof(frames));

          FramesLeft = frames;
          Callback = callback;
      }

      public bool Update(TimeSpan scaledDelta, TimeSpan unscaledDelta)
      {
          if (Cancelled) return true;

          FramesLeft--;

          if (FramesLeft <= 0)
          {
              Callback?.Invoke();
              return true;
          }

          return false;
      }
  }
}
