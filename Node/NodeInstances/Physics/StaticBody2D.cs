using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monolith.Geometry;


namespace Monolith.Nodes
{
    public record class StaticBodyConfig : SpatialNodeConfig
    {
        public bool Collidable { get; set; } = true;
        public bool OneWay { get; set; }
    }
    public class StaticBody2D : Node2D, ICollidable2D
    {
        public bool Collidable { get; set; }
        public bool OneWay { get; set; }

        public StaticBody2D(StaticBodyConfig cfg) : base(cfg)
        {
            Collidable = cfg.Collidable;
            OneWay = cfg.OneWay;
        }
    }

}