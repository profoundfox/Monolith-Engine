using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monolith.Util;

namespace Monolith.Managers
{
    /// <summary>
    /// Supports one-shot timers, repeating timers, and frame-based timing.
    /// </summary>
    public class TimerManager
    {
        private readonly List<Timer> timers = new();

        public void WaitFrames(int frames, Action callback)
        {
            if (frames <= 0) throw new ArgumentOutOfRangeException(nameof(frames));

            timers.Add(new Timer
            {
                FramesLeft = frames,
                Callback = callback,
                Repeat = false,
                UseUnscaledTime = false
            });
        }

        public void Wait(TimeSpan time, Action callback)
        {
            timers.Add(new Timer
            {
                TimeLeft = time,
                Interval = time,
                Callback = callback,
                Repeat = false,
                UseUnscaledTime = false
            });
        }

        public void WaitUnscaled(TimeSpan time, Action callback)
        {
            timers.Add(new Timer
            {
                TimeLeft = time,
                Interval = time,
                Callback = callback,
                Repeat = false,
                UseUnscaledTime = true
            });
        }

        public void Repeat(TimeSpan interval, Action callback)
        {
            timers.Add(new Timer
            {
                TimeLeft = interval,
                Interval = interval,
                Callback = callback,
                Repeat = true,
                UseUnscaledTime = false
            });
        }

        public void RepeatUnscaled(TimeSpan interval, Action callback)
        {
            timers.Add(new Timer
            {
                TimeLeft = interval,
                Interval = interval,
                Callback = callback,
                Repeat = true,
                UseUnscaledTime = true
            });
        }

        /// <summary>
        /// Add a cancelable timer. Returns an action that cancels it.
        /// </summary>
        public Action WaitCancelable(TimeSpan time, Action callback)
        {
            if (time < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(time));

            var timer = new Timer
            {
                TimeLeft = time,
                Interval = time,
                Callback = callback,
                Repeat = false
            };

            timers.Add(timer);

            return () => timer.Cancelled = true;
        }

        /// <summary>
        /// Update all timers. Call this every frame from Game.Update().
        /// </summary>
        public void PhysicsUpdate(TimeSpan scaledDelta, TimeSpan unscaledDelta)
        {
            for (int i = timers.Count - 1; i >= 0; i--)
            {
                var t = timers[i];

                if (t.Cancelled)
                {
                    timers.RemoveAt(i);
                    continue;
                }

                if (t.FramesLeft > 0)
                {
                    t.FramesLeft--;
                    if (t.FramesLeft == 0)
                    {
                        t.Callback?.Invoke();
                        timers.RemoveAt(i);
                    }
                    continue;
                }

                TimeSpan dt = t.UseUnscaledTime ? unscaledDelta : scaledDelta;
                t.TimeLeft -= dt;

                if (t.TimeLeft <= TimeSpan.Zero)
                {
                    t.Callback?.Invoke();

                    if (t.Repeat)
                        t.TimeLeft += t.Interval;
                    else
                        timers.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Clear all timers.
        /// </summary>
        public void ClearAll()
        {
            timers.Clear();
        }
    }
}
