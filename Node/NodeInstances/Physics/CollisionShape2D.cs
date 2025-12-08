using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Geometry;
using Monolith.Helpers;
using Monolith.Nodes;
using RenderingLibrary.Math.Geometry;

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

        public int Width
        {
            get => Shape.Width;
            set => Shape.Width = value;
        }

        public int Height
        {
            get => Shape.Height;
            set => Shape.Height = value;
        }

        public CollisionShape2D(CollisionShapeConfig cfg) : base(cfg)
        {
            Shape = cfg.Shape;

            if (Shape.Location != Point.Zero)
            {
                Position = new Vector2(Shape.Location.X, Shape.Location.Y);
            }

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

        public bool Contains(Point p)
        {
            if (!Disabled)
                return Shape.Contains(p);
            return false;
        }

        public bool Contains(CollisionShape2D other)
        {
            if (!Disabled)
                return Shape.Contains(other.Shape);
            return false;
        }

        public bool Intersects(CollisionShape2D other)
        {
            if (!Disabled)
                return Shape.Intersects(other.Shape);
            return false;
        }

        public bool Contains(IRegionShape2D other)
        {
            if (!Disabled)
                return Shape.Contains(other);
            return false;
        }

        public bool Intersects(IRegionShape2D other)
        {
            if (!Disabled)
                return Shape.Intersects(other);
            return false;
        }


        public bool RayIntersect(Vector2 rayOrigin, Vector2 rayDir, float maxLength, out Vector2 hitPoint, out float distance)
        {
            hitPoint = Vector2.Zero;
            distance = float.MaxValue;

            if (!Disabled)
                return Shape.RayIntersect(rayOrigin, rayDir, maxLength, out hitPoint, out distance);
            
            return false;
        }

        public CollisionShape2D Clone()
        {
            var clonedShape = Shape.Clone();

            var cfg = new CollisionShapeConfig
            {
                Shape = clonedShape,
                Position = this.Position,
                Name = Name,
                Parent = Parent
            };

            var clone = new CollisionShape2D(cfg)
            {
                Disabled = this.Disabled
            };

            return clone;
        }


    }
}