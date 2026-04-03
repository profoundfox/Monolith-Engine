
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
        bool RayIntersect(
            Vector2 rayOrigin,
            Vector2 rayDir,
            float maxLength,
            Vector2 shapePosition,
            out Vector2 hitPoint,
            out float distance
        );
        
        IShape2D Clone();
    }
}