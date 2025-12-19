using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Monolith.Geometry;

namespace Monolith.Helpers
{
    public enum CollisionSide
    {
        None,
        Left,
        Right,
        Top,
        Bottom
    }

    public static class CollisionHelper
    {
        public static bool IsRectangleEmpty(Rectangle collider)
        {
            if (collider.X == 0 && collider.Y == 0 && collider.Width == 0 && collider.Height == 0)
            {
                return true;
            }

            return false;

        }

            
        public static bool CircleIntersectsRectangle(CircleShape2D circle, Rectangle rect)
        {
            int closestX = Math.Clamp(circle.Location.X, rect.Left, rect.Right);
            int closestY = Math.Clamp(circle.Location.Y, rect.Top, rect.Bottom);

            int deltaX = circle.Location.X - closestX;
            int deltaY = circle.Location.Y - closestY;

            return (deltaX * deltaX + deltaY * deltaY) <= (circle.Radius * circle.Radius);
        }

        public static CollisionSide GetCameraEdge(IRegionShape2D target, IRegionShape2D camera)
        {

            if (target.BoundingBox.Right > camera.BoundingBox.Right)
            {
                return CollisionSide.Right;
            }

            if (target.BoundingBox.Left < camera.BoundingBox.Left)
            {
                return CollisionSide.Left;
            }

            if (target.BoundingBox.Top < camera.BoundingBox.Top)
            {
                return CollisionSide.Top;
            }

            if (target.BoundingBox.Bottom > camera.BoundingBox.Bottom)
            {
                return CollisionSide.Bottom;
            }
            

            return CollisionSide.None;
        }
    }
}