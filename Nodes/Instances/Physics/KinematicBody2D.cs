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
        private CollisionShape2D _wallShape;
        
        private Vector2 _lastWallGlobalPosition;
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

            // Debug: Log initial position and velocity
            Console.WriteLine($"Before Move: Position: {LocalPosition}, Velocity: {Velocity}, Movement: {movement}");

            if (_isOnFloor && _floorShape != null)
            {
                Vector2 platformDelta = _floorShape.GlobalTransform.Position - _lastFloorGlobalPosition;
                LocalPosition += platformDelta;
                _lastFloorGlobalPosition = _floorShape.GlobalTransform.Position;
            }

            if (_isOnWall && _wallShape != null)
            {
                Vector2 wallDelta = _wallShape.GlobalTransform.Position - _lastWallGlobalPosition;
                LocalPosition += wallDelta;
                _lastWallGlobalPosition = _wallShape.GlobalTransform.Position;
            }

            ResolveStaticPenetration();

            _isOnFloor = false;
            _isOnRoof = false;
            _isOnWall = false;
            WallNormal = Vector2.Zero;
            _floorShape = null;
            _wallShape = null;

            Vector2 horizontalMovement = new Vector2(movement.X, 0);
            LocalPosition += horizontalMovement;

            var nearby = Engine.Physics.Query(Bounds);

            foreach (var other in nearby.Where(b => b != this))
            {
                if (CollisionShape.Intersects(other.CollisionShape))
                {
                    _isOnWall = true;
                    _wallShape = other.CollisionShape;
                    _lastWallGlobalPosition = _wallShape.GlobalTransform.Position;

                    WallNormal = movement.X > 0 ? new Vector2(-1, 0) : new Vector2(1, 0);

                    LocalPosition -= horizontalMovement;
                    Velocity = new Vector2(0, Velocity.Y);
                    break;
                }

                bool nearWall = false;
                Vector2 nearWallNormal = Vector2.Zero;

                if (CollisionShape.IntersectsAt(new Vector2(WALL_TOLERANCE, 0), other.CollisionShape))
                {
                    nearWall = true;
                    nearWallNormal = new Vector2(-1, 0);
                }
                else if (CollisionShape.IntersectsAt(new Vector2(-WALL_TOLERANCE, 0), other.CollisionShape))
                {
                    nearWall = true;
                    nearWallNormal = new Vector2(1, 0);
                }

                if (nearWall && movement.X != 0)
                {
                    _isOnWall = true;
                    WallNormal = nearWallNormal;
                    break;
                }
            }

            Vector2 verticalMovement = new Vector2(0, movement.Y);
            LocalPosition += verticalMovement;

            nearby = Engine.Physics.Query(Bounds);

            foreach (var other in nearby.Where(b => b != this))
            {
                if (CollisionShape.Intersects(other.CollisionShape))
                {
                    if (movement.Y > 0)
                    {
                        float penetration = (GlobalPosition.Y + CollisionShape.Height) - other.GlobalPosition.Y;
                        // Debug: Log penetration for upward movement
                        Console.WriteLine($"Penetration (Upward): {penetration}, Position: {LocalPosition}, Velocity: {Velocity}");

                        LocalPosition -= new Vector2(0, penetration);
                        _isOnFloor = true;
                        _floorShape = other.CollisionShape;
                        _lastFloorGlobalPosition = _floorShape.GlobalTransform.Position;
                    }
                    else if (movement.Y < 0)
                    {
                        float penetration = other.GlobalPosition.Y + other.CollisionShape.Height - GlobalPosition.Y;
                        // Debug: Log penetration for downward movement
                        Console.WriteLine($"Penetration (Downward): {penetration}, Position: {LocalPosition}, Velocity: {Velocity}");

                        LocalPosition += new Vector2(0, penetration);
                        _isOnRoof = true;
                    }

                    Velocity = new Vector2(Velocity.X, 0);
                    break;
                }

                if (movement.Y >= 0 &&
                    CollisionShape.IntersectsAt(new Vector2(0, FLOOR_TOLERANCE), other.CollisionShape))
                {
                    // Debug: Log when floor tolerance is hit
                    Console.WriteLine($"Floor Tolerance Hit: Position: {LocalPosition}, Movement: {movement.Y}");

                    _isOnFloor = true;
                    _floorShape = other.CollisionShape;
                    _lastFloorGlobalPosition = _floorShape.GlobalTransform.Position;
                }

                if (_isOnFloor) break;
            }

            // Debug: Log final position after move and collision resolution
            Console.WriteLine($"After Move: Position: {LocalPosition}, Velocity: {Velocity}");
        }

        private void ResolveStaticPenetration()
        {
            var nearby = Engine.Physics.Query(Bounds);

            foreach (var other in nearby.Where(b => b != this))
            {
                if (CollisionShape.Intersects(other.CollisionShape))
                {
                    Rectangle a = this.Bounds;
                    Rectangle b = other.Bounds;

                    float moveRight = b.Right - a.Left;
                    float moveLeft = a.Right - b.Left;
                    float moveDown = b.Bottom - a.Top;
                    float moveUp = a.Bottom - b.Top;

                    float minX = Math.Min(moveRight, moveLeft);
                    float minY = Math.Min(moveDown, moveUp);

                    // Debug: Log resolution of static penetration
                    Console.WriteLine($"Resolving Static Penetration: MinX: {minX}, MinY: {minY}");

                    if (minX < minY)
                    {
                        LocalPosition += new Vector2(
                            moveRight < moveLeft ? moveRight : -moveLeft,
                            0);
                    }
                    else
                    {
                        LocalPosition += new Vector2(
                            0,
                            moveDown < moveUp ? moveDown : -moveUp);
                    }
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