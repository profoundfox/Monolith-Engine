using System;
using Microsoft.Xna.Framework;

namespace Monolith.Geometry
{
    public class CircleShape2D : IRegionShape2D, IEquatable<CircleShape2D>
    {
        public Point Location { get; set; }
        public int Radius { get; set; }

        public CircleShape2D(int x, int y, int radius)
        {
            Location = new Point(x, y);
            Radius = radius;
        }

        public CircleShape2D(Point location, int radius)
        {
            Location = location;
            Radius = radius;
        }

        public int Width
        {
            get => Radius * 2;
            set => Radius = value / 2;
        }

        public int Height
        {
            get => Radius * 2;
            set => Radius = value / 2;
        }

        public void Offset(int x, int y) => Location = new Point(Location.X + x, Location.Y + y);

        public Rectangle BoundingBox =>
            new Rectangle(Location.X - Radius, Location.Y - Radius, Radius * 2, Radius * 2);

        public bool Contains(Point p)
        {
            int dx = p.X - Location.X;
            int dy = p.Y - Location.Y;
            return dx * dx + dy * dy <= Radius * Radius;
        }

        public bool Contains(IRegionShape2D other)
        {
            return other switch
            {
                CircleShape2D c    => ContainsCircle(c),
                RectangleShape2D r => ContainsRect(r.BoundingBox),
                _                  => false
            };
        }

        private bool ContainsCircle(CircleShape2D other)
        {
            float dist = Vector2.Distance(Location.ToVector2(), other.Location.ToVector2());
            return dist + other.Radius <= Radius;
        }

        private bool ContainsRect(Rectangle r)
        {
            return Contains(r.Location) &&
                   Contains(new Point(r.Right, r.Top)) &&
                   Contains(new Point(r.Left, r.Bottom)) &&
                   Contains(new Point(r.Right, r.Bottom));
        }

        public bool Intersects(IRegionShape2D other)
        {
            return other switch
            {
                CircleShape2D c    => IntersectsCircle(c),
                RectangleShape2D r => IntersectsRectangle(r.BoundingBox),
                _                  => false
            };
        }

        private bool IntersectsCircle(CircleShape2D other)
        {
            int sum = Radius + other.Radius;
            float distSq = Vector2.DistanceSquared(Location.ToVector2(), other.Location.ToVector2());
            return distSq <= sum * sum;
        }

        private bool IntersectsRectangle(Rectangle r)
        {
            int closestX = Math.Clamp(Location.X, r.Left, r.Right);
            int closestY = Math.Clamp(Location.Y, r.Top, r.Bottom);
            int dx = Location.X - closestX;
            int dy = Location.Y - closestY;
            return dx * dx + dy * dy <= Radius * Radius;
        }

        public bool RayIntersect(Vector2 rayOrigin, Vector2 rayDir, float maxLength, out Vector2 hitPoint, out float distance)
        {
            float t = Vector2.Dot(Location.ToVector2() - rayOrigin, rayDir);

            Vector2 closest = rayOrigin + rayDir * t;
            float distToCenter = Vector2.Distance(closest, Location.ToVector2());

            if (distToCenter > Radius)
            {
                hitPoint = Vector2.Zero;
                distance = float.MaxValue;
                return false;
            }

            float dt = (float)Math.Sqrt(Radius * Radius - distToCenter * distToCenter);
            float t0 = t - dt;
            float t1 = t + dt;

            float hitT = (t0 >= 0) ? t0 : t1;

            if (hitT < 0 || hitT > maxLength)
            {
                hitPoint = Vector2.Zero;
                distance = float.MaxValue;
                return false;
            }

            distance = hitT;
            hitPoint = rayOrigin + rayDir * hitT;
            return true;
        }

        public IRegionShape2D Clone() => new CircleShape2D(Location, Radius);

        public bool Equals(CircleShape2D other) =>
            other != null && Location.Equals(other.Location) && Radius == other.Radius;

        public override bool Equals(object obj) => obj is CircleShape2D c && Equals(c);
        public override int GetHashCode() => HashCode.Combine(Location, Radius);
    }
}
