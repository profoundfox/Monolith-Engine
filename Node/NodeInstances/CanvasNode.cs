using System.IO.Compression;
using System.Reflection.Metadata.Ecma335;
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
        /// The self contained visibility of this node.
        /// </summary>
        public Visibility LocalVisibility
        {
            get => localVisibility;
            set
            {
                localVisibility = value;
                UpdateAttributes();
            }
        }

        /// <summary>
        /// The self contained visibility of this node. 
        /// </summary>
        public bool LocalVisible
        {
            get => LocalVisibility.Visibile;
            set
            {
                LocalVisibility = LocalVisibility with { Visibile = value };
            }
        }

        /// <summary>
        /// The self contained modulate of this node.
        /// </summary>
        public Color LocalModulate
        {
            get => LocalVisibility.Modulate;
            set
            {
                LocalVisibility = LocalVisibility with { Modulate = value };
            }
        }

        /// <summary>
        /// The self contained ordering of this node.
        /// </summary>
        public Ordering LocalOrdering
        {
            get => localOrdering;
            set
            {
                localOrdering = value;
                UpdateAttributes();
            }
        }

        /// <summary>
        /// The self contained depth of this node.
        /// </summary>
        public int LocalDepth
        {
            get => LocalOrdering.Depth;
            set
            {
                LocalOrdering = LocalOrdering with { Depth = value };
            }
        }

        /// <summary>
        /// The self contained material of this node.
        /// </summary>
        public Material LocalMaterial
        {
            get => localMaterial;
            set
            {
                localMaterial = value;
                UpdateAttributes();
            }
        }

        /// <summary>
        /// The self contained shader of this node.
        /// </summary>
        public Effect LocalShader
        {
            get => LocalMaterial.Shader;
            set
            {
                LocalMaterial = LocalMaterial with { Shader = value };
            }
        }

        /// <summary>
        /// The self contained sprite effects of this node.
        /// </summary>
        public SpriteEffects LocalSpriteEffects
        {
            get => LocalMaterial.SpriteEffects;
            set
            {
                LocalMaterial = LocalMaterial with { SpriteEffects = value };
            }
        }

        /// <summary>
        /// The combined visibility after inheriting from parents.
        /// </summary>
        public Visibility GlobalVisibility { get; private set; }

        /// <summary>
        /// The visibility relative to the parent.
        /// </summary>
        public bool GlobalVisible => GlobalVisibility.Visibile;

        /// <summary>
        /// The modulate relative to the parent.
        /// </summary>
        public Color GlobalModulate => GlobalVisibility.Modulate;

        /// <summary>
        /// The combined ordering after inheriting from parents.
        /// </summary>
        public Ordering GlobalOrdering { get; private set; }

        /// <summary>
        /// The depth relative to the parent.
        /// </summary>
        public int GlobalDepth => GlobalOrdering.Depth;

        /// <summary>
        /// The combined material after inheriting from parents.
        /// </summary>
        public Material GlobalMaterial { get; private set; }

        /// <summary>
        /// The shader relative to the parent.
        /// </summary>
        public Effect GlobalShader => GlobalMaterial.Shader;

        /// <summary>
        /// The global sprite effects relative to the parent.
        /// </summary>
        public SpriteEffects GlobalSpriteEffects => GlobalMaterial.SpriteEffects;


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