using System;

namespace Monolith.Util 
{
  public class IntervalTimer : ITimer
  {
      public TimeSpan Interval { get; set; }
      
      public TimeSpan TimeLeft { get; private set; }

      public bool Repeat { get; set; }

      public bool UseUnscaledTime { get; set; }

      public Action Callback { get; set; }

      public bool Cancelled { get; set; }
      
      public IntervalTimer(TimeSpan interval, Action callback, bool repeat, bool useUnscaledTime)
      {
          Interval = interval;
          TimeLeft = interval;
          Repeat = repeat;
          UseUnscaledTime = useUnscaledTime;
          Callback = callback;
      }
    
      public bool Update(TimeSpan scaledDelta, TimeSpan unscaledDelta)
      {
          if (Cancelled) return true;

          var dt = UseUnscaledTime ? unscaledDelta : scaledDelta;
          TimeLeft -= dt;

          if (TimeLeft <= TimeSpan.Zero)

          {
              Callback?.Invoke();

              if (Repeat)
              {
                  TimeLeft += Interval;
                  return false;
              }

              return true;
          }

          return false;
      }
  }
}
