
using Microsoft.Xna.Framework;

namespace Monolith.Geometry
{
    public interface IShape2D
    {
        Point[] Vertices { get; }
        Extent Size { get; set; }

        Rectangle GetAABB(Point position);

        bool Intersect(IShape2D otherShape, Point thisPosition, Point otherPosition);

        bool Contains(Point point, Point position);
        bool IntersectsAt(Point offset, IShape2D otherShape, Point thisPosition, Point otherPosition);
        bool RayIntersect(Point rayOrigin, Point rayDir, float maxLength, out Point hitPoint, out float distance);

        IShape2D Clone();
    }
}