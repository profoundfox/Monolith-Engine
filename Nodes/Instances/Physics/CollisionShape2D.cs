using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Geometry;
using Monolith.Attributes;

namespace Monolith.Nodes
{
    public class CollisionShape2D : Node2D
    {
        public bool Disabled { get; set; }
        public bool OneWay { get; set; }
        public IShape2D Shape { get; set; }

        public int Width
        {
            get => Shape?.Size.Width ?? 0;
            set
            {
                if (Shape != null)
                    Shape.Size = new Extent(value, Height);
            }
        }

        public int Height
        {
            get => Shape?.Size.Height ?? 0;
            set
            {
                if (Shape != null)
                    Shape.Size = new Extent(Width, value);
            }
        }

        public CollisionShape2D() {}

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void PhysicsUpdate(float delta)
        {
            base.PhysicsUpdate(delta);
            //CheckOneWay();
        }


        private void CheckOneWay()
        {
            if (!OneWay || Shape == null)
                return;

            foreach (KinematicBody2D kb in Engine.Tree.GetAll<KinematicBody2D>())
            { 
                if (kb.CollisionShape?.Shape == null)
                    continue; 
                
                IShape2D body = kb.CollisionShape.Shape;

                if (kb.Velocity.Y < 0)
                {
                    Disabled = true;
                }
                else if (!Shape.Intersect(body, GlobalPosition.ToPoint(), kb.GlobalPosition.ToPoint()))
                {
                    Disabled = false;
                }
            }
        }

        public bool Intersects(CollisionShape2D other)
        {
            if (Disabled || other?.Shape == null || Shape == null)
                return false;

            return Shape.Intersect(other.Shape, GlobalPosition.ToPoint(), other.GlobalPosition.ToPoint());
        }

        public bool IntersectsAt(Vector2 offset, CollisionShape2D other)
        {
            if (Disabled || other?.Shape == null || other.Disabled || Shape == null)
                return false;

            return Shape.IntersectsAt(offset.ToPoint(), other.Shape, GlobalPosition.ToPoint(), other.GlobalPosition.ToPoint());
        }

        public bool Contains(Vector2 position)
        {
            if (!Disabled && Shape != null)
                return Shape.Contains(position.ToPoint(), GlobalPosition.ToPoint());

            return false;
        }


        public CollisionShape2D Clone()
        {
            IShape2D clonedShape = Shape?.Clone();
            
            return new CollisionShape2D()
            {
                Disabled = this.Disabled,
                OneWay = this.OneWay,
                Shape = clonedShape
            };
        }
    }
}