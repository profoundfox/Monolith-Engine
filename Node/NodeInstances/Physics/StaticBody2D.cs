using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Monolith.Geometry;
using Monolith.Managers;

namespace Monolith.Nodes
{
    public record class StaticBodyConfig : SpatialNodeConfig
    {
        public bool Collidable { get; set; } = true;
        public bool OneWay { get; set; }
        public CollisionShape2D CollisionShape2D { get; set; }
    }
    public class StaticBody2D : Node2D
    {
        public bool Collidable { get; set; }
        public bool OneWay { get; set; }
        public CollisionShape2D CollisionShape2D { get; set; }

        public StaticBody2D(StaticBodyConfig cfg) : base(cfg)
        {
            Collidable = cfg.Collidable;
            OneWay = cfg.OneWay;
            CollisionShape2D = cfg.CollisionShape2D;

            CollisionShape2D.OneWay = OneWay;
            CollisionShape2D.Disabled = !Collidable;
        }

        public override void ProcessUpdate(GameTime gameTime)
        {
            base.ProcessUpdate(gameTime);
        }

    }

}