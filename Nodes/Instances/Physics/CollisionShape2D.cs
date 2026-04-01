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
            get => Shape?.Width ?? 0;
            set
            {
                if (Shape != null)
                    Shape.Width = value;
            }
        }

        public int Height
        {
            get => Shape?.Height ?? 0;
            set
            {
                if (Shape != null)
                    Shape.Height = value;
            }
        }

        public CollisionShape2D() {}

        public override void OnEnter()
        {
            base.OnEnter();

            if (Shape != null)
            {
                Shape.Location = GlobalTransform.Position.ToPoint();
            }

            _onTransformChanged = delegate(Transform2D newTransform)
            {
                if (Shape != null)
                {
                    Shape.Location = newTransform.Position.ToPoint();
                }
                if (GetParent() is StaticBody2D)
                    Console.WriteLine($"Collision: {Shape.BoundingBox}");
            };

            OnTransformChanged += _onTransformChanged;
        }

        public override void OnExit()
        {
            base.OnExit();

            if (_onTransformChanged != null)
                OnTransformChanged -= _onTransformChanged;
        }

        public override void PhysicsUpdate(float delta)
        {
            base.PhysicsUpdate(delta);
            CheckOneWay();
        }

       public AABB GetAABB()
        {
            if (Shape == null || Disabled)
                return new AABB(GlobalPosition, GlobalPosition);

            Vector2 worldMin = Shape.Location.ToVector2();
            Vector2 worldMax = worldMin + new Vector2(Shape.Width, Shape.Height);

            return new AABB(worldMin, worldMax);
        }

        private void CheckOneWay()
        {
            if (!OneWay || Shape == null)
                return;

            foreach (KinematicBody2D kb in Engine.Tree.GetAll<KinematicBody2D>())
            { 
                if (kb.CollisionShape?.Shape == null)
                    continue; 
                
                IRegionShape2D body = kb.CollisionShape.Shape;

                if (kb.Velocity.Y < 0)
                {
                    Disabled = true;
                }
                else if (!Shape.Intersects(body))
                {
                    Disabled = false;
                }
            }
        }

        public bool Contains(Point p)
        {
            if (!Disabled && Shape != null)
                return Shape.Contains(p);

            return false;
        }

        public bool Contains(CollisionShape2D other)
        {
            if (!Disabled && other?.Shape != null && Shape != null)
                return Shape.Contains(other.Shape);

            return false;
        }

        public bool Intersects(CollisionShape2D other)
        {
            if (Disabled || other?.Shape == null || Shape == null)
                return false;

            return Shape.Intersects(other.Shape);
        }

        public bool IntersectsAt(Vector2 offset, CollisionShape2D other)
        {
            if (Disabled || other?.Shape == null || other.Disabled || Shape == null)
                return false;

            return Shape.IntersectsAt(offset.ToPoint(), other.Shape);
        }

        public bool Contains(IRegionShape2D other)
        {
            if (!Disabled && Shape != null && other != null)
                return Shape.Contains(other);

            return false;
        }

        public bool Intersects(IRegionShape2D other)
        {
            if (!Disabled && Shape != null && other != null)
                return Shape.Intersects(other);

            return false;
        }

        public bool RayIntersect(Vector2 rayOrigin, Vector2 rayDir, float maxLength, out Vector2 hitPoint, out float distance)
        {
            hitPoint = Vector2.Zero;
            distance = float.MaxValue;

            if (!Disabled && Shape != null)
                return Shape.RayIntersect(rayOrigin, rayDir, maxLength, out hitPoint, out distance);

            return false;
        }

        public CollisionShape2D Clone()
        {
            IRegionShape2D clonedShape = Shape?.Clone();
            
            return new CollisionShape2D()
            {
                Disabled = this.Disabled,
                OneWay = this.OneWay,
                Shape = clonedShape
            };
        }
    }
}