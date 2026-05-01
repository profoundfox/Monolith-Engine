using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Monolith.Params;

namespace Monolith.Hierarchy
{
  public class KinematicBody2D : PhysicsBody2D
  {
    [Export]
    public Vector2 Velocity = Vector2.Zero;

    [Export]
    public Vector2 WallNormal { get; private set; } = Vector2.Zero;
    
    [Export]
    public bool IsOnFloor => _isOnFloor;

    [Export]
    public bool IsOnWall => _isOnWall;

    [Export]
    public bool IsOnRoof => _isOnRoof;

    private CollisionNode2D _floorBody;
    private CollisionNode2D _wallBody;

    private Vector2 _lastWallGlobalPosition;
    private Vector2 _lastFloorGlobalPosition;

    private bool _isOnWall = false;
    private bool _isOnFloor = false;
    private bool _isOnRoof = false;

    const float FLOOR_TOLERANCE = 2f;
    const float WALL_TOLERANCE = 2f;

    public KinematicBody2D() { }

    private void Move(float delta)
    {
      if (CollisionShapes.Count == 0)
        return;

      Vector2 movement = Velocity * delta;

      if (_isOnFloor && _floorBody != null)
      {
        Vector2 platformDelta = _floorBody.Transform.Global.Position - _lastFloorGlobalPosition;
        LocalPosition += platformDelta;
        _lastFloorGlobalPosition = _floorBody.Transform.Global.Position;
      }

      if (_isOnWall && _wallBody != null)
      {
        Vector2 wallDelta = _wallBody.Transform.Global.Position - _lastWallGlobalPosition;
        LocalPosition += wallDelta;
        _lastWallGlobalPosition = _wallBody.Transform.Global.Position;
      }

      ResolveStaticPenetration();

      _isOnFloor = false;
      _isOnRoof = false;
      _isOnWall = false;
      WallNormal = Vector2.Zero;
      _floorBody = null;
      _wallBody = null;

      Vector2 horizontalMovement = new Vector2(movement.X, 0);
      LocalPosition += horizontalMovement;

      var nearby = Engine.Physics.Query(Bounds);

      foreach (var other in nearby.Where(b => b != this))
      {
        if (this.Intersects(other))
        {
          _isOnWall = true;
          _wallBody = other;
          _lastWallGlobalPosition = other.Transform.Global.Position;

          WallNormal = movement.X > 0 ? new Vector2(-1, 0) : new Vector2(1, 0);

          LocalPosition -= horizontalMovement;
          Velocity = new Vector2(0, Velocity.Y);
          break;
        }

        bool nearWall = false;
        Vector2 nearWallNormal = Vector2.Zero;

        if (this.IntersectsAt(new Vector2(WALL_TOLERANCE, 0), other))
        {
          nearWall = true;
          nearWallNormal = new Vector2(-1, 0);
        }
        else if (this.IntersectsAt(new Vector2(-WALL_TOLERANCE, 0), other))
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
        if (this.Intersects(other))
        {
          if (movement.Y > 0)
          {
            ResolveVerticalPenetration(other, true);

            _isOnFloor = true;
            _floorBody = other;
            _lastFloorGlobalPosition = other.Transform.Global.Position;
          }
          else if (movement.Y < 0)
          {
            ResolveVerticalPenetration(other, false);
            _isOnRoof = true;
          }

          Velocity = new Vector2(Velocity.X, 0);
          break;
        }

        if (movement.Y >= 0 && this.IntersectsAt(new Vector2(0, FLOOR_TOLERANCE), other))
        {
          _isOnFloor = true;
          _floorBody = other;
          _lastFloorGlobalPosition = other.Transform.Global.Position;
          break;
        }
      }
    }

    private void ResolveVerticalPenetration(CollisionNode2D other, bool fromTop)
    {
      foreach (var a in this.Bounds)
        foreach (var b in other.Bounds)
        {
          if (!a.Intersects(b)) continue;

          if (fromTop)
          {
            float penetration = (a.Bottom - b.Top);
            LocalPosition -= new Vector2(0, penetration);
          }
          else
          {
            float penetration = (b.Bottom - a.Top);
            LocalPosition += new Vector2(0, penetration);
          }
        }
    }

    private void ResolveStaticPenetration()
    {
      var nearby = Engine.Physics.Query(Bounds);

      foreach (var other in nearby.Where(b => b != this))
      {
        if (!this.Intersects(other)) continue;

        foreach (var a in this.Bounds)
          foreach (var b in other.Bounds)
          {
            if (!a.Intersects(b)) continue;

            float moveRight = b.Right - a.Left;
            float moveLeft = a.Right - b.Left;
            float moveDown = b.Bottom - a.Top;
            float moveUp = a.Bottom - b.Top;

            float minX = Math.Min(moveRight, moveLeft);
            float minY = Math.Min(moveDown, moveUp);

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

    public override void _PhysicsUpdate(float delta)
    {
      base._PhysicsUpdate(delta);
      Move(delta);
    }

    public void ApplyImpulse(Vector2 impulse)
    {
      Velocity += impulse;
    }
  }
}
