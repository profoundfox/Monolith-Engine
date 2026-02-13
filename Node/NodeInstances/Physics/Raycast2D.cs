using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Monolith.Geometry;

namespace Monolith.Nodes
{
    public record class RayCastConfig : SpatialNodeConfig
    {
        public Vector2 TargetPosition { get; set; } = new Vector2(0, 50);

        public override Type NodeType => typeof(RayCast2D);

    }

    public class RayCast2D : Node2D
    {
        public Vector2 TargetPosition { get; set; }

        public bool Disabled { get; set; }

        public readonly RayCastShape2D Ray = new RayCastShape2D();

        public RayCast2D(RayCastConfig cfg) : base(cfg)
        {
            TargetPosition = cfg.TargetPosition;
        }

        public override void PhysicsUpdate(float delta)
        {
            if (Disabled)
                return;

            Ray.Origin = GlobalTransform.Position;

            Vector2 worldOffset = Vector2.Transform(
                TargetPosition,
                Matrix.CreateRotationZ(
                    MathHelper.ToRadians(GlobalTransform.Rotation)
                )
            );

            Ray.TargetOffset = worldOffset;

            var shapes = Engine.Node
                .GetAll<CollisionShape2D>()
                .Where(cs => cs.Shape != null)
                .Select(cs => cs.Shape)
                .ToList();


            Ray.CheckIntersections(shapes);
        }

        public bool IsColliding => Ray.HasHit;
        public Vector2 CollisionPoint => Ray.HitPoint;
    }
}
