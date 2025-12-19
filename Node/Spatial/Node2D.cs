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
        /// Optional position for the node. Defaults to Vector2.Zero.
        /// </summary>
        public Vector2? Position { get; set; }
    }


    public class Node2D : Node
    {
        private Vector2 _position;

        public event Action<Vector2> PositionChanged;

        /// <summary>
        /// The current position of the node, linked with the shape's location.
        /// Propagates delta to all children.
        /// </summary>
        public Vector2 Position
        {
            get => _position;
            set
            {
                if (_position == value)
                    return;

                var delta = value - _position;
                _position = value;

                foreach (var child in Children)
                {
                    if (child is Node2D c2d)
                        c2d.Position += delta;
                }

                PositionChanged?.Invoke(_position);
            }
        }

        /// <summary>
        /// Creates a new Node2D using a Node2DConfig.
        /// </summary>
        public Node2D(SpatialNodeConfig config) : base(config)
        {
            _position = config.Position ?? Vector2.Zero;
        }

        /// <summary>
        /// An offset function for adding onto the node's position with a Vector2.
        /// Acts the same as +=.
        /// </summary>
        /// <param name="delta"></param>
        public void Offset(Vector2 delta)
        {
            Position = _position + delta;
        }

        /// <summary>
        /// An offset function for adding onto the node's position.
        /// Acts the same as +=.
        /// </summary>
        /// <param name="delta"></param>
        public void Offset(float x, float y)
        {
            Offset(new Vector2(x, y));
            
        }


        public override void Load()
        {
            base.Load();
        }

        public override void Unload()
        {
            base.Unload();
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
