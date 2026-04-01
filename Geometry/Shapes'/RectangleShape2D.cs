using System;
using Microsoft.Xna.Framework;

namespace Monolith.Geometry
{
    public struct RectangleShape2D : IShape2D
    {
        public float Width { get; set; }
        public float Height { get; set; }

        private Vector2[] _vertices;

        public Vector2[] Vertices
        {
            get
            {
                if (_vertices == null)
                {
                    _vertices = new Vector2[]
                    {
                        new Vector2(0, 0),
                        new Vector2(Width, 0),
                        new Vector2(Width, Height),
                        new Vector2(0, Height)
                    };
                }
                return _vertices;
            }
        }

        public Extent Size => new Extent(Width, Height);

        public RectangleShape2D(float width, float height)
        {
            Width = width;
            Height = height;
            _vertices = null;
        }

        public bool Intersect(IShape2D otherShape, Vector2 thisPosition, Vector2 otherPosition)
        {
            if (otherShape is RectangleShape2D otherRect)
            {
                return AABBIntersect(thisPosition, this, otherPosition, otherRect);
            }

            if (otherShape is CircleShape2D otherCircle)
            {
                return ShapeTools.RectangleIntersectWithCircle(thisPosition, this, otherPosition, otherCircle);
            }

            return false;
        }

        private bool AABBIntersect(Vector2 pos1, RectangleShape2D rect1, Vector2 pos2, RectangleShape2D rect2)
        {
            var min1 = pos1;
            var max1 = pos1 + new Vector2(rect1.Width, rect1.Height);

            var min2 = pos2;
            var max2 = pos2 + new Vector2(rect2.Width, rect2.Height);

            return (min1.X <= max2.X && max1.X >= min2.X &&
                    min1.Y <= max2.Y && max1.Y >= min2.Y);
        }
    }
}