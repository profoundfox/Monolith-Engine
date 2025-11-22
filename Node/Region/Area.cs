using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ConstructEngine.Helpers;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using ConstructEngine.Components;
using ConstructEngine.Region;

namespace ConstructEngine.Nodes
{
    public class Area2D : RegionNode
    {
        private bool wasInArea2D = false;

        private static readonly Type[] AcceptedTypes = 
        {
            typeof(Area2D),
            typeof(KinematicBody2D),
        };

        public Area2D(NodeConfig config) : base(config) {}

        private Area2D GetOverlapping()
        {
            return AllInstances
                .Where(a => a != this && AcceptedTypes.Contains(a.GetType()))
                .Cast<Area2D>()
                .FirstOrDefault(a => Shape.Intersects(a.Shape));
        }
        
        public bool AreaEntered(out Area2D overlapping)
        {
            overlapping = GetOverlapping();

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
            overlapping = GetOverlapping();

            bool isInArea2D = overlapping != null;
            bool exited = wasInArea2D && !isInArea2D;

            wasInArea2D = isInArea2D;
            return exited;
        }

        public bool AreaExited()
        {
            return AreaExited(out _);
        }
    }
}