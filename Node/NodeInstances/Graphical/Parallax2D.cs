using System.Linq;
using Microsoft.Xna.Framework;
using Monolith.Graphics;
using Monolith.Nodes;

namespace Monolith.Nodes
{
    public record class ParallaxConfig : SpatialNodeConfig { }
    public class Parallax2D : Node2D
    {
        private Vector2 lastCameraPos;

        public Parallax2D(ParallaxConfig cfg) : base(cfg) { }

        public override void ProcessUpdate(GameTime gameTime)
        {
            base.ProcessUpdate(gameTime);

            var camera = Camera2D.CurrentCameraInstance;
            Vector2 delta = camera.GlobalPosition - lastCameraPos;
            lastCameraPos = camera.GlobalPosition;

            foreach (var child in Children.OfType<ParallaxLayer>())
            {
                child.ApplyCameraDelta(delta);
            }
        }
    }
}