using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Monolith.Graphics;
using Monolith.Nodes;

namespace Monolith.Nodes
{
    public class Parallax2D : Node2D
    {
        private Vector2 lastCameraPos;

        public Parallax2D() {}

        public override void ProcessUpdate(float delta)
        {
            base.ProcessUpdate(delta);

            var camera = Camera2D.CurrentCameraInstance;
            Vector2 camDelta = camera.Position - lastCameraPos;
            lastCameraPos = camera.Position;

            foreach (var child in Children.OfType<ParallaxLayer>())
            {
                child.ApplyCameraDelta(camDelta);
            }
        }
    }
}