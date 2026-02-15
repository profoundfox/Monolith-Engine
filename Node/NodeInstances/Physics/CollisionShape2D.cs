using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Geometry;
using Monolith.Attributes;

namespace Monolith.Nodes
{
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

        public CollisionShape2D() {}

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

        public override void PhysicsUpdate(float delta)
        {
            base.PhysicsUpdate(delta);

            CheckOneWay();
        }

        public AABB GetAABB()
        {
            if (Shape == null || Disabled)
                return new AABB(Position, Position);

            Vector2 localMin = Shape.Location.ToVector2();
            Vector2 localMax = localMin + new Vector2(Shape.Width, Shape.Height);

            Vector2 worldMin = Position + localMin;
            Vector2 worldMax = Position + localMax;

            return new AABB(worldMin, worldMax);
        }

        private void CheckOneWay()
        {
            foreach (KinematicBody2D kb in Engine.Node.GetAll<KinematicBody2D>())
            {
                IRegionShape2D body = kb.CollisionShape.Shape;

                if (!OneWay)
                    continue;

                if (kb.Velocity.Y < 0)
                {
                    Disabled = true;
                }

                else if(!Shape.Intersects(body))
                {
                    Disabled = false;
                }
            }
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
            if (Disabled || other == null)
                return false;

            return Shape.Intersects(other.Shape);
        }

        public bool IntersectsAt(Vector2 offset, CollisionShape2D other)
        {
            if (Disabled || other == null || other.Disabled)
                return false;

            return Shape.IntersectsAt(offset.ToPoint(), other.Shape);
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
            
            var clone = new CollisionShape2D()
            {
                Disabled = this.Disabled,
                OneWay = this.OneWay
            };

            return clone;
        }
    }
}
