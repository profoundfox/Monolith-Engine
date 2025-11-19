using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ConstructEngine.Helpers;
using System;

namespace ConstructEngine.Area
{
    public class Area2D
    {
        public static List<Area2D> AreaList = new List<Area2D>();
        public static List<CircleShape2D> CircleShape2DList = new List<CircleShape2D>();
        public static List<Rectangle> RectangleList = new List<Rectangle>();
        public bool HasRect
        {
            get { return Rect != default; }
        }
        public bool HasCircle
        {
            get { return Circ != null; }
        }
        public Vector2 Position
        {
            get
            {
                if (HasCircle)
                    return new Vector2(Circ.X, Circ.Y);
                if (HasRect)
                    return new Vector2(Rect.X, Rect.Y);
                return Vector2.Zero;
            }
            set
            {
                if (HasCircle)
                {
                    Circ.X = (int)value.X;
                    Circ.Y = (int)value.Y;
                }
                else if (HasRect)
                {
                    var rect = Rect;
                    rect.X = (int)value.X;
                    rect.Y = (int)value.Y;
                    Rect = rect;
                }
            }
        }

        public Rectangle Rect;
        public CircleShape2D Circ;
        public object Root;
        public Type RootType { get; private set; }

        public bool Enabled;

        public Vector2 Velocity = Vector2.Zero;

        /// <summary>
        /// An Area2D with a rect, includes paramteres for a root and whether or not it is enabled.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="enabled"></param>
        /// <param name="root"></param>
        public Area2D(Rectangle rect, bool enabled, object root)
        {
            Rect = rect;
            Enabled = enabled;

            Root = root;

            RootType = Root.GetType();

            RectangleList.Add(Rect);

            AreaList.Add(this);
        }

        /// <summary>
        /// An area with a CircleShape2D, includes paramteres for a root and whether or not it is enabled.
        /// </summary>
        /// <param name="CircleShape2D"></param>
        /// <param name="enabled"></param>
        /// <param name="root"></param>

        public Area2D(CircleShape2D CircleShape2D, bool enabled, object root)
        {
            Circ = CircleShape2D;
            Enabled = enabled;

            Root = root;

            RootType = Root.GetType();

            CircleShape2DList.Add(Circ);

            AreaList.Add(this);
        }
        
        /// <summary>
        /// Frees the current area immediately
        /// </summary>

        public void Free()
        {
            AreaList.Remove(this);
        }

        /// <summary>
        /// Gets the information for the currently intersecting area
        /// </summary>
        /// <returns></returns>
        
        public Area2D GetCurrentlyIntersectingArea()
        {
            if (!this.Enabled) return null;

            foreach (var other in AreaList)
            {
                if (other == this || !other.Enabled) continue;

                if (this.HasRect && other.HasRect && this.Rect.Intersects(other.Rect))
                    return other;

                if (this.HasCircle && other.HasCircle && this.Circ.Intersects(other.Circ))
                    return other;

                if (this.HasRect && other.HasCircle && CollisionHelper.CircleIntersectsRectangle(other.Circ, this.Rect))
                    return other;

                if (this.HasCircle && other.HasRect && CollisionHelper.CircleIntersectsRectangle(this.Circ, other.Rect))
                    return other;
            }

            return null;
        }

        /// <summary>
        /// Checks if the area is intersecting with any area
        /// </summary>
        /// <returns></returns>

        public bool IsIntersectingAny()
        {
            if (!this.Enabled) return false;

            foreach (var other in AreaList)
            {
                if (other == this || !other.Enabled) continue;

                if (this.HasRect && other.HasRect && this.Rect.Intersects(other.Rect)) return true;
                if (this.HasCircle && other.HasCircle && this.Circ.Intersects(other.Circ)) return true;
                if (this.HasRect && other.HasCircle && CollisionHelper.CircleIntersectsRectangle(other.Circ, this.Rect)) return true;
                if (this.HasCircle && other.HasRect && CollisionHelper.CircleIntersectsRectangle(this.Circ, other.Rect)) return true;
            }

            return false;
        }
        /// <summary>
        /// Checks if the area is intersecting another target area
        /// </summary>
        /// <param name="otherArea"></param>
        /// <returns></returns>
        public bool Intersecting(Area2D otherArea)
        {
            if (otherArea == null || !Enabled || !otherArea.Enabled)
                return false;

            if (HasRect && otherArea.HasRect)
                return Rect.Intersects(otherArea.Rect);

            if (HasCircle && otherArea.HasCircle)
                return Circ.Intersects(otherArea.Circ);

            if (HasRect && otherArea.HasCircle)
                return CollisionHelper.CircleIntersectsRectangle(otherArea.Circ, Rect);

            if (HasCircle && otherArea.HasRect)
                return CollisionHelper.CircleIntersectsRectangle(Circ, otherArea.Rect);

            return false;
        }

    }
}