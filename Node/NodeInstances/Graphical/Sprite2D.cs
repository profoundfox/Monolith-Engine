using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Nodes;
using Monolith.Graphics;
using Monolith;
using System;
using Monolith.Attributes;

namespace Monolith.Nodes
{   

    public class Sprite2D : Node2D
    {
        public MTexture Texture { get; set; }

        public Sprite2D() {}
        
        public override void SubmitCall()
        {
            Texture.Draw(
                position: GlobalTransform.Position,
                color: GlobalVisibility.Modulate,
                rotation: GlobalTransform.Rotation,
                origin: Texture.Center,
                scale: GlobalTransform.Scale,
                effects: GlobalMaterial.SpriteEffects,
                shader: GlobalMaterial.Shader,
                depth: GlobalOrdering.Depth
            );
        }
    }


}