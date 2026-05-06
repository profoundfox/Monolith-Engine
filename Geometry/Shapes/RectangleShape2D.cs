using System;
using Microsoft.Xna.Framework;

namespace Monolith.Geometry
{
  public struct RectangleShape2D : IShape2D
  {
    public int Width { get; set; }
    public int Height { get; set; }

    private Point[] _vertices;

    public Point[] Vertices
    {
      get
      {
        if (_vertices == null)
        {
          _vertices = new Point[]
          {
                        new Point(0, 0),
                        new Point(Width, 0),
                        new Point(Width, Height),
                        new Point(0, Height)
          };
        }
        return _vertices;
      }
    }

    public Extent Size
    {
      get => new Extent(Width, Height);
      set
      {
        Width = value.Width;
        Height = value.Height;
      }
    }

    public RectangleShape2D(Extent size)
    {
      Width = size.Width;
      Height = size.Height;
      _vertices = null;
    }

    public RectangleShape2D(int width, int height)
    {
      Width = width;
      Height = height;
      _vertices = null;
    }

    public bool Intersect(IShape2D otherShape, Point thisPosition, Point otherPosition)
    {
      if (otherShape is RectangleShape2D otherRect)
        return AABBIntersect(thisPosition, this, otherPosition, otherRect);

      if (otherShape is CircleShape2D otherCircle)
        return Intersection.RectangleIntersectWithCircle(thisPosition, this, otherPosition, otherCircle);

      return false;
    }

    public bool IntersectsAt(Point offset, IShape2D otherShape, Point thisPosition, Point otherPosition)
    {
      return Intersect(otherShape, thisPosition + offset, otherPosition);
    }

    private bool AABBIntersect(Point pos1, RectangleShape2D rect1, Point pos2, RectangleShape2D rect2)
    {
      int min1X = pos1.X;
      int min1Y = pos1.Y;
      int max1X = pos1.X + rect1.Width;
      int max1Y = pos1.Y + rect1.Height;

      int min2X = pos2.X;
      int min2Y = pos2.Y;
      int max2X = pos2.X + rect2.Width;
      int max2Y = pos2.Y + rect2.Height;

      return min1X < max2X && max1X > min2X &&
             min1Y < max2Y && max1Y > min2Y;
    }

    public Rectangle GetAABB(Point position)
    {
      if (Vertices == null || Vertices.Length == 0)
        return Rectangle.Empty;

      int minX = int.MaxValue;
      int minY = int.MaxValue;
      int maxX = int.MinValue;
      int maxY = int.MinValue;

      foreach (var v in Vertices)
      {
        var world = v + position;

        if (world.X < minX) minX = world.X;
        if (world.Y < minY) minY = world.Y;
        if (world.X > maxX) maxX = world.X;
        if (world.Y > maxY) maxY = world.Y;
      }

      return new Rectangle(minX, minY, maxX - minX, maxY - minY);
    }

    public bool Contains(Point point, Point position)
    {
      int minX = position.X;
      int minY = position.Y;
      int maxX = position.X + Width;
      int maxY = position.Y + Height;

      return point.X >= minX && point.X <= maxX &&
             point.Y >= minY && point.Y <= maxY;
    }

    public bool RayIntersect(
        Vector2 rayOrigin,
        Vector2 rayDir,
        float maxLength,
        Vector2 position,
        out Vector2 hitPoint,
        out float distance)
    {
      hitPoint = Vector2.Zero;
      distance = 0f;

      Rectangle r = GetAABB(position.ToPoint());

      float tmin = 0f;
      float tmax = maxLength;

      if (rayDir.X != 0f)
      {
        float inv = 1f / rayDir.X;
        float t1 = (r.Left - rayOrigin.X) * inv;
        float t2 = (r.Right - rayOrigin.X) * inv;

        if (t1 > t2) (t1, t2) = (t2, t1);

        tmin = MathF.Max(tmin, t1);
        tmax = MathF.Min(tmax, t2);

        if (tmin > tmax) return false;
      }
      else if (rayOrigin.X < r.Left || rayOrigin.X > r.Right)
      {
        return false;
      }

      if (rayDir.Y != 0f)
      {
        float inv = 1f / rayDir.Y;
        float t1 = (r.Top - rayOrigin.Y) * inv;
        float t2 = (r.Bottom - rayOrigin.Y) * inv;

        if (t1 > t2) (t1, t2) = (t2, t1);

        tmin = MathF.Max(tmin, t1);
        tmax = MathF.Min(tmax, t2);

        if (tmin > tmax) return false;
      }
      else if (rayOrigin.Y < r.Top || rayOrigin.Y > r.Bottom)
      {
        return false;
      }

      distance = tmin;
      hitPoint = rayOrigin + rayDir * tmin;

      return true;
    }

    public IShape2D Clone()
    {
      return new RectangleShape2D(Width, Height);
    }
  }
}
