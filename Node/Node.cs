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
        /// <summary>
        /// The object that the node is instantiated within.
        /// </summary>
        public object Parent { get; internal set; }

        /// <summary>
        /// The type of the Parent object.
        /// </summary>
        public Type ParentType => Parent?.GetType();

        /// <summary>
        /// The shape that the object has.
        /// </summary>
        public IRegionShape2D Shape { get; set; }

        /// <summary>
        /// The name of the node, defaults to the class name of the node.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The location of the node.
        /// </summary>
        public Point Location { get => Shape.Location; set => Shape.Location = value; }

        /// <summary>
        /// Values that come from the ogmo level dictionary, can be ignored if user is not using ogmo.
        /// </summary>
        public Dictionary<string, object> Values { get; internal set; } = new();

        /// <summary>
        /// Creates a new Node using a NodeConfig.
        /// </summary>
        public Node(NodeConfig config)
        {
            Parent = config.Parent;
            Shape = config.Shape;
            Name = config.Name;
            Values = config.Values;

            NodeManager.QueueAdd(this);
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
        /// Returns a list of all currently know children of this node.
        /// </summary>
        /// <returns></returns>
        public List<Node> GetChildren()
        {
            return NodeManager.GetNodeChildren(this);
        }

        /// <summary>
        /// Returns the current parrent of this node.
        /// </summary>
        /// <returns></returns>
        public object GetParent()
        {
            return NodeManager.GetNodeParent(this);
        }  

        /// <summary>
        /// Clears the node's data to help with memory management.
        /// </summary>
        internal void ClearNodeData()
        {
            Parent = null;
            Shape = null;
            Name = null;
            Values?.Clear();
            Values = null;
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
            DrawHelper.DrawRegionShape(Shape, color, layerDepth, layer);
        }

        /// <summary>
        /// Draws the shape with a hollow texture.
        /// </summary>
        public void DrawShapeHollow(Color color, int thickness = 2, float layerDepth = 0.1f, DrawLayer layer = DrawLayer.Middleground)
        {
            DrawHelper.DrawRegionShapeHollow(Shape, color, thickness, layerDepth, layer);
        }
    }
}
