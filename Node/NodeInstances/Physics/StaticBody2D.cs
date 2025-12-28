using System;
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
            CollisionShape2D.Disabled = !Collidable;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            CollisionShape2D.Disabled = !Collidable;

            foreach (KinematicBody2D kb in Engine.Node.GetNodesByT<KinematicBody2D>())
            {
                if (CollisionShape2D == null)
                    continue;

                IRegionShape2D platform = CollisionShape2D.Shape;
                IRegionShape2D body = kb.CollisionShape2D.Shape;

                if (!OneWay)
                    continue;

                if (kb.Velocity.Y < 0)
                {
                    Collidable = false;
                }

                else if(!platform.Intersects(body))
                {
                    Collidable = true;
                }
            }
        }

    }

}