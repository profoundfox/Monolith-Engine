using System;
using System.Collections.Generic;
using System.Linq;
using Monolith;
using Monolith.Managers;
using Monolith.Hierarchy;

namespace Monolith.Hierarchy
{
  public class Area2D : CollisionNode2D
  {
    private bool wasInArea2D = false;


    public bool MonitorAreas { get; set; }
    public bool MonitorBodies { get; set; }

    public Area2D() { }

    private Area2D GetOverlappingArea()
    {
      return Engine.Index.GetAll<CollisionNode2D>()
          .Where(c => c.GetParent() != this)
          .FirstOrDefault(c => c.Intersects(this))
          ?.GetParent<Area2D>();
    }


    private PhysicsBody2D GetOverlappingBody()
    {
      return Engine.Index.GetAll()
          .Where(a => a != this && typeof(PhysicsBody2D).IsAssignableFrom(a.GetType()))
          .Cast<PhysicsBody2D>()
          .FirstOrDefault(Intersects);
    }


    public bool AreaEntered(out Area2D overlapping)
    {
      overlapping = GetOverlappingArea();

      bool isInArea2D = overlapping != null;
      bool entered = !wasInArea2D && isInArea2D;

      wasInArea2D = isInArea2D;
      return entered;
    }

    public bool AreaEntered()
    {
      return AreaEntered(out _);
    }

    public bool AreaExited(out Area2D overlapping)
    {
      overlapping = GetOverlappingArea();

      bool isInArea2D = overlapping != null;
      bool exited = wasInArea2D && !isInArea2D;

      wasInArea2D = isInArea2D;
      return exited;
    }

    public bool AreaExited()
    {
      return AreaExited(out _);
    }


    public bool AreaInside(out Area2D overlapping)
    {
      overlapping = GetOverlappingArea();
      return overlapping != null;
    }

    public bool AreaInside()
    {
      return AreaInside(out _);
    }

    public bool BodyEntered(out PhysicsBody2D overlapping)
    {
      overlapping = GetOverlappingBody();

      bool isInBody2D = overlapping != null;
      bool entered = !wasInArea2D && isInBody2D;

      wasInArea2D = isInBody2D;
      return entered;
    }

    public bool BodyEntered()
    {
      return BodyEntered(out _);
    }

    public bool BodyExited(out PhysicsBody2D overlapping)
    {
      overlapping = GetOverlappingBody();

      bool isInBody2D = overlapping != null;
      bool exited = wasInArea2D && !isInBody2D;

      wasInArea2D = isInBody2D;
      return exited;
    }

    public bool BodyExited()
    {
      return BodyExited(out _);
    }

    public bool BodyInside(out PhysicsBody2D overlapping)
    {
      overlapping = GetOverlappingBody();
      return overlapping != null;
    }

    public bool BodyInside()
    {
      return BodyInside(out _);
    }
  }
}