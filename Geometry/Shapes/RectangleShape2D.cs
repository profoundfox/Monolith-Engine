using System;
using Microsoft.Xna.Framework;
using Monolith.Graphics;
using Monolith.Managers;
using Monolith.Structs;

namespace Monolith.Geometry
{
    public class RectangleShape2D : IRegionShape2D, IEquatable<RectangleShape2D>
    {
        public Rectangle Rect;


        public RectangleShape2D(int width, int height)
        {
            Rect = new Rectangle(0, 0, width, height);
        }
        
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
            public void Offset(Point point) => Rect.Offset(point.ToVector2());


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

        public bool RayIntersect(
            Vector2 rayOrigin,
            Vector2 rayDir,
            float maxLength,
            out Vector2 hitPoint,
            out float distance)
        {
            hitPoint = Vector2.Zero;
            distance = 0f;

            Rectangle r = BoundingBox;

            float tmin = 0f;
            float tmax = maxLength;

            if (Math.Abs(rayDir.X) < float.Epsilon)
            {
                if (rayOrigin.X < r.Left || rayOrigin.X > r.Right)
                    return false;
            }
            else
            {
                float inv = 1f / rayDir.X;
                float t1 = (r.Left - rayOrigin.X) * inv;
                float t2 = (r.Right - rayOrigin.X) * inv;

                if (t1 > t2) (t1, t2) = (t2, t1);

                tmin = Math.Max(tmin, t1);
                tmax = Math.Min(tmax, t2);

                if (tmin > tmax)
                    return false;
            }

            if (Math.Abs(rayDir.Y) < float.Epsilon)
            {
                if (rayOrigin.Y < r.Top || rayOrigin.Y > r.Bottom)
                    return false;
            }
            else
            {
                float inv = 1f / rayDir.Y;
                float t1 = (r.Top - rayOrigin.Y) * inv;
                float t2 = (r.Bottom - rayOrigin.Y) * inv;

                if (t1 > t2) (t1, t2) = (t2, t1);

                tmin = Math.Max(tmin, t1);
                tmax = Math.Min(tmax, t2);

                if (tmin > tmax)
                    return false;
            }

            distance = tmin;
            hitPoint = rayOrigin + rayDir * distance;
            return true;
        }


        public IRegionShape2D Clone() => new RectangleShape2D(Rect);

        public bool Equals(RectangleShape2D other) => other != null && Rect.Equals(other.Rect);
        public override bool Equals(object obj) => obj is RectangleShape2D r && Equals(r);
        public override int GetHashCode() => Rect.GetHashCode();

        public void Draw()
        {
            Color color = Color.Red;
            int depth = 99;
            int thickness = 2;

            Engine.Screen.Draw(
                new TextureDrawCall
                {
                    Position = new Vector2(X, Y),
                    Texture = Engine.Pixel,
                    Color = color,
                    Scale = new Vector2(BoundingBox.Width, thickness),
                    Depth = depth
                },
                DrawLayer.Middleground
            );
            

            Engine.Screen.Draw(
                new TextureDrawCall
                {
                    Position = new Vector2(X, BoundingBox.Bottom - thickness),
                    Texture = Engine.Pixel,
                    Color = color,
                    Scale = new Vector2(BoundingBox.Width, thickness),
                    Depth = depth
                },
                DrawLayer.Middleground
            );

            Engine.Screen.Draw(
                new TextureDrawCall
                {
                    Position = new Vector2(X, Y),
                    Texture = Engine.Pixel,
                    Color = color,
                    Scale = new Vector2(thickness, BoundingBox.Height),
                    Depth = depth
                },
                DrawLayer.Middleground
            );

            Engine.Screen.Draw(
                new TextureDrawCall
                {
                    Position = new Vector2(BoundingBox.Right - thickness, Y),
                    Texture = Engine.Pixel,
                    Color = color,
                    Scale = new Vector2(thickness, BoundingBox.Height),
                    Depth = depth
                },
                DrawLayer.Middleground
            );
        }
    }
}
