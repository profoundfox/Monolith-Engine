using System;
using System.Linq;
using ConstructEngine;
using ConstructEngine.Nodes;

public class Area2D : RegionNode
{
    private bool wasInArea2D = false;

    private static readonly Type[] AcceptedAreaType = 
    {
        typeof(Area2D),
        typeof(KinematicBody2D),
    };

    public Area2D(NodeConfig config) : base(config) {}

    private Node GetOverlappingArea()
    {
        return AllInstances
            .Where(a => a != this && AcceptedAreaType.Any(t => t.IsAssignableFrom(a.GetType())))
            .Cast<Node>()
            .FirstOrDefault(a => Shape.Intersects(a.Shape));
    }

    private KinematicBody2D GetOverlappingBody()
    {
        return AllInstances
            .Where(a => a != this && typeof(KinematicBody2D).IsAssignableFrom(a.GetType()))
            .Cast<KinematicBody2D>()
            .FirstOrDefault(a => Shape.Intersects(a.Shape));
    }


        
    public bool AreaEntered(out Node overlapping)
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
        
    public bool AreaExited(out Node overlapping)
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


    public bool AreaInside(out Node overlapping)
    {
        overlapping = GetOverlappingArea();
        return overlapping != null;
    }

    public bool AreaInside()
    {
        return AreaInside(out _);
    }

    public bool BodyEntered(out KinematicBody2D overlapping)
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

    public bool BodyExited(out KinematicBody2D overlapping)
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

    public bool BodyInside(out KinematicBody2D overlapping)
    {
        overlapping = GetOverlappingBody();
        return overlapping != null;
    }

    public bool BodyInside()
    {
        return BodyInside(out _);
    }
}
