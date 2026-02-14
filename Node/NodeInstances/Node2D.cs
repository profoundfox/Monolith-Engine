using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Geometry;
using Monolith.Graphics;
using Monolith.Helpers;
using Monolith.Managers;
using Monolith.Attributes;

namespace Monolith.Nodes
{

    public class Node2D : CanvasNode
    {
        private Transform2D _localTransform = Transform2D.Identity;

        public event Action<Transform2D> TransformChanged;


        /// <summary>
        /// The transform relative to the parent.
        /// </summary>
        public Transform2D GlobalTransform { get; private set; }

 

        /// <summary>
        /// The position of the node.
        /// </summary>
        public Vector2 Position
        {
            get => GlobalTransform.Position;
            set
            {
                _localTransform = _localTransform with { Position = value };
                UpdateGlobalTransform();
            }
        }

        /// <summary>
        /// The local rotation of the node, measured in radians.
        /// </summary>
        public float Rotation
        {
            get => GlobalTransform.Rotation;
            set
            {
                _localTransform = _localTransform with { Rotation = value };
                UpdateGlobalTransform();
            }
        }

        /// <summary>
        /// The local scale of the node.
        /// </summary>
        public Vector2 Scale
        {
            get => GlobalTransform.Scale;
            set
            {
                _localTransform = _localTransform with { Scale = value };
                UpdateGlobalTransform();
            }
        }

        /// <summary>
        /// Creates a new Node2D using a SpatialNodeConfig.
        /// </summary>
        public Node2D()
        {
            UpdateGlobalTransform();
        }

        protected override void OnParentChanged()
        {
            base.OnParentChanged();

            UpdateGlobalTransform();
        }

        /// <summary>
        /// Recompute global transform based on parent.
        /// Automatically ProcessProcessProcessProcessUpdates children.
        /// </summary>
        internal void UpdateGlobalTransform()
        {
            if (Parent is Node2D parent2D)
            {
                GlobalTransform = Transform2D.Combine(parent2D.GlobalTransform, _localTransform);
            }
            else
            {
                GlobalTransform = _localTransform;
            }

            TransformChanged?.Invoke(GlobalTransform);

            foreach (var child in Children)
            {
                if (child is Node2D c2d)
                    c2d.UpdateGlobalTransform();
            }
        }

        /// <summary>
        /// An offset function for adding onto the node's LocalPosition with a Vector2.
        /// Acts the same as +=.
        /// </summary>
        /// <param name="delta"></param>
        public void Offset(Vector2 delta)
        {
            Position += delta;
        }

        /// <summary>
        /// An offset function for adding onto the node's LocalPosition.
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
