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
            if (Texture == null)
                return; 
            
            Engine.Canvas.Call(new TextureDrawCall
            {
                Texture = Texture,
                Position = Transform.Global.Position,
                Color = Visibility.Global.Modulate,
                Rotation = Transform.Global.Rotation,
                Origin = Texture.Center,
                Scale = Transform.Global.Scale,
                Effects = Material.Global.SpriteEffects,
                Depth = Ordering.Global.Depth,
                SpriteBatchConfig = SpriteBatchConfig.Default with
                {
                    Effect = Material.Global.Shader
                }
            });
            
        }
    }


}