using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Nodes;
using Monolith.Graphics;
using Monolith;
using System;
using Monolith.Structs;

namespace Monolith.Nodes
{   
    public record class SpriteConfig : SpatialNodeConfig
    {
        public MTexture Texture { get; set; }
    }

    public class Sprite2D : Node2D
    {
        public MTexture Texture { get; set; }

        public Sprite2D(SpriteConfig cfg) : base(cfg)
        {
            Texture = cfg.Texture;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture.Draw(
                position: GlobalTransform.Position,
                color: GlobalVisibility.Modulate,
                rotation: Rotation,
                origin: Texture.Center,
                scale: Scale,
                effects: GlobalMaterial.SpriteEffects,
                shader: GlobalMaterial.Shader,
                layerDepth: GlobalOrdering.Depth
            );
        }
    }


}