using System;
using Microsoft.Xna.Framework;

namespace Monolith.Nodes
{
    public record class KinematicBodyConfig : SpatialNodeConfig {}

    public class KinematicBody2D : Node2D
    {
        public Vector2 Velocity = Vector2.Zero;
        public Vector2 WallNormal { get; private set; } = Vector2.Zero;

        public bool IsOnFloor => _isOnFloor;
        public bool IsOnWall => _isOnWall;
        public bool IsOnRoof => _isOnRoof;

        public Vector2 movement;

        public CollisionShape2D CollisionShape { get; set; }

        private CollisionShape2D _floorShape;
        private Vector2 _lastFloorPosition;

        private bool _isOnWall = false;
        private bool _isOnFloor = false;
        private bool _isOnRoof = false;

        const float FLOOR_TOLERANCE = 2f;
        const float WALL_TOLERANCE = 2f;

        public KinematicBody2D(KinematicBodyConfig cfg) : base(cfg) {}

        private void Move(float delta)
        {
            CollisionShape = (CollisionShape2D)GetFirstChildByT<CollisionShape2D>();
            if (CollisionShape == null)
                return;

            movement = Velocity * delta;

            if (_isOnFloor && _floorShape != null)
            {
                Vector2 platformDelta = _floorShape.GlobalPosition - _lastFloorPosition;
                LocalPosition += platformDelta;
                _lastFloorPosition = _floorShape.GlobalPosition;
            }

            _isOnFloor = false;
            _isOnRoof = false;
            _floorShape = null;
            _isOnWall = false;
            WallNormal = Vector2.Zero;

            LocalPosition += new Vector2(movement.X, 0);

            _isOnWall = false;
            WallNormal = Vector2.Zero;

            foreach (var other in Engine.Node.GetNodesByT<PhysicsBody2D>())
            {
                if (other.CollisionShape == CollisionShape || other.CollisionShape.Disabled)
                    continue;

                if (CollisionShape.Intersects(other.CollisionShape))
                {
                    _isOnWall = true;
                    WallNormal = movement.X > 0 ? new Vector2(-1, 0) : new Vector2(1, 0);
                    LocalPosition -= new Vector2(movement.X, 0);
                    Velocity = new Vector2(0, Velocity.Y);
                    break;
                }
            }

            if (!_isOnWall)
            {
                foreach (var other in Engine.Node.GetNodesByT<PhysicsBody2D>())
                {
                    if (other.CollisionShape == CollisionShape || other.CollisionShape.Disabled)
                        continue;

                    Vector2 rightOffset = new Vector2(WALL_TOLERANCE, 0);
                    Vector2 leftOffset = new Vector2(-WALL_TOLERANCE, 0);

                    CollisionShape.LocalPosition += rightOffset;
                    if (CollisionShape.Intersects(other.CollisionShape))
                    {
                        _isOnWall = true;
                        WallNormal = new Vector2(-1, 0);
                    }
                    CollisionShape.LocalPosition -= rightOffset;

                    if (_isOnWall) break;

                    CollisionShape.LocalPosition += leftOffset;
                    if (CollisionShape.Intersects(other.CollisionShape))
                    {
                        _isOnWall = true;
                        WallNormal = new Vector2(1, 0);
                    }
                    CollisionShape.LocalPosition -= leftOffset;

                    if (_isOnWall) break;
                }
            }


            LocalPosition += new Vector2(0, movement.Y);

            foreach (var other in Engine.Node.GetNodesByT<PhysicsBody2D>())
            {
                if (other.CollisionShape == CollisionShape || other.CollisionShape.Disabled)
                    continue;

                if (CollisionShape.Intersects(other.CollisionShape))
                {
                    if (movement.Y > 0 || movement.Y >= -FLOOR_TOLERANCE)
                    {
                        _isOnFloor = true;
                        _floorShape = other.CollisionShape;
                        _lastFloorPosition = _floorShape.GlobalPosition;
                    }
                    else if (movement.Y < 0)
                    {
                        _isOnRoof = true;
                    }

                    LocalPosition -= new Vector2(0, movement.Y);
                    Velocity = new Vector2(Velocity.X, 0);
                    break;
                }
            }

            if (!_isOnFloor)
            {
                foreach (var other in Engine.Node.GetNodesByT<PhysicsBody2D>())
                {
                    if (other.CollisionShape == CollisionShape || other.CollisionShape.Disabled)
                        continue;

                    var verticalOffset = new Vector2(0, FLOOR_TOLERANCE);
                    CollisionShape.LocalPosition += verticalOffset;

                    if (CollisionShape.Intersects(other.CollisionShape))
                    {
                        _isOnFloor = true;
                        _floorShape = other.CollisionShape;
                        _lastFloorPosition = _floorShape.GlobalPosition;
                    }

                    CollisionShape.LocalPosition -= verticalOffset;
                    if (_isOnFloor)
                        break;
                }
            }
        }


        public override void PhysicsUpdate(float delta)
        {
            base.PhysicsUpdate(delta);
            Move(delta);
        }

        public void ApplyImpulse(Vector2 impulse)
        {
            Velocity += impulse;
        }
    }
}
