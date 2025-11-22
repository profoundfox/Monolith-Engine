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
        public Area2D(NodeConfig config) : base(config) {}

        public bool AreaEntered(out Area2D overlapping)
        {
            overlapping = AllInstances.OfType<Area2D>()
                .FirstOrDefault(a => a != this && Shape.Intersects(a.Shape));

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
            overlapping = AllInstances.OfType<Area2D>()
                .FirstOrDefault(a => a != this && Shape.Intersects(a.Shape));

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