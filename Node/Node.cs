using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Region;
using Monolith.Helpers;
using Monolith.Managers;

namespace Monolith.Nodes
{
    public abstract class Node
    {
        private readonly List<Node> children = new();

        private Vector2 _position;

        /// <summary>
        /// The parent node in the hierarchy, required to be set, can be null
        /// </summary>
        public Node Parent { get; private set; }

        /// <summary>
        /// The shape that the object has.
        /// </summary>
        public IRegionShape2D Shape { get; set; }

        /// <summary>
        /// The name of the node, required to be set.
        /// </summary>
        public string Name { get; set; }

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

                if (Shape != null)
                    Shape.Location = new Point((int)MathF.Round(_position.X), (int)MathF.Round(_position.Y));

                foreach (var child in children)
                    child.Position += delta;
            }
        }

        /// <summary>
        /// Creates a new Node using a NodeConfig.
        /// </summary>
        public Node(NodeConfig config)
        {
            Shape = config.Shape;
            Name = config.Name ?? GetType().Name;

            _position = Shape != null
                ? new Vector2(Shape.Location.X, Shape.Location.Y)
                : (config.Position ?? Vector2.Zero);

            if (Shape != null)
                Shape.Location = new Point((int)MathF.Round(_position.X), (int)MathF.Round(_position.Y));

            if (config.Parent != null)
                SetParent(config.Parent);

            NodeManager.QueueAdd(this);
        }

        /// <summary>
        /// Sets the parent of this node, automatically updating the children lists.
        /// </summary>
        public void SetParent(Node newParent)
        {
            if (Parent == newParent) return;

            Parent?.children.Remove(this);
            Parent = newParent;
            Parent?.children.Add(this);
        }

        /// <summary>
        /// Adds a child to this node.
        /// </summary>
        public void AddChild(Node child)
        {
            if (child == null || children.Contains(child)) return;

            child.SetParent(this);
        }

        /// <summary>
        /// Removes a child from this node.
        /// </summary>
        public void RemoveChild(Node child)
        {
            if (child == null || !children.Contains(child)) return;

            child.SetParent(null);
        }

        /// <summary>
        /// Queues a node for removal from the main instance list and clears its data.
        /// </summary>
        public void QueueFree()
        {
            NodeManager.QueueRemove(this);
        }

        /// <summary>
        /// Removes the instance of this node immediately.
        /// </summary>
        public void FreeImmediate()
        {
            NodeManager.RemoveImmediate(this);
        }

        /// <summary>
        /// Clears the node's data to help with memory management.
        /// </summary>
        internal void ClearNodeData()
        {
            Parent = null;
            children.Clear();
            Shape = null;
            Name = null;
        }

        public virtual void Load() { }
        public virtual void Unload() { }
        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(SpriteBatch spriteBatch) { }

        /// <summary>
        /// Draws the shape with a filled texture.
        /// </summary>
        public void DrawShape(Color color, float layerDepth = 0.1f, DrawLayer layer = DrawLayer.Middleground)
        {
            if (Shape != null)
                DrawHelper.DrawRegionShape(Shape, color, layerDepth, layer);
        }

        /// <summary>
        /// Draws the shape with a hollow texture.
        /// </summary>
        public void DrawShapeHollow(Color color, int thickness = 2, float layerDepth = 0.1f, DrawLayer layer = DrawLayer.Middleground)
        {
            if (Shape != null)
                DrawHelper.DrawRegionShapeHollow(Shape, color, thickness, layerDepth, layer);
        }

         public IReadOnlyList<Node> Children => children.AsReadOnly();
    }
}
