using System;

namespace Monolith.Tools
{
  public static class MathE
  {
    public static Random Random { get; set; } = new();

    public static float RandomFloat(float min, float max)
    {
      return (float)(Random.NextDouble() * (max - min)) + min;
    }
  }
}