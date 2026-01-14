using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Geometry;

namespace Monolith.Nodes
{
    public record class RayCastConfig : SpatialNodeConfig
    {
        public RayCast Ray { get; set; }
    }

    public class Raycast2D : Node2D
    {
        public RayCast Ray { get; set; }
        public bool Disabled { get; set; }
        
        public Raycast2D(RayCastConfig cfg) : base(cfg)
        {
            Ray = cfg.Ray;
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