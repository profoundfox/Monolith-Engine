using System;
using System.Text.RegularExpressions;

namespace Monolith.Graphics
{
  public abstract class Layered : BaseObject
  {
    private int _depth;

    /// <summary>
    /// The maximum value of the depth
    /// </summary>
    public int Max { get; set; } = 100;
    /// <summary>
    /// The minimum value of the depth;
    /// </summary>
    public int Min { get; set; } = -100;

    /// <summary>
    /// The depth, reperesented in integer values.
    /// </summary>
    public int Depth
    {
      get => _depth;
      set => _depth = Math.Clamp(value, Min, Max);
    }

    /// <summary>
    /// The depth, reperesented in float values from 0f to 1f.
    /// </summary>
    public float InternalDepth
    {
      get
      {
        return 1f - (_depth - Min) / (float)(Max - Min);
      }
    }
  }
}
