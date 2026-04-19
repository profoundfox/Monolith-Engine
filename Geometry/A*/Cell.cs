using Microsoft.Xna.Framework;

namespace Monolith.Geometry
{
  public class Cell
  {
    public Point Location { get; set; }
    public Cell Parent { get; set; }
    public int H { get; set; }
    public int G { get; set; }
    public int F => G + H;

    public Cell(Point location)
    {
      Location = location;
      G = int.MaxValue;
      H = 0;
      Parent = null;
    }

    public override bool Equals(object obj)
    {
      if (obj is Cell p)
        return Location == p.Location;
      return false;
    }

    public override int GetHashCode()
    {
      return Location.GetHashCode();
    }
  }
}