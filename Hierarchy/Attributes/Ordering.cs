using System;
using Monolith.Managers;

namespace Monolith.Params
{
  public readonly record struct Ordering : IProperty<Ordering>
  {
    public int Depth { get; init; }
    public bool RelativeDepth { get; init; }
    
    public DrawLayer DrawLayer { get; init; }

    public static readonly Ordering Identity =
        new(0, true, DrawLayer.Middleground);

    public Ordering(int depth, bool relativeDepth, DrawLayer drawLayer)
    {
      Depth = depth;
      RelativeDepth = relativeDepth;
    }

    public static Ordering Combine(in Ordering parent, in Ordering child)
    {
      int depth = child.RelativeDepth ? parent.Depth + child.Depth : child.Depth;
      return new Ordering(depth, child.RelativeDepth, child.DrawLayer);
    }
  }
}
