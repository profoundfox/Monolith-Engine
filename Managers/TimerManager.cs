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

        /// <summary>
        /// Add a one-shot timer in seconds.
        /// </summary>
        public void Wait(TimeSpan time, Action callback)
        {
            if (time < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(time));

            timers.Add(new Timer
            {
                TimeLeft = time,
                Interval = time,
                Callback = callback,
                Repeat = false
            });
        }

        /// <summary>
        /// Add a repeating timer in seconds.
        /// </summary>
        public void Repeat(TimeSpan interval, Action callback)
        {  
            if (interval < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(interval));

            timers.Add(new Timer
            {
                TimeLeft = interval,
                Interval = interval,
                Callback = callback,
                Repeat = true
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
        public void PhysicsUpdate(TimeSpan deltaTime)
        {
            for (int i = timers.Count - 1; i >= 0; i--)
            {
                var t = timers[i];

                if (t.Cancelled)
                {
                    timers.RemoveAt(i);
                    continue;
                }

                t.TimeLeft -= deltaTime;

                if (t.TimeLeft <= TimeSpan.Zero)
                {
                    t.Callback?.Invoke();

                    if (t.Repeat)
                    {
                        t.TimeLeft += t.Interval;
                    }
                    else
                    {
                        timers.RemoveAt(i);
                    }
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
