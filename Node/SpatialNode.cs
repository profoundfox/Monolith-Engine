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

        /// <summary>
        /// The rotation of the node measured in radians
        /// </summary>
        public float Rotation { get; set; } = 0f;

        /// <summary>
        /// The scale of the node.
        /// </summary>
        public Vector2 Scale { get; set; } = Vector2.Zero;
    }


    public class Node2D : Node
    {
        private Transform2D _localTransform;

        public event Action<Transform2D> TransformChanged;

        /// <summary>
        /// The transform relative to this node's parent.
        /// </summary>
        public Transform2D LocalTransform
        {
            get => _localTransform;
            set
            {
                _localTransform = value;
                UpdateGlobalTransform();
            }
        }

        /// <summary>
        /// The visual configuration for the node.
        /// </summary>
        public Visual2D Visual { get; set; } = Visual2D.Identity;

        /// <summary>
        /// The transform relative to global coordinates.
        /// </summary>
        public Transform2D GlobalTransform { get; private set; }

        /// <summary>
        /// The global position of the node.
        /// </summary>
        public Vector2 GlobalPosition
        {
            get => GlobalTransform.Position;
        }

        /// <summary>
        /// The local position of the node, relative to its parent.
        /// </summary>
        public Vector2 Position
        {
            get => LocalTransform.Position;
            set => LocalTransform = LocalTransform with { Position = value };
        }

        /// <summary>
        /// The local rotation of the node, measured in radians.
        /// </summary>
        public float Rotation
        {
            get => LocalTransform.Rotation;
            set => LocalTransform = LocalTransform with { Rotation = value };
        }

        /// <summary>
        /// The local scale of the node.
        /// </summary>
        public Vector2 Scale
        {
            get => LocalTransform.Scale;
            set => LocalTransform = LocalTransform with { Scale = value };
        }

        /// <summary>
        /// The local origin of the node.
        /// </summary>
        public Vector2 Origin
        {
            get => LocalTransform.Origin;
            set => LocalTransform = LocalTransform with { Origin = value };
        }

        /// <summary>
        /// Creates a new Node2D using a SpatialNodeConfig.
        /// </summary>
        public Node2D(SpatialNodeConfig cfg) : base(cfg)
        {
            _localTransform = new Transform2D(cfg.Position ?? Vector2.Zero, rotation: cfg.Rotation, scale: cfg.Scale);
            UpdateGlobalTransform();
        }
        /// <summary>
        /// Recompute global transform based on parent.
        /// Automatically updates children.
        /// </summary>
        private void UpdateGlobalTransform()
        {
            if (Parent is Node2D parent2D)
            {
                GlobalTransform = Transform2D.Combine(parent2D.GlobalTransform, LocalTransform);
            }
            else
            {
                GlobalTransform = LocalTransform;
            }

            TransformChanged?.Invoke(GlobalTransform);

            foreach (var child in Children)
            {
                if (child is Node2D c2d)
                    c2d.UpdateGlobalTransform();
            }
        }

        /// <summary>
        /// An offset function for adding onto the node's position with a Vector2.
        /// Acts the same as +=.
        /// </summary>
        /// <param name="delta"></param>
        public void Offset(Vector2 delta)
        {
            Position += delta;
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
