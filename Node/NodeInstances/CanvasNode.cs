using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Attributes;

namespace Monolith.Nodes
{
    public class CanvasNode : Node
    {
        private Visibility localVisibility = Visibility.Identity;
        private Ordering localOrdering = Ordering.Identity;
        private Material localMaterial = Material.Identity;

        /// <summary>
        /// The combined visibility after inheriting from parents.
        /// </summary>
        public Visibility GlobalVisibility { get; private set; }

        /// <summary>
        /// The combined ordering after inheriting from parents.
        /// </summary>
        public Ordering GlobalOrdering { get; private set; }

        /// <summary>
        /// The combined material after inheriting from parents.
        /// </summary>
        public Material GlobalMaterial { get; private set; }

        /// <summary>
        /// Whether the node is visible.
        /// </summary>
        public bool Visible
        {
            get => GlobalVisibility.Visibile;
            set
            {
                localVisibility = localVisibility with { Visibile = value };
                UpdateAttributes();
            }
        }

        /// <summary>
        /// The color modulation applied to this node.
        /// </summary>
        public Color Modulate
        {
            get => GlobalVisibility.Modulate;
            set
            {
                localVisibility = localVisibility with { Modulate = value };
                UpdateAttributes();
            }
        }

        /// <summary>
        /// The rendering depth of the node.
        /// </summary>
        public int Depth
        {
            get => GlobalOrdering.Depth;
            set
            {
                localOrdering = localOrdering with { Depth = value };
                UpdateAttributes();
            }
        }

        /// <summary>
        /// The shader used by this node.
        /// </summary>
        public Effect Shader
        {
            get => GlobalMaterial.Shader;
            set
            {
                localMaterial = localMaterial with { Shader = value };
                UpdateAttributes();
            }
        }

        /// <summary>
        /// The sprite effects applied to this node.
        /// </summary>
        public SpriteEffects SpriteEffects
        {
            get => GlobalMaterial.SpriteEffects;
            set
            {
                localMaterial = localMaterial with { SpriteEffects = value };
                UpdateAttributes();
            }
        }

        public CanvasNode()
        {
            UpdateAttributes();
        }

        protected override void OnParentChanged()
        {
            base.OnParentChanged();
            
            UpdateAttributes();
        }

        /// <summary>
        /// Recalculates global rendering attributes and propagates them to children.
        /// </summary>
        private void UpdateAttributes()
        {
            if (Parent is CanvasNode parentCanvas)
            {
                GlobalVisibility = Visibility.Combine(parentCanvas.GlobalVisibility, localVisibility);
                GlobalOrdering = Ordering.Combine(parentCanvas.GlobalOrdering, localOrdering);
                GlobalMaterial = Material.Combine(parentCanvas.GlobalMaterial, localMaterial);
            }
            else
            {
                GlobalVisibility = localVisibility;
                GlobalOrdering = localOrdering;
                GlobalMaterial = localMaterial;
            }

            foreach (var child in Children)
            {
                if (child is CanvasNode canvasChild)
                    canvasChild.UpdateAttributes();
            }
        }
    }

}