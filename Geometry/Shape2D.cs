
using Microsoft.Xna.Framework;

namespace Monolith.Geometry
{
    ///<summary>
    /// The interface for all shapes.
    ///</summary>
    public interface IShape2D
    {
        ///<summary>
        /// The vertices of this shape.
        /// E.g a rectangle has four vertices, a triangle has three.
        ///</summary>
        Point[] Vertices { get; }

        ///<summary>
        /// The width and height of this rectangle.
        ///</summary>
        Extent Size { get; set; }
        
        ///<summary>
        /// Gets the axis aligned bounding box of this shape.
        ///</summary>
        ///<param name="position"> </param>
        Rectangle GetAABB(Point position);
        
        ///<summary>
        /// Checks if this shape intersects with another specified shape.
        ///</summary>
        ///<param name="otherShape">The other shape.</param>
        ///<param name="thisPosition">The position of this shape.</param>
        ///<param name="otherPosition">The position of the other shape.</param>
        bool Intersect(IShape2D otherShape, Point thisPosition, Point otherPosition);
        
        ///<summary>
        /// Checks if this shape contains a specified point.
        ///</summary>
        ///<param name="point">The point in question.</param>
        ///<param name="position">The position of this shape.</param>
        bool Contains(Point point, Point position);

        ///<summary>
        /// Checks if this shape interesects with a specified shape at a specified offset.
        ///</summary>
        ///<param name="offset">The offset.</param>
        ///<param name="otherShape">The other shape.</param>
        ///<param name="thisPosition">The non offset position of this shape.</param>
        ///<param name="otherPosition">The non offset position of the other shape.</param>
        bool IntersectsAt(Point offset, IShape2D otherShape, Point thisPosition, Point otherPosition);
        
        bool RayIntersect(
            Vector2 rayOrigin,
            Vector2 rayDir,
            float maxLength,
            Vector2 shapePosition,
            out Vector2 hitPoint,
            out float distance
        );
        
        ///<summary>
        /// Clones this shape.
        ///</summary>
        IShape2D Clone();
    }
}
