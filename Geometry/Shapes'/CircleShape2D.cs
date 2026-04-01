using Microsoft.Xna.Framework;

namespace Monolith.Geometry
{
    public struct CircleShape2D : IShape2D
    {
        public float Radius { get; set; }

        public Vector2[] Vertices => new Vector2[0];

        public Extent Size => new Extent(Radius * 2, Radius * 2);

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
    }
}