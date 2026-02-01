using Microsoft.Xna.Framework;
using Monolith.Graphics;

namespace Monolith.Attributes
{
    public readonly record struct ParticleInfo
    {
        public MTexture Texture { get; init; }
        public float Lifespan { get; init; }
        public Color ColorStart { get; init; }
        public Color ColorEnd { get; init; }
        public float OpacityStart { get; init; }
        public float OpacityEnd { get; init; }

        public static readonly ParticleInfo Identity = 
            new(Engine.Pixel, 2f, Color.Yellow, Color.Red, 1f, 0f);

        public ParticleInfo(
            MTexture texture, 
            float lifespan, 
            Color colorStart, 
            Color colorEnd, 
            float opacityStart, 
            float opacityEnd)
        {
            Texture = texture;
            Lifespan = lifespan;
            ColorStart = colorStart;
            ColorEnd = colorEnd;
            OpacityStart = opacityStart;
            OpacityEnd = opacityEnd;
        }
    }
}