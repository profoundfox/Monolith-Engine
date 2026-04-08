
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Monolith.Geometry;
using Monolith.Tools;

namespace Monolith.Nodes
{
   
    public class CollisionNode2D : Node2D
    {
        public CollisionShape2D CollisionShape { get => Get<CollisionShape2D>(); }

        public int MaxLayer { get; set; } = int.MaxValue;

        public List<int> Layers { get; private set; }

        public Rectangle Bounds 
        {
            get
            {
                if (CollisionShape == null)
                    return Rectangle.Empty;

                return CollisionShape.Shape.GetAABB(GlobalPosition.ToPoint());
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