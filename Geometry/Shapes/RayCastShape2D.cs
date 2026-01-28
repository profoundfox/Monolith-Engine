using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Graphics;
using Monolith.Helpers;
using Monolith.Managers;
using Monolith.Attributes;

namespace Monolith.Geometry
{
    public class RayCastShape2D
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

        public void Draw()
        {
            Color color = HasHit ? Color.Yellow : Color.Red;
            int depth = 99;
            int thickness = 2;

            Engine.Screen.Call(
                new TextureDrawCall
                {
                    Position = Origin,
                    Texture = Engine.Pixel,
                    Color = color,
                    Scale = new Vector2(Length, thickness),
                    Rotation = MathF.Atan2(Direction.Y, Direction.X),
                    Origin = new Vector2(0f, 0.5f),
                    Depth = depth
                },
                DrawLayer.Middleground
            );

            if (HasHit)
            {
                Engine.Screen.Call(
                    new TextureDrawCall
                    {
                        Position = HitPoint,
                        Texture = Engine.Pixel,
                        Color = Color.Blue,
                        Scale = new Vector2(4, 4),
                        Origin = new Vector2(0.5f),
                        Depth = depth + 1
                    },
                    DrawLayer.Middleground
                );
            }
        }

    }
}
