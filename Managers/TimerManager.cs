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
        public void Wait(float seconds, Action callback)
        {
            if (seconds < 0) throw new ArgumentOutOfRangeException(nameof(seconds));

            timers.Add(new Timer
            {
                TimeLeft = seconds,
                Interval = seconds,
                Callback = callback,
                Repeat = false
            });
        }

        /// <summary>
        /// Add a repeating timer in seconds.
        /// </summary>
        public void Repeat(float interval, Action callback)
        {
            if (interval <= 0) throw new ArgumentOutOfRangeException(nameof(interval));

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
        public Action WaitCancelable(float seconds, Action callback)
        {
            if (seconds < 0) throw new ArgumentOutOfRangeException(nameof(seconds));

            var timer = new Timer
            {
                TimeLeft = seconds,
                Interval = seconds,
                Callback = callback,
                Repeat = false
            };

            timers.Add(timer);

            return () => timer.Cancelled = true;
        }

        /// <summary>
        /// Update all timers. Call this every frame from Game.Update().
        /// </summary>
        public void PhysicsUpdate(float deltaTime)
        {
            for (int i = timers.Count - 1; i >= 0; i--)
            {
                var t = timers[i];

                if (t.Cancelled)
                {
                    timers.RemoveAt(i);
                    continue;
                }

                t.TimeLeft -= Engine.DeltaTime;

                if (t.TimeLeft <= 0f)
                {
                    t.Callback?.Invoke();

                    if (t.Repeat)
                    {
                        t.TimeLeft = t.Interval;
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
