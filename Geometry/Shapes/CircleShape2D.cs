using Microsoft.Xna.Framework;

namespace Monolith.Geometry
{
    public struct CircleShape2D : IShape2D
    {
        public float Radius { get; set; }

        public Vector2[] Vertices => new Vector2[0];

        public Extent Size
        {
            get => new Extent(Radius * 2, Radius * 2);
            set
            {
                Radius = value.Width / 2; 
            }
        }

        public CircleShape2D(float radius)
        {
            Radius = radius;
        }

        public bool Intersect(IShape2D otherShape, Vector2 thisPosition, Vector2 otherPosition)
        {
            switch (otherShape)
            {
                case CircleShape2D otherCircle:
                    return CheckCircleIntersection(thisPosition, this, otherPosition, otherCircle);

                case RectangleShape2D otherRectangle:
                    return CheckRectangleIntersectionWithCircle(thisPosition, this, otherPosition, otherRectangle);

                default:
                    return false;
            }
        }

        private bool CheckCircleIntersection(Vector2 circle1Position, CircleShape2D circle1, Vector2 circle2Position, CircleShape2D circle2)
        {
            float distance = Vector2.Distance(circle1Position, circle2Position);
            return distance <= (circle1.Radius + circle2.Radius);
        }

        private bool CheckRectangleIntersectionWithCircle(Vector2 circlePosition, CircleShape2D circle, Vector2 rectPosition, RectangleShape2D rectangle)
        {
            return ShapeTools.RectangleIntersectWithCircle(rectPosition, rectangle, circlePosition, circle);
        }

        public IShape2D Clone()
        {
            return new CircleShape2D(Radius);
        }

        public bool Contains(Vector2 point, Vector2 position)
        {
            float distance = Vector2.Distance(point, position);
            return distance <= Radius;
        }

        public bool IntersectsAt(Vector2 offset, IShape2D otherShape, Vector2 thisPosition, Vector2 otherPosition)
        {
            return Intersect(otherShape, thisPosition + offset, otherPosition);
        }

        public bool RayIntersect(Vector2 rayOrigin, Vector2 rayDir, float maxLength, out Vector2 hitPoint, out float distance)
        {
            hitPoint = Vector2.Zero;
            distance = 0f;

            return false;
        }
    }
}