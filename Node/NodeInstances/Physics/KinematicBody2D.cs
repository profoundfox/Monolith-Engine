using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Monolith.Nodes
{
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

        public KinematicBody2D() {}

        private void Move(float delta)
        {
            CollisionShape = (CollisionShape2D)GetFirstChildByT<CollisionShape2D>();
            if (CollisionShape == null)
                return;

            movement = Velocity * delta;

            if (_isOnFloor && _floorShape != null)
            {
                Vector2 platformDelta = _floorShape.Position - _lastFloorPosition;
                Position += platformDelta;
                _lastFloorPosition = _floorShape.Position;
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
            Position += horizontalMovement;

            foreach (var other in bodies)
            {
                if (CollisionShape.Intersects(other.CollisionShape))
                {
                    _isOnWall = true;
                    WallNormal = movement.X > 0 ? new Vector2(-1, 0) : new Vector2(1, 0);
                    Position -= horizontalMovement;
                    Velocity = new Vector2(0, Velocity.Y);
                    break;
                }

                CollisionShape.Position += new Vector2(WALL_TOLERANCE, 0);
                if (CollisionShape.Intersects(other.CollisionShape))
                {
                    _isOnWall = true;
                    WallNormal = new Vector2(-1, 0);
                }
                CollisionShape.Position -= new Vector2(WALL_TOLERANCE, 0);

                if (_isOnWall) break;

                CollisionShape.Position += new Vector2(-WALL_TOLERANCE, 0);
                if (CollisionShape.Intersects(other.CollisionShape))
                {
                    _isOnWall = true;
                    WallNormal = new Vector2(1, 0);
                }
                CollisionShape.Position -= new Vector2(-WALL_TOLERANCE, 0);

                if (_isOnWall) break;
            }

            Vector2 verticalMovement = new Vector2(0, movement.Y);
            Position += verticalMovement;

            foreach (var other in bodies)
            {
                if (CollisionShape.Intersects(other.CollisionShape))
                {
                    if (movement.Y > 0 || movement.Y >= -FLOOR_TOLERANCE)
                    {
                        _isOnFloor = true;
                        _floorShape = other.CollisionShape;
                        _lastFloorPosition = _floorShape.Position;
                    }
                    else if (movement.Y < 0)
                    {
                        _isOnRoof = true;
                    }

                    Position -= verticalMovement;
                    Velocity = new Vector2(Velocity.X, 0);
                    break;
                }

                CollisionShape.Position += new Vector2(0, FLOOR_TOLERANCE);
                if (CollisionShape.Intersects(other.CollisionShape))
                {
                    _isOnFloor = true;
                    _floorShape = other.CollisionShape;
                    _lastFloorPosition = _floorShape.Position;
                }
                CollisionShape.Position -= new Vector2(0, FLOOR_TOLERANCE);

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
