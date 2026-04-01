using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monolith.Geometry
{
    public static class ShapeTools
    {
        public static bool RectangleIntersectWithCircle(Vector2 rectPos, RectangleShape2D rect, Vector2 circlePos, CircleShape2D circle)
        {
            float closestX = Math.Clamp(circlePos.X, rectPos.X, rectPos.X + rect.Width);
            float closestY = Math.Clamp(circlePos.Y, rectPos.Y, rectPos.Y + rect.Height);

            float distanceX = circlePos.X - closestX;
            float distanceY = circlePos.Y - closestY;
            float distanceSquared = distanceX * distanceX + distanceY * distanceY;

            return distanceSquared <= circle.Radius * circle.Radius;
        }
    }
}