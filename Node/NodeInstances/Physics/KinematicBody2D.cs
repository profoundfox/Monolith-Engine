using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Monolith.Nodes
{
    public class KinematicBody2D : PhysicsBody2D
    {
        public Vector2 Velocity = Vector2.Zero;
        public Vector2 WallNormal { get; private set; } = Vector2.Zero;

        public bool IsOnFloor => _isOnFloor;
        public bool IsOnWall => _isOnWall;
        public bool IsOnRoof => _isOnRoof;

        private CollisionShape2D _floorShape;
        private Vector2 _lastFloorGlobalPosition;

        private bool _isOnWall = false;
        private bool _isOnFloor = false;
        private bool _isOnRoof = false;

        const float FLOOR_TOLERANCE = 2f;
        const float WALL_TOLERANCE = 2f;

        public KinematicBody2D() {}

        private void Move(float delta)
        {
            if (CollisionShape == null)
                return;

            Vector2 movement = Velocity * delta;

            if (_isOnFloor && _floorShape != null)
            {
                Vector2 platformDelta = _floorShape.GlobalTransform.Position - _lastFloorGlobalPosition;
                LocalPosition += platformDelta;
                _lastFloorGlobalPosition = _floorShape.GlobalTransform.Position;
            }

            _isOnFloor = false;
            _isOnRoof = false;
            _isOnWall = false;
            WallNormal = Vector2.Zero;
            _floorShape = null;

            var bodies = Engine.Node.GetAll<PhysicsBody2D>()
                            .Where(b => b.CollisionShape != CollisionShape && !b.CollisionShape.Disabled)
                            .ToArray();

            Vector2 horizontalMovement = new Vector2(movement.X, 0);
            LocalPosition += horizontalMovement;

            foreach (var other in bodies)
            {
                if (CollisionShape.Intersects(other.CollisionShape))
                {
                    _isOnWall = true;
                    WallNormal = movement.X > 0 ? new Vector2(-1, 0) : new Vector2(1, 0);
                    LocalPosition -= horizontalMovement;
                    Velocity = new Vector2(0, Velocity.Y);
                    break;
                }

                if (CollisionShape.IntersectsAt(new Vector2(WALL_TOLERANCE, 0), other.CollisionShape))
                {
                    _isOnWall = true;
                    WallNormal = new Vector2(-1, 0);
                }

                if (_isOnWall) break;

                if (CollisionShape.IntersectsAt(new Vector2(-WALL_TOLERANCE, 0), other.CollisionShape))
                {
                    _isOnWall = true;
                    WallNormal = new Vector2(1, 0);
                }

                if (_isOnWall) break;
            }

            Vector2 verticalMovement = new Vector2(0, movement.Y);
            LocalPosition += verticalMovement;

            foreach (var other in bodies)
            {
                if (CollisionShape.Intersects(other.CollisionShape))
                {
                    if (movement.Y > 0)
                    {
                        float penetration = (GlobalPosition.Y + CollisionShape.Height) - other.GlobalPosition.Y;
                        LocalPosition -= new Vector2(0, penetration);
                        _isOnFloor = true;
                        _floorShape = other.CollisionShape;
                        _lastFloorGlobalPosition = _floorShape.GlobalTransform.Position;
                    }
                    else if (movement.Y < 0)
                    {
                        float penetration = other.GlobalPosition.Y + other.CollisionShape.Height - GlobalPosition.Y;
                        LocalPosition += new Vector2(0, penetration);
                        _isOnRoof = true;
                    }

                    Velocity = new Vector2(Velocity.X, 0);
                    break;
                }

                if (CollisionShape.IntersectsAt(new Vector2(0, FLOOR_TOLERANCE), other.CollisionShape))
                {
                    _isOnFloor = true;
                    _floorShape = other.CollisionShape;
                    _lastFloorGlobalPosition = _floorShape.GlobalTransform.Position;
                }

                if (_isOnFloor) break;
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