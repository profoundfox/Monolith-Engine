namespace Monolith
{
    public sealed class EngineTime
    {
        public float FixedDelta { get; }
        public float FrameDelta { get; private set; }
        public float Alpha { get; private set; }

        private float _accumulator;
        private const int MaxSteps = 5;

        public EngineTime(float fixedDelta)
        {
            FixedDelta = fixedDelta;
        }

        public int Update(float frameDelta)
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
                _accumulator = 0f;

            Alpha = _accumulator / FixedDelta;
            return steps;
        }
    }
}
