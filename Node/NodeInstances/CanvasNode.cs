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

        public Visibility LocalVisibility
        {
            get => _localVisibility;
            set
            {
                _localVisibility = value;
                UpdateStructs();
            }
        }

        public Ordering LocalOrdering
        {
            get => _localOrdering;
            set
            {
                _localOrdering = value;
                UpdateStructs();
            }
        }

        public Visibility GlobalVisibility { get; private set; }
        public Ordering GlobalOrdering { get; private set; }

        public CanvasNode(CanvasNodeConfig cfg) : base(cfg)
        {
            
        }

        public void UpdateStructs()
        {
            if (Parent is CanvasNode parent2D)
            {
                GlobalVisibility = Visibility.Combine(parent2D.GlobalVisibility, LocalVisibility);
                GlobalOrdering = Ordering.Combine(parent2D.GlobalOrdering, LocalOrdering);
            }
            else
            {
                GlobalVisibility = LocalVisibility;
                GlobalOrdering = LocalOrdering;
            }

            foreach (var child in Children)
            {
                if (child is CanvasNode c2d)
                    c2d.UpdateStructs();
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