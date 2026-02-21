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
            Engine.Screen.Call(new TextureDrawCall
            {
                Texture = Texture,
                Position = GlobalPosition,
                Color = Modulate,
                Rotation = Rotation,
                Origin = Texture.Center,
                Scale = Scale,
                Effects = SpriteEffects,
                Effect = Shader,
                Depth = Depth
            });
            
        }
    }


}