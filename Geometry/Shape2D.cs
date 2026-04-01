using System.Numerics;

namespace Monolith.Geometry
{
    public interface IShape2D
    {
        Vector2[] Vertices { get; }
        Extent Size { get; }
        bool Intersect(IShape2D otherShape, Vector2 thisPosition, Vector2 otherPosition);
    }
}