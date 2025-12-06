using System;
using System.Collections.Generic;
using System.Linq;
using Monlith.Nodes;
using Monolith;
using Monolith.Managers;
using Monolith.Nodes;

namespace Monolith.Nodes
{
    public record class AreaConfig : SpatialNodeConfig
    {
        public bool MonitorAreas { get; set; }
        public bool MonitorBodies { get; set; }
        public CollisionShape2D CollisionShape2D { get; set; }
    }

    public class Area2D : Node2D
    {
        private bool wasInArea2D = false;

        private static readonly Type[] AcceptedType = 
        {
            typeof(Area2D),
            typeof(KinematicBody2D),
        };

        public bool MonitorAreas { get; set; }
        public bool MonitorBodies { get; set; }
        public CollisionShape2D CollisionShape2D { get; set; }

        public Area2D(AreaConfig cfg) : base(cfg)
        {
            CollisionShape2D = cfg.CollisionShape2D;
            MonitorAreas = cfg.MonitorAreas;
            MonitorBodies = cfg.MonitorBodies;
        }

        private Node2D GetOverlappingArea()
        {
            return NodeManager.GetNodesByType<CollisionShape2D>()
                .Where(c => c.Parent != this)
                .Where(c => AcceptedType.Any(t => t.IsAssignableFrom(c.Parent.GetType())))
                .FirstOrDefault(c => c.Shape.Intersects(CollisionShape2D.Shape))
                ?.Parent as Node2D;
        }


        private KinematicBody2D GetOverlappingBody()
        {
            return NodeManager.AllInstances
                .Where(a => a != this && typeof(KinematicBody2D).IsAssignableFrom(a.GetType()))
                .Cast<KinematicBody2D>()
                .FirstOrDefault(a => CollisionShape2D.Shape.Intersects(a.CollisionShape2D.Shape));
        }


            
        public bool AreaEntered(out Node2D overlapping)
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
            
        public bool AreaExited(out Node2D overlapping)
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


        public bool AreaInside(out Node2D overlapping)
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
}