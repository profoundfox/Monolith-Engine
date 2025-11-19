using System;
using Microsoft.Xna.Framework;
using ConstructEngine.Area;
using System.Collections.Generic;
using System.Linq;

namespace ConstructEngine.Helpers
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
            int closestX = Math.Clamp(circle.X, rect.Left, rect.Right);
            int closestY = Math.Clamp(circle.Y, rect.Top, rect.Bottom);

            int deltaX = circle.X - closestX;
            int deltaY = circle.Y - closestY;

            return (deltaX * deltaX + deltaY * deltaY) <= (circle.Radius * circle.Radius);
        }
        public static CollisionSide GetCameraEdge(Area2D target, Rectangle camera)
        {
            if (target.HasRect)
            {
                if (target.Rect.Right > camera.Right)
                {
                    return CollisionSide.Right;
                }

                if (target.Rect.Left < camera.Left)
                {
                    return CollisionSide.Left;
                }

                if (target.Rect.Top < camera.Top)
                {
                    return CollisionSide.Top;
                }

                if (target.Rect.Bottom > camera.Bottom)
                {
                    return CollisionSide.Bottom;
                }
            }

            if (target.HasCircle)
            {
                if (target.Circ.Right > camera.Right)
                {
                    return CollisionSide.Right;
                }

                if (target.Circ.Left < camera.Left)
                {
                    return CollisionSide.Left;
                }

                if (target.Circ.Top < camera.Top)
                {
                    return CollisionSide.Top;
                }

                if (target.Circ.Bottom > camera.Bottom)
                {
                    return CollisionSide.Bottom;
                }
            }

            return CollisionSide.None;
        }
    }
}