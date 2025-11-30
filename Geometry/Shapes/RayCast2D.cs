using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Monolith.Geometry
{
    public class RayCast2D
    {
        public Vector2 Position { get; set; }
        public float AngleDegrees { get; set; }
        public float Length { get; set; }

        private bool _hasHit;
        private Vector2 _hitPoint;

        public bool HasHit => _hasHit;
        public Vector2 HitPoint => _hitPoint;

        public Vector2 Direction => new Vector2(
            MathF.Cos(MathHelper.ToRadians(AngleDegrees)),
            MathF.Sin(MathHelper.ToRadians(AngleDegrees))
        );

        public RayCast2D(Vector2 position, float angleDegrees, float length)
        {
            Position = position;
            AngleDegrees = angleDegrees;
            Length = length;

            _hasHit = false;
            _hitPoint = Vector2.Zero;
        }

        public void Update(Vector2 pos, float angleDegrees, float length)
        {
            Position = pos;
            AngleDegrees = angleDegrees;
            Length = length;
        }

        // ------------------------------------------------------
        // Check a list of IRegionShape2D for intersection
        // ------------------------------------------------------
        public bool CheckIntersections(IEnumerable<IRegionShape2D> shapes)
        {
            float closestDistance = float.MaxValue;
            _hasHit = false;
            _hitPoint = Vector2.Zero;

            foreach (var shape in shapes)
            {
                if (shape.RayIntersect(Position, Direction, Length, out Vector2 hitPoint, out float distance))
                {
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        _hitPoint = hitPoint;
                        _hasHit = true;
                    }
                }
            }

            return _hasHit;
        }

        public bool CheckIntersection(IEnumerable<IRegionShape2D> shapes)
        {
            return CheckIntersections(shapes);
        }

        public bool CheckIntersectionAny(IEnumerable<IRegionShape2D> allShapes)
        {
            return CheckIntersections(allShapes);
        }

        // Optional: helper for line intersection
        public static bool LineIntersects(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 intersection)
        {
            intersection = Vector2.Zero;

            float denom = ((p4.Y - p3.Y) * (p2.X - p1.X)) - ((p4.X - p3.X) * (p2.Y - p1.Y));
            if (Math.Abs(denom) < float.Epsilon) return false;

            float uA = (((p4.X - p3.X) * (p1.Y - p3.Y)) - ((p4.Y - p3.Y) * (p1.X - p3.X))) / denom;
            float uB = (((p2.X - p1.X) * (p1.Y - p3.Y)) - ((p2.Y - p1.Y) * (p1.X - p3.X))) / denom;

            if (uA >= 0 && uA <= 1 && uB >= 0 && uB <= 1)
            {
                intersection = p1 + uA * (p2 - p1);
                return true;
            }

            return false;
        }
    }
}
