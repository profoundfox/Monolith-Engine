using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Geometry;
using Monolith.Helpers;
using Monolith.Nodes;

namespace Monlith.Nodes
{
    public record class CollisionShapeConfig : SpatialNodeConfig
    {
        public IRegionShape2D Shape { get; set; }
    }

    public class CollisionShape2D : Node2D
    {
        public bool Disabled { get; set; }
        public IRegionShape2D Shape { get; set; }

        public CollisionShape2D(CollisionShapeConfig cfg) : base(cfg)
        {
            Shape = cfg.Shape;

            if (Shape.Location != Point.Zero)
                Position = new Vector2(Shape.Location.X, Shape.Location.Y);

            Shape.Location = Position.ToPoint();

            PositionChanged += (newPos) =>
            {
                Shape.Location = newPos.ToPoint();
            };
        }

        public override void Load()
        {
            base.Load();
        }

        public override void Unload()
        {
            base.Unload();
            
            Shape = null;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}