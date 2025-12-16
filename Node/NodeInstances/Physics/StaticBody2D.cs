using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Monlith.Nodes;
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
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var kinBodies = NodeManager.GetNodesByType<KinematicBody2D>();
        }
    }

}