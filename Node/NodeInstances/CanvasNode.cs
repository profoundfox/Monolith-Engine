using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Structs;

namespace Monolith.Nodes
{
    public record class CanvasNodeConfig : NodeConfig
    {
        
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
                ProcessUpdateStructs();
            }
        }

        public Ordering LocalOrdering
        {
            get => _localOrdering;
            set
            {
                _localOrdering = value;
                ProcessUpdateStructs();
            }
        }

        public Material LocalMaterial
        {
            get => _localMaterial;
            set
            {
                _localMaterial = value;
                ProcessUpdateStructs();
            }
        }

        public Visibility GlobalVisibility { get; private set; }
        public Ordering GlobalOrdering { get; private set; }
        public Material GlobalMaterial { get; private set; }

        public CanvasNode(CanvasNodeConfig cfg) : base(cfg)
        {
            _localVisibility = Visibility.Identity;
            _localOrdering = Ordering.Identity;
            _localMaterial = Material.Identity;
            
            ProcessUpdateStructs();
        }

        public void ProcessUpdateStructs()
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
                    c2d.ProcessUpdateStructs();
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

        public override void ProcessUpdate(GameTime gameTime)
        {
            base.ProcessUpdate(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}