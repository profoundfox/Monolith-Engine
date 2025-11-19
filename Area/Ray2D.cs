using System;
using System.Collections.Generic;
using ConstructEngine.Area;
using Microsoft.Xna.Framework;

namespace ConstructEngine.Area
{
    public class Ray2D
    {
        public static List<Ray2D> RayList = new();
        public Vector2 Position { get; set; }

        /// <summary>
        /// 0° is Right | 90° is Up | 180° is Left | 270° is Down
        /// </summary>
        public float AngleDegrees { get; set; }
        public float Length { get; set; }
        private bool _hasHit;
        private Vector2 _hitPoint;
        public bool HasHit => _hasHit;
        public Vector2 HitPoint => _hitPoint;

        /// <summary>
        /// Converts the angle to the direction
        /// </summary>
        public Vector2 Direction => new Vector2(
            MathF.Cos(MathHelper.ToRadians(AngleDegrees)),
            MathF.Sin(MathHelper.ToRadians(AngleDegrees))
        );

        public Ray2D(Vector2 position, float angleDegrees, float length)
        {
            Position = position;
            AngleDegrees = angleDegrees;
            Length = length;

            _hasHit = false;
            _hitPoint = Vector2.Zero;

            RayList.Add(this);
        }

        /// <summary>
        /// Updates the position, length and degrees
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="angleDegrees"></param>
        /// <param name="length"></param>

        public void Update(Vector2 pos, float angleDegrees, float length)
        {
            //Length = length;
            Position = pos;
            AngleDegrees = angleDegrees;
        }

        /// <summary>
        /// Casts a ray and checks for intersections based on a list of areas
        /// </summary>
        /// <param name="areas"></param>
        /// <returns></returns>
        public bool CheckIntersection(List<Area2D> areas)
        {
            float maxLength = Length;
            float closestDistance = float.MaxValue;
            _hasHit = false;
            _hitPoint = Vector2.Zero;

            foreach (var area in areas)
            {
                Vector2 currentHitPoint;
                float currentDistance;
                bool hit = false;

                hit = CheckRaycast(this, area, maxLength, out currentHitPoint, out currentDistance);

                if (hit && currentDistance < closestDistance)
                {
                    closestDistance = currentDistance;
                    _hitPoint = currentHitPoint;
                    _hasHit = true;
                }
            }

            return _hasHit;
        }

        /// <summary>
        /// Casts a ray and checks for intersections based on a list of areas and a type
        /// </summary>
        /// <param name="areas"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool CheckIntersection(List<Area2D> areas, Type target)
        {
            float maxLength = Length;
            float closestDistance = float.MaxValue;
            _hasHit = false;
            _hitPoint = Vector2.Zero;

            foreach (var area in areas)
            {
                if (area.RootType != target) continue;

                Vector2 currentHitPoint;
                float currentDistance;
                bool hit = false;

                hit = CheckRaycast(this, area, maxLength, out currentHitPoint, out currentDistance);

                if (hit && currentDistance < closestDistance)
                {
                    closestDistance = currentDistance;
                    _hitPoint = currentHitPoint;
                    _hasHit = true;
                }
            }

            return _hasHit;
        }

        /// <summary>
        /// Checks for intersections from all areas
        /// </summary>
        /// <returns></returns>
        public bool CheckIntersectionAny()
        {
            return CheckIntersection(Area2D.AreaList);
        }

        /// <summary>
        /// Rectangle intersection with the ray
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="rect"></param>
        /// <param name="maxLength"></param>
        /// <param name="hitPoint"></param>
        /// <param name="distance"></param>
        /// <returns></returns>

        private static bool RectangleIntersect(Ray2D ray, Rectangle rect, float maxLength, out Vector2 hitPoint, out float distance)
        {
            hitPoint = Vector2.Zero;
            distance = float.MaxValue;
            bool hasHit = false;

            Vector2 rayEnd = ray.Position + ray.Direction * maxLength;

            Vector2[] corners =
            {
                new Vector2(rect.Left, rect.Top),
                new Vector2(rect.Right, rect.Top),
                new Vector2(rect.Right, rect.Bottom),
                new Vector2(rect.Left, rect.Bottom)
            };

            for (int i = 0; i < 4; i++)
            {
                Vector2 p1 = corners[i];
                Vector2 p2 = corners[(i + 1) % 4];

                if (LineIntersects(ray.Position, rayEnd, p1, p2, out Vector2 intersectionPoint))
                {
                    float currentDistance = Vector2.Distance(ray.Position, intersectionPoint);
                    if (currentDistance < distance)
                    {
                        distance = currentDistance;
                        hitPoint = intersectionPoint;
                        hasHit = true;
                    }
                }
            }

            return hasHit;
        }

        /// <summary>
        /// CircleShape2D intersection with the ray
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="circ"></param>
        /// <param name="maxLength"></param>
        /// <param name="hitPoint"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        private static bool CircleShape2DIntersect(Ray2D ray, CircleShape2D circ, float maxLength, out Vector2 hitPoint, out float distance)
        {
            hitPoint = Vector2.Zero;
            distance = float.MaxValue;

            float t = Vector2.Dot(circ.Location.ToVector2() - ray.Position, ray.Direction);

            Vector2 closestPoint = ray.Position + ray.Direction * t;
            float distToCircleShape2D = Vector2.Distance(closestPoint, circ.Location.ToVector2());

            if (distToCircleShape2D > circ.Radius) return false;

            float dt = (float)Math.Sqrt(circ.Radius * circ.Radius - distToCircleShape2D * distToCircleShape2D);
            float t0 = t - dt;
            float t1 = t + dt;

            float hitT = (t0 >= 0) ? t0 : t1;
            if (hitT < 0) return false;

            hitPoint = ray.Position + ray.Direction * hitT;
            distance = hitT;

            return true;
        }


        /// <summary>
        /// Universal check that checks for both rectangle and CircleShape2D intersection
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="area"></param>
        /// <param name="maxLength"></param>
        /// <param name="hitPoint"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        private static bool CheckRaycast(Ray2D ray, Area2D area, float maxLength, out Vector2 hitPoint, out float distance)
        {
            if (area.HasRect)
                return RectangleIntersect(ray, area.Rect, maxLength, out hitPoint, out distance);
            else if (area.HasCircle)
                return CircleShape2DIntersect(ray, area.Circ, maxLength, out hitPoint, out distance);

            hitPoint = Vector2.Zero;
            distance = float.MaxValue;
            return false;
        }

        /// <summary>
        /// Line intersection logic
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <param name="intersectionPoint"></param>
        /// <returns></returns>
        private static bool LineIntersects(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 intersectionPoint)
        {
            intersectionPoint = Vector2.Zero;
            float denominator = ((p4.Y - p3.Y) * (p2.X - p1.X)) - ((p4.X - p3.X) * (p2.Y - p1.Y));

            if (Math.Abs(denominator) < float.Epsilon)
                return false;

            float uA = (((p4.X - p3.X) * (p1.Y - p3.Y)) - ((p4.Y - p3.Y) * (p1.X - p3.X))) / denominator;
            float uB = (((p2.X - p1.X) * (p1.Y - p3.Y)) - ((p2.Y - p1.Y) * (p1.X - p3.X))) / denominator;

            if (uA >= 0 && uA <= 1 && uB >= 0 && uB <= 1)
            {
                intersectionPoint = p1 + uA * (p2 - p1);
                return true;
            }

            return false;
        }
    }
}
