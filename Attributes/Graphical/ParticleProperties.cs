
using Microsoft.Xna.Framework;

namespace Monolith.Graphics
{
    public record struct ParticleProperties
    {
        public Vector2 Velocity;
        public Vector2 Acceleration;
        public float LifeSpan;
        public float InitialSize;
        public float SizeRandomness;
        public float SizeGrowthFactor;
        public float SizeDecayFactor;
        public float Size;
        public Color Color;
        public float Rotation;
        public float Alpha;

        public static readonly ParticleProperties Identity = new
        (
            velocity: new Vector2(0, 50), acceleration: new Vector2(0f, 0f),
            lifeSpan: 2.0f, initialSize: 10, sizeRandomness: 0f, 
            sizeGrowthFactor: 0f, sizeDecayFactor: 0f, color: Color.White, rotation: 0f, alpha: 1f
        );

        public ParticleProperties(Vector2 velocity, Vector2 acceleration, float lifeSpan, float initialSize, float sizeRandomness, float sizeGrowthFactor, float sizeDecayFactor, Color color, float rotation, float alpha)
        {
            Velocity = velocity;
            Acceleration = acceleration;
            LifeSpan = lifeSpan;
            InitialSize = initialSize;
            SizeRandomness = sizeRandomness;
            SizeGrowthFactor = sizeGrowthFactor;
            SizeDecayFactor = sizeDecayFactor;
            Size = initialSize;
            Color = color;
            Rotation = rotation;
            Alpha = alpha;
        }
    }
}