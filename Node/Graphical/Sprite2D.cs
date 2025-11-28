using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Nodes;
using Monolith.Graphics;
using Monolith;

namespace Monolith.Nodes
{   
    public class Sprite2DValues
    {
        public MTexture Texture { get; set; } = default!;
        public Vector2 Scale { get; set; } = Vector2.One;
        public float Rotation { get; set; } = 0f;
        public Color Modulate { get; set; } = Color.White;
    }


    public class Sprite2D : Node
    {
        public MTexture Texture { get; private set; }
        public Color Modulate { get; set; } = Color.White;
        public Vector2 Scale { get; set; } = Vector2.One;
        public float Rotation { get; set; } = 0f;

        public Sprite2D(NodeConfig config) : base(config)
        {
            var values = GetValues<Sprite2DValues>(); // ✅ strongly typed

            Texture = values.Texture;
            Scale = values.Scale;
            Rotation = values.Rotation;
            Modulate = values.Modulate;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture.Draw(
                position: Shape.Location.ToVector2(),
                color: Modulate,
                rotation: Rotation,
                origin: Texture.Center,
                scale: Scale,
                effects: SpriteEffects.None,
                layerDepth: 0f
            );
        }
    }

}