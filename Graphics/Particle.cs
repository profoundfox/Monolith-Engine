
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Attributes;
using Monolith.Managers;

namespace Monolith.Graphics
{
    public struct Particle
    {
        public Vector2 Position;
        public ParticleProperties Properties;
        public float TimeAlive;

        private static Random rnd;

        public Particle(Vector2 position, ParticleProperties properties)
        {
            Position = position;
            Properties = properties;

            rnd = new();
        }

        public void Update(float deltaTime)
        {
            TimeAlive += deltaTime;

            Properties.Velocity += Properties.Acceleration * deltaTime;
            Position += Properties.Velocity * deltaTime;

            Properties.Alpha = MathHelper.Clamp(1f - (TimeAlive / Properties.LifeSpan), 0f, 1f);

            if (TimeAlive == 0f) 
            {
                Properties.Size += (float)(rnd.NextDouble() - 0.5f) * Properties.SizeRandomness;
            }

            float normalizedTime = TimeAlive / Properties.LifeSpan;

            Properties.Size = Properties.InitialSize * (1f + Properties.SizeGrowthFactor * normalizedTime);

            if (normalizedTime > 0)
            {
                Properties.Size *= (1f - (float)Math.Exp(-normalizedTime * Properties.SizeDecayFactor));
            }

            Properties.Color = Color.Lerp(Color.White, Color.Red, normalizedTime);
        }

        public bool IsExpired => TimeAlive >= Properties.LifeSpan;
    }
}