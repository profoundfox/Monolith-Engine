using System;
using Microsoft.Xna.Framework;

namespace ConstructEngine.Region
{
    public class RectangleShape2D : IRegionShape2D, IEquatable<RectangleShape2D>
    {
        public Rectangle Rect;

        public RectangleShape2D(int x, int y, int width, int height)
        {
            Rect = new Rectangle(x, y, width, height);
        }

        public RectangleShape2D(Rectangle rect)
        {
            Rect = rect;
        }

        public Point Location
        {
            get => Rect.Location;
            set => Rect.Location = value;
        }

        public int X
        {
            get => Location.X;
            set => Location = new Point(value, Y);
        }

        public int Y
        {
            get => Location.Y;
            set => Location = new Point(X, value);
        }

        public int Width
        {
            get => Rect.Width;
            set => Rect.Width = value;
        }

        public int Height
        {
            get => Rect.Height;
            set => Rect.Height = value;
        }

        public void Offset(int x, int y) => Rect.Offset(x, y);

        public Rectangle BoundingBox => Rect;

        public bool Contains(Point p) => Rect.Contains(p);

        public bool Contains(IRegionShape2D other)
        {
            return other switch
            {
                RectangleShape2D r => Rect.Contains(r.Rect),
                CircleShape2D c    => Rect.Contains(c.BoundingBox),
                _                  => false
            };
        }

        public bool Intersects(IRegionShape2D other)
        {
            return other switch
            {
                RectangleShape2D r => Rect.Intersects(r.Rect),
                CircleShape2D c    => c.Intersects(this),
                _                  => false
            };
        }

        public bool RayIntersect(Vector2 rayOrigin, Vector2 rayDir, float maxLength, out Vector2 hitPoint, out float distance)
        {
            hitPoint = Vector2.Zero;
            distance = float.MaxValue;

            Vector2 rayEnd = rayOrigin + rayDir * maxLength;

            Vector2[] corners =
            {
                new Vector2(BoundingBox.Left,  BoundingBox.Top),
                new Vector2(BoundingBox.Right, BoundingBox.Top),
                new Vector2(BoundingBox.Right, BoundingBox.Bottom),
                new Vector2(BoundingBox.Left,  BoundingBox.Bottom)
            };

            bool hit = false;

            for (int i = 0; i < 4; i++)
            {
                Vector2 p1 = corners[i];
                Vector2 p2 = corners[(i + 1) % 4];

                if (RayCast2D.LineIntersects(rayOrigin, rayEnd, p1, p2, out Vector2 pt))
                {
                    float d = Vector2.Distance(rayOrigin, pt);

                    if (d < distance)
                    {
                        distance = d;
                        hitPoint = pt;
                        hit = true;
                    }
                }
            }

            return hit;
        }

        public IRegionShape2D Clone() => new RectangleShape2D(Rect);

        public bool Equals(RectangleShape2D other) => other != null && Rect.Equals(other.Rect);
        public override bool Equals(object obj) => obj is RectangleShape2D r && Equals(r);
        public override int GetHashCode() => Rect.GetHashCode();
    }
}
