using System.Linq;
using Microsoft.Xna.Framework;
using Monolith.Geometry;

namespace Monolith.Nodes
{
    public record class RayCastConfig : SpatialNodeConfig
    {
        public Vector2 TargetPosition { get; set; } = new Vector2(0, 50);
    }

    public class RayCast2D : Node2D
    {
        public Vector2 TargetPosition { get; set; }

        public bool Disabled { get; set; }

        private readonly RayCast _ray = new RayCast();

        public RayCast2D(RayCastConfig cfg) : base(cfg)
        {
            TargetPosition = cfg.TargetPosition;
        }

        public override void PhysicsUpdate(float delta)
        {
            if (Disabled)
                return;

            _ray.Origin = GlobalTransform.Position;

            Vector2 worldOffset = Vector2.Transform(
                TargetPosition,
                Matrix.CreateRotationZ(
                    MathHelper.ToRadians(GlobalTransform.Rotation)
                )
            );

            _ray.TargetOffset = worldOffset;

            var shapes = Engine.Node
                .GetNodesByT<CollisionShape2D>()
                .Where(cs => cs.Shape != null)
                .Select(cs => cs.Shape)
                .ToList();


            _ray.CheckIntersections(shapes);
        }

        public bool IsColliding => _ray.HasHit;
        public Vector2 CollisionPoint => _ray.HitPoint;
    }
}
