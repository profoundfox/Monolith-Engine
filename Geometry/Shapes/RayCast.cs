using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Graphics;
using Monolith.Helpers;
using Monolith.Managers;
using Monolith.Structs;

namespace Monolith.Geometry
{
    public class RayCast
    {
        public Vector2 Origin { get; set; }
        public Vector2 TargetOffset { get; set; }

        public Vector2 Direction =>
            Vector2.Normalize(TargetOffset);

        public float Length =>
            TargetOffset.Length();

        private bool _hasHit;
        private Vector2 _hitPoint;

        public bool HasHit => _hasHit;
        public Vector2 HitPoint => _hitPoint;

        public bool CheckIntersections(IEnumerable<IRegionShape2D> shapes)
        {
            _hasHit = false;
            float closest = float.MaxValue;

            foreach (var shape in shapes)
            {
                if (shape.RayIntersect(
                    Origin,
                    Direction,
                    Length,
                    out Vector2 hit,
                    out float distance))
                {
                    if (distance < closest)
                    {
                        closest = distance;
                        _hasHit = true;
                        _hitPoint = hit;
                    }
                }
            }

            return _hasHit;
        }
    }
}
