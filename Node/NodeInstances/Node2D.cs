using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Geometry;
using Monolith.Graphics;
using Monolith.Helpers;
using Monolith.Managers;
using Monolith.Attributes;
using System.IO.Compression;

namespace Monolith.Nodes
{

    public class Node2D : CanvasNode
    {
        private Transform2D _localTransform = Transform2D.Identity;

        public event Action<Transform2D> TransformChanged;

        /// <summary>
        /// The self contained transform of this node.
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
        /// The self contained position of this node, updates child nodes' position.
        /// </summary>
        public Vector2 LocalPosition
        {
            get => LocalTransform.Position;
            set
            {
                LocalTransform = LocalTransform with { Position = value };
            }
        }

        /// <summary>
        /// The self contained rotation of this node, updates child node's rotation.
        /// </summary>
        public float LocalRotation
        {
            get => LocalTransform.Rotation;
            set
            {
                LocalTransform = LocalTransform with { Rotation = value };
            }
        }

        /// <summary>
        /// The self contained scale of this node, updates child node's scale.
        /// </summary>
        public Vector2 LocalScale
        {
            get => LocalTransform.Scale;
            set
            {
                LocalTransform = LocalTransform with { Scale = value };
            }
        }
    

        /// <summary>
        /// The transform relative to the parent.
        /// </summary>
        public Transform2D GlobalTransform { get; private set; }

        /// <summary>
        /// The position relative to the parent.
        /// </summary>
        public Vector2 GlobalPosition => GlobalTransform.Position;

        /// <summary>
        /// The rotation reltaive to the parent.
        /// </summary>
        public float GlobalRotation => GlobalTransform.Rotation;

        /// <summary>
        /// The scale relative to the parent.
        /// </summary>
        public Vector2 GlobalScale => GlobalTransform.Scale;

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
            LocalPosition += delta;
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
