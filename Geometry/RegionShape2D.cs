using System.Collections.Generic;
using Microsoft.Xna.Framework;


namespace Monolith.Geometry
{
    public interface IRegionShape2D
    {
        int Width { get; set; }
        int Height { get; set; }
        
        Point Location { get; set; }
        void Offset(int x, int y);
        void Offset(Point point);

        Rectangle BoundingBox { get; }

        bool Contains(Point p);
        bool Contains(IRegionShape2D other);
        bool Intersects(IRegionShape2D other);
        bool RayIntersect(Vector2 rayOrigin, Vector2 rayDir, float maxLength, out Vector2 hitPoint, out float distance);

        IRegionShape2D Clone();

        void Draw();
    }


}