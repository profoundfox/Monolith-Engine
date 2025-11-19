using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ConstructEngine.Helpers;
using System;
using System.Linq;

namespace ConstructEngine.Area
{
    public class Area
    {
        private static List<Area> allInstances = new List<Area>();
        private bool wasInArea = false;
        
        public RegionShape2D RegionShape {get; set;}
        public static IReadOnlyList<Area> AllInstances => allInstances.AsReadOnly();

        public Area(RegionShape2D shape)
        {
            allInstances.Add(this);
            RegionShape = shape;
        }
        public bool AreaEntered()
        {
            bool isInArea = AllInstances
                .Where(a => a != this)
                .Any(a => RegionShape.Shape.Intersects(a.RegionShape.Shape)
            );            
            wasInArea = isInArea;
            return isInArea;
        }

        public bool AreaExited()
        {
            foreach (Area a in AllInstances)
            {
                if (a == this) continue; 

                bool isInArea = AllInstances
                    .Where(a => a != this)
                    .Any(a => RegionShape.Shape.Intersects(a.RegionShape.Shape)
                );

                bool exited = wasInArea && !isInArea;
                wasInArea = isInArea;
                return exited;
            }
            return false;
        }

    }
}