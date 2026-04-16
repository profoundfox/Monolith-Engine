using Monolith.Graphics;

namespace Monolith.Params
{
    public record struct EmitterParams
    {
        public ParticleParams Params { get; set; }
        public float Angle { get; set; }
        public float AngleVariance { get; set; }
        public float LifespanMin { get; set; }
        public float LifespanMax { get; set; }
        public float SpeedMin { get; set; }
        public float SpeedMax { get; set; }
        public float Interval { get; set; }
        public int EmitCount { get; set; }

        public static readonly EmitterParams Identity = new(
            particleParams: ParticleParams.Identity, angle: 0f, angleVariance: 45f, lifespanMin: 0.1f,
            lifspanMax: 2f, speedMin: 10f, speedMax: 100f,
            interval: 1f, emitCount: 1
        );

        public EmitterParams(
            ParticleParams particleParams, float angle, float angleVariance, float lifespanMin,
            float lifspanMax, float speedMin, float speedMax,
            float interval, int emitCount)
        {
            Params = particleParams;
            Angle = angle;
            AngleVariance = angleVariance;
            LifespanMin = lifespanMin;
            LifespanMax = lifspanMax;
            SpeedMin = speedMin;
            SpeedMax = speedMax;
            Interval = interval;
            EmitCount = emitCount;
        }
    }
}
