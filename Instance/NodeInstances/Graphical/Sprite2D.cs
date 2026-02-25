using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Instances;
using Monolith.Graphics;
using Monolith;
using System;
using Monolith.Attributes;

namespace Monolith.Instances
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
                Color = GlobalModulate,
                Rotation = GlobalRotation,
                Origin = Texture.Center,
                Scale = GlobalScale,
                Effects = GlobalSpriteEffects,
                Effect = GlobalShader,
                Depth = GlobalDepth
            });
            
        }
    }


}