using System;
using Microsoft.Xna.Framework;

namespace ConstructEngine.Area
{
    public class CircleShape2D : IRegionShape2D, IEquatable<CircleShape2D>
    {
        private static readonly CircleShape2D s_empty = new CircleShape2D(0, 0, 0);

        public int X { get; set; }
        public int Y { get; set; }
        public int Radius { get; set; }
        private bool Enabled { get; set; }

        public Point Location
        {
            get => new Point(X, Y);
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        public static CircleShape2D Empty => s_empty;
        public bool IsEmpty => X == 0 && Y == 0 && Radius == 0;

        public int Top => Y - Radius;
        public int Bottom => Y + Radius;
        public int Left => X - Radius;
        public int Right => X + Radius;

        public CircleShape2D(int x, int y, int radius, bool enabled = true)
        {
            Enabled = enabled;
            X = x;
            Y = y;
            Radius = radius;
        }

        public CircleShape2D(Point location, int radius, bool enabled = true)
        {
            Enabled = enabled;
            X = location.X;
            Y = location.Y;
            Radius = radius;
        }

        public bool Contains(Point p)
        {
            int dx = p.X - X;
            int dy = p.Y - Y;
            return dx * dx + dy * dy <= Radius * Radius;
        }

        public bool Intersects(IRegionShape2D other)
        {
            return other switch
            {
                CircleShape2D c    => Intersects(c),
                RectangleShape2D r => IntersectsRectangle(r.Rect),
                _           => false
            };
        }

        public bool Intersects(CircleShape2D other)
        {
            int radiiSquared = (Radius + other.Radius) * (Radius + other.Radius);
            float distanceSquared = Vector2.DistanceSquared(
                Location.ToVector2(),
                other.Location.ToVector2()
            );
            return distanceSquared < radiiSquared;
        }

        public bool IntersectsRectangle(Rectangle r)
        {
            int closestX = Math.Clamp(X, r.Left, r.Right);
            int closestY = Math.Clamp(Y, r.Top, r.Bottom);

            int dx = X - closestX;
            int dy = Y - closestY;

            return dx * dx + dy * dy <= Radius * Radius;
        }

        public override bool Equals(object obj) =>
            obj is CircleShape2D other && Equals(other);

        public bool Equals(CircleShape2D other)
        {
            if (other is null) return false;

            return X == other.X &&
                   Y == other.Y &&
                   Radius == other.Radius;
        }

        public override int GetHashCode() =>
            HashCode.Combine(X, Y, Radius);

        public static bool operator ==(CircleShape2D lhs, CircleShape2D rhs)
        {
            if (ReferenceEquals(lhs, rhs)) return true;
            if (lhs is null || rhs is null) return false;
            return lhs.Equals(rhs);
        }

        public static bool operator !=(CircleShape2D lhs, CircleShape2D rhs) =>
            !(lhs == rhs);
    }
}
