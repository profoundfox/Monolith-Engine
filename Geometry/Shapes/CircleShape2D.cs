using Microsoft.Xna.Framework;

namespace Monolith.Geometry
{
    public struct CircleShape2D : IShape2D
    {
        public int Radius { get; set; }

        public Point[] Vertices => new Point[0];

        public Extent Size
        {
            get => new Extent(Radius * 2, Radius * 2);
            set => Radius = value.Width / 2;
        }

        public CircleShape2D(int radius)
        {
            Radius = radius;
        }

        public bool Intersect(IShape2D otherShape, Point thisPosition, Point otherPosition)
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

        private bool CheckCircleIntersection(Point pos1, CircleShape2D c1, Point pos2, CircleShape2D c2)
        {
            int dx = pos1.X - pos2.X;
            int dy = pos1.Y - pos2.Y;
            int radiusSum = c1.Radius + c2.Radius;

            return dx * dx + dy * dy <= radiusSum * radiusSum;
        }

        private bool CheckRectangleIntersectionWithCircle(Point circlePos, CircleShape2D circle, Point rectPos, RectangleShape2D rectangle)
        {
            return Intersection.RectangleIntersectWithCircle(rectPos, rectangle, circlePos, circle);
        }

        public IShape2D Clone() => new CircleShape2D(Radius);

        public bool Contains(Point point, Point position)
        {
            int dx = point.X - position.X;
            int dy = point.Y - position.Y;
            return dx * dx + dy * dy <= Radius * Radius;
        }

        public Rectangle GetAABB(Point position)
        {
            int diameter = Radius * 2;
            return new Rectangle(
                position.X - Radius,
                position.Y - Radius,
                diameter,
                diameter
            );
        }

        public bool IntersectsAt(Point offset, IShape2D otherShape, Point thisPosition, Point otherPosition)
        {
            return Intersect(otherShape, thisPosition + offset, otherPosition);
        }

        public bool RayIntersect(Point rayOrigin, Point rayDir, float maxLength, out Point hitPoint, out float distance)
        {
            hitPoint = new Point();
            distance = 0;
            return false;
        }
    }
}