using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Geometry;
using Monolith.Nodes;
using Monolith.Structs;

namespace Monlith.Nodes
{
    public record class CollisionShapeConfig : SpatialNodeConfig
    {
        public IRegionShape2D Shape { get; set; }
    }

    public class CollisionShape2D : Node2D
    {
        private Action<Transform2D> _onTransformChanged;

        public bool Disabled { get; set; }
        public bool OneWay { get; set; }
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
                LocalPosition = Shape.Location.ToVector2();
            }
        }

        public override void Load()
        {
            base.Load();

            if (Shape != null)
            {
                Shape.Location = GlobalTransform.Position.ToPoint();
            }

            _onTransformChanged = delegate(Transform2D newTransform)
            {
                Shape.Location = newTransform.Position.ToPoint();
            };

            TransformChanged += _onTransformChanged;
        }

        public override void Unload()
        {
            base.Unload();
            TransformChanged -= _onTransformChanged;
        }

        public bool Contains(Point p)
        {
            if (!Disabled)
            {
                return Shape.Contains(p);
            }
            return false;
        }

        public bool Contains(CollisionShape2D other)
        {
            if (!Disabled && other != null)
            {
                return Shape.Contains(other.Shape);
            }
            return false;
        }

        public bool Intersects(CollisionShape2D other)
        {
            if (!Disabled && other != null)
            {
                return Shape.Intersects(other.Shape);
            }
            return false;
        }

        public bool Contains(IRegionShape2D other)
        {
            if (!Disabled)
            {
                return Shape.Contains(other);
            }
            return false;
        }

        public bool Intersects(IRegionShape2D other)
        {
            if (!Disabled)
            {
                return Shape.Intersects(other);
            }
            return false;
        }

        public bool RayIntersect(Vector2 rayOrigin, Vector2 rayDir, float maxLength, out Vector2 hitPoint, out float distance)
        {
            hitPoint = Vector2.Zero;
            distance = float.MaxValue;

            if (!Disabled)
            {
                return Shape.RayIntersect(rayOrigin, rayDir, maxLength, out hitPoint, out distance);
            }

            return false;
        }

        public CollisionShape2D Clone()
        {
            IRegionShape2D clonedShape = null;
            if (Shape != null)
            {
                clonedShape = Shape.Clone();
            }

            var cfg = new CollisionShapeConfig
            {
                Shape = clonedShape,
                LocalPosition = this.LocalPosition,
                Name = Name,
                Parent = Parent
            };

            var clone = new CollisionShape2D(cfg)
            {
                Disabled = this.Disabled,
                OneWay = this.OneWay
            };

            return clone;
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
