using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Attributes;

namespace Monolith.Nodes
{
    public record class CanvasNodeConfig : NodeConfig
    {
        public int Depth { get; set; }
    }

    public class CanvasNode : Node
    {
        private Visibility _localVisibility;
        private Ordering _localOrdering;
        private Material _localMaterial;

        public Visibility LocalVisibility
        {
            get => _localVisibility;
            set
            {
                _localVisibility = value;
                ProcessUpdateAttributes();
            }
        }

        public Ordering LocalOrdering
        {
            get => _localOrdering;
            set
            {
                _localOrdering = value;
                ProcessUpdateAttributes();
            }
        }

        public int LocalDepth
        {
            get => LocalOrdering.Depth;
            set => LocalOrdering = LocalOrdering with { Depth = value };
        }

        public Material LocalMaterial
        {
            get => _localMaterial;
            set
            {
                _localMaterial = value;
                ProcessUpdateAttributes();
            }
        }

        public Visibility GlobalVisibility { get; private set; }
        public Ordering GlobalOrdering { get; private set; }
        public Material GlobalMaterial { get; private set; }

        public CanvasNode(CanvasNodeConfig cfg) : base(cfg)
        {
            _localVisibility = Visibility.Identity;
            _localOrdering = Ordering.Identity with { Depth = cfg.Depth };
            _localMaterial = Material.Identity;
            
            ProcessUpdateAttributes();
        }

        public void ProcessUpdateAttributes()
        {
            if (Parent is CanvasNode parent2D)
            {
                GlobalVisibility = Visibility.Combine(parent2D.GlobalVisibility, LocalVisibility);
                GlobalOrdering = Ordering.Combine(parent2D.GlobalOrdering, LocalOrdering);
                GlobalMaterial = Material.Combine(parent2D.GlobalMaterial, LocalMaterial);
            }
            else
            {
                GlobalVisibility = LocalVisibility;
                GlobalOrdering = LocalOrdering;
                GlobalMaterial = LocalMaterial;
            }

            foreach (var child in Children)
            {
                if (child is CanvasNode c2d)
                    c2d.ProcessUpdateAttributes();
            }
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