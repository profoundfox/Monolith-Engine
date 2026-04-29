using System;

namespace Monolith.Runtime
{
  public readonly struct TimeContext
  {
    public TimeSpan FrameDelta { get; }
    public TimeSpan FixedDelta { get; }
    public TimeSpan Accumulator { get; }
    public double Alpha { get; }

    public TimeContext(
        TimeSpan frameDelta,
        TimeSpan fixedDelta,
        TimeSpan accumulator,
        double alpha)
    {
      FrameDelta = frameDelta;
      FixedDelta = fixedDelta;
      Accumulator = accumulator;
      Alpha = alpha;
    }
  }
}
