using System;

namespace Monolith.Util
{
    public sealed class EngineTime
    {
        public TimeSpan FixedDelta { get; }
        public TimeSpan FrameDelta { get; private set; }
        public float Alpha { get; private set; }

        private TimeSpan _accumulator;
        private const int MaxSteps = 5;

        public EngineTime(TimeSpan fixedDelta)
        {
            FixedDelta = fixedDelta;
        }

        public int Update(TimeSpan frameDelta)
        {
            FrameDelta = frameDelta;
            _accumulator += frameDelta;

            int steps = 0;
            while (_accumulator >= FixedDelta && steps < MaxSteps)
            {
                _accumulator -= FixedDelta;
                steps++;
            }

            if (steps == MaxSteps)
                _accumulator = TimeSpan.Zero;

            Alpha = (float)(_accumulator.TotalMilliseconds / FixedDelta.TotalMilliseconds);
            return steps;
        }
    }
}