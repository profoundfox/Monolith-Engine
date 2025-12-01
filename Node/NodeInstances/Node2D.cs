using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Geometry;
using Monolith.Helpers;
using Monolith.Managers;

namespace Monolith.Nodes
{
    public record class SpatialNodeConfig : NodeConfig
    {
        /// <summary>
        /// Optional shape for the node.
        /// </summary>
        public IRegionShape2D Region { get; set; }

        /// <summary>
        /// Optional position for the node. Defaults to Vector2.Zero.
        /// </summary>
        public Vector2? Position { get; set; }
    }


    public abstract class Node2D : Node
    {
        private Vector2 _position;

        /// <summary>
        /// The shape that the object has.
        /// </summary>
        public IRegionShape2D Region { get; set; }

        /// <summary>
        /// The current position of the node, linked with the shape's location.
        /// Propagates delta to all children.
        /// </summary>
        public Vector2 Position
        {
            get => _position;
            set
            {
                var delta = value - _position;
                _position = value;

                if (Region != null)
                    Region.Location = new Point((int)MathF.Round(_position.X), (int)MathF.Round(_position.Y));

                foreach (var child in Children)
                {
                    if (child is Node2D c2d)
                        c2d.Position += delta;
                }
            }
        }

        /// <summary>
        /// Creates a new Node2D using a Node2DConfig.
        /// </summary>
        public Node2D(SpatialNodeConfig config) : base(config)
        {
            Region = config.Region;

            _position = Region != null
                ? new Vector2(Region.Location.X, Region.Location.Y)
                : (config.Position ?? Vector2.Zero);

            if (Region != null)
                Region.Location = new Point((int)MathF.Round(_position.X), (int)MathF.Round(_position.Y));
        }

        /// <summary>
        /// Draws the shape with a filled texture.
        /// </summary>
        public void DrawShape(Color color, float layerDepth = 0.1f, DrawLayer layer = DrawLayer.Middleground)
        {
            if (Region != null)
                DrawHelper.DrawRegionShape(Region, color, layerDepth, layer);
        }

        /// <summary>
        /// Draws the shape with a hollow texture.
        /// </summary>
        public void DrawShapeHollow(Color color, int thickness = 2, float layerDepth = 0.1f, DrawLayer layer = DrawLayer.Middleground)
        {
            if (Region != null)
                DrawHelper.DrawRegionShapeHollow(Region, color, thickness, layerDepth, layer);
        }
    }
}
