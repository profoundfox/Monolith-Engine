using Microsoft.Xna.Framework;
using Monolith.Geometry;

namespace Monolith.Helpers
{
    public static class ShapeHelper
    {
        public static RectangleShape2D ToShape(this Rectangle normRect)
        {
            return new RectangleShape2D(normRect);
        }

        public static Rectangle ToRectangle(this RectangleShape2D rectShape)
        {
            return new Rectangle(rectShape.X, rectShape.Y, rectShape.Width, rectShape.Height);
        }
    }
}