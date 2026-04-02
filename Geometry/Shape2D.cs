
using Microsoft.Xna.Framework;

namespace Monolith.Geometry
{
    public interface IShape2D
    {
        Vector2[] Vertices { get; }
        Extent Size { get; set; }
        bool Intersect(IShape2D otherShape, Vector2 thisPosition, Vector2 otherPosition);

        bool Contains(Vector2 point, Vector2 position);
        bool IntersectsAt(Vector2 offset, IShape2D otherShape, Vector2 thisPosition, Vector2 otherPosition);
        bool RayIntersect(Vector2 rayOrigin, Vector2 rayDir, float maxLength, out Vector2 hitPoint, out float distance);

        IShape2D Clone();
    }
}