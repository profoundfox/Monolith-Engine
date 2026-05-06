
using Microsoft.Xna.Framework;
using Monolith.Graphics;

namespace Monolith.Params
{
  public record struct ParticleParams
  {
    public MTexture Texture { get; set; }
    public float Lifespan { get; set; }
    public Color ColorStart { get; set; }
    public Color ColorEnd { get; set; }
    public float OpacityStart { get; set; }
    public float OpacityEnd { get; set; }
    public float SizeStart { get; set; }
    public float SizeEnd { get; set; }
    public float Speed { get; set; }
    public float Angle { get; set; }

    public static readonly ParticleParams Identity = new(
        texture: Core.Pixel, lifespan: 2f, colorStart: Color.Yellow,
        colorEnd: Color.Red, opacityStart: 1f,
        opacityEnd: 0f, sizeStart: 32f, sizeEnd: 4f,
        speed: 100f, angle: 0f
    );

    public ParticleParams(
        MTexture texture, float lifespan,
        Color colorStart, Color colorEnd, float opacityStart,
        float opacityEnd, float sizeStart, float sizeEnd,
        float speed, float angle)
    {
      Texture = texture;
      Lifespan = lifespan;
      ColorStart = colorStart;
      ColorEnd = colorEnd;
      OpacityStart = opacityStart;
      OpacityEnd = opacityEnd;
      SizeStart = sizeStart;
      SizeEnd = sizeEnd;
      Speed = speed;
      Angle = angle;
    }
  }

}
