using System;
using System.Collections.Generic;
using System.Linq;
using Monolith.Params;

namespace Monolith.Hierarchy
{
  public class Area2D : CollisionNode2D
  {
    [Export]
    public bool MonitorAreas { get; set; } = true;

    [Export]
    public bool MonitorBodies { get; set; } = true;

    private HashSet<Area2D> _previousAreas = new();
    private HashSet<Area2D> _currentAreas = new();

    private HashSet<PhysicsBody2D> _previousBodies = new();
    private HashSet<PhysicsBody2D> _currentBodies = new();

    private IEnumerable<Area2D> GetOverlappingAreas()
    {
      return Engine.Index.GetAll()
        .Where(a => a != this && a is Area2D area && Intersects(area))
        .Cast<Area2D>();
    }

    private IEnumerable<PhysicsBody2D> GetOverlappingBodies()
    {
      return Engine.Index.GetAll()
        .Where(a => a != this && a is PhysicsBody2D body && Intersects(body))
        .Cast<PhysicsBody2D>();
    }

    private HashSet<Area2D> SnapshotAreas()
    {
      return GetOverlappingAreas().ToHashSet();
    }

    private HashSet<PhysicsBody2D> SnapshotBodies()
    {
      return GetOverlappingBodies().ToHashSet();
    }

    public override void _Process(float delta)
    {
      base._Process(delta);

      if (MonitorAreas)
      {
        _previousAreas = _currentAreas;
        _currentAreas = SnapshotAreas();
      }

      if (MonitorBodies)
      {
        _previousBodies = _currentBodies;
        _currentBodies = SnapshotBodies();
      }
    }

    public IEnumerable<Area2D> AreasEntered() => _currentAreas.Except(_previousAreas);

    public IEnumerable<Area2D> AreasExited() => _previousAreas.Except(_currentAreas);

    public IEnumerable<Area2D> AreasInside() => _currentAreas;


    public bool IsInsideAnyArea() => _currentAreas.Count > 0;


    public IEnumerable<PhysicsBody2D> BodiesEntered() => _currentBodies.Except(_previousBodies);

    public IEnumerable<PhysicsBody2D> BodiesExited() => _previousBodies.Except(_currentBodies);

    public IEnumerable<PhysicsBody2D> BodiesInside() => _currentBodies;


    public bool IsInsideAnyBody() => _currentBodies.Count > 0;

    public Area2D GetAnyArea()
    {
      return _currentAreas.FirstOrDefault();
    }

    public PhysicsBody2D GetAnyBody()
    {
      return _currentBodies.FirstOrDefault();
    }
  }
}
