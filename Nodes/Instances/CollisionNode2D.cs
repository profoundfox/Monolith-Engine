
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using Microsoft.Xna.Framework;
using Monolith.Geometry;
using Monolith.Tools;

namespace Monolith.Nodes
{
   
    public class CollisionNode2D : Node2D
    {
        public List<CollisionShape2D> CollisionShapes { get => GetAll<CollisionShape2D>().ToList(); }

        public int MaxLayer { get; set; } = int.MaxValue;

        public List<int> Layers { get; private set; }

        public List<Rectangle> Bounds 
        {
            get
            {
                if (CollisionShapes.Count == 0)
                    return [Rectangle.Empty];

                var b = new List<Rectangle>();

                foreach (var c in CollisionShapes)
                    b.Add(c.Shape.GetAABB(GlobalPosition.ToPoint()));
                
                return b;
            }
        }

        public CollisionNode2D() {}

        public int AddLayer(int layer)
        {
            var finVal = Math.Clamp(layer, 0, MaxLayer);
        
            Layers.Add(finVal);

            return finVal;
        }

        public int[] AddLayers(params int[] layers)
        {
            var finVals = layers.ClampArray(0, MaxLayer);

            Layers.AddRange(finVals);

            return finVals;
        }

        public int RemoveLayer(int layer)
        {
            var finVal = Math.Clamp(layer, 0, MaxLayer);

            Layers.Remove(finVal);

            return finVal;
        }

        public int[] RemoveLayers(params int[] layers)
        {
            var finVals = layers.ClampArray(0, MaxLayer);

            Layers.RemoveAll(item => layers.Contains(item));

            return finVals;
        }

        private bool IsValid(CollisionShape2D shape)
        {
            return shape.Disabled == false && shape?.Shape != null; 
        }

        public bool Intersects(CollisionNode2D other)
        {
            return this.CollisionShapes.Any(
                myShape => other.CollisionShapes.Any(
                    otherShape => myShape.Intersects(otherShape)
                     && IsValid(myShape) && IsValid(otherShape)
                ));
        }

        public bool IntersectsAt(Vector2 offset, CollisionNode2D other)
        {
            return this.CollisionShapes.Any(
                myShape => other.CollisionShapes.Any(
                    otherShape => myShape.IntersectsAt(offset, otherShape)
                     && IsValid(myShape) && IsValid(otherShape)
                ));
        }

        public bool Contains(Vector2 position)
        {
            return this.CollisionShapes.Any(
                c => c.Shape.Contains(position.ToPoint(), GlobalPosition.ToPoint()) && IsValid(c));
        }

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
        }

        public override void ProcessUpdate(float delta)
        {
            base.ProcessUpdate(delta);
        }

        public override void SubmitCall()
        {
            base.SubmitCall();
        }
    }
}