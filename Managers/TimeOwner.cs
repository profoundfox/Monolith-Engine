using System;
using Monolith.Util;

namespace Monolith.Managers
{
    public sealed class TimeOwner
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

        public int Update(TimeSpan rawDelta)
        {
            if (TimeScale == 0.0)
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

            Alpha = _accumulator.Ticks / _fixedDelta.Ticks;

            return steps;
        }

        public TimeContext GetContext()
        {
            return new TimeContext(_frameDelta, _fixedDelta, _accumulator, Alpha);
        }
    }
}
