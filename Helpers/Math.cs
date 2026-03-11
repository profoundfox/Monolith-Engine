using System;

namespace Monolith.Helpers
{
    public static class MathM
    {
        public static Random Random { get; set; } = new();

        public static float RandomFloat(float min, float max)
        {
            return (float)(Random.NextDouble() * (max - min)) + min;
        }
    }
}