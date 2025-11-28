using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Managers;

namespace Monolith.Graphics
{
    public class Sprite
    {
        public MTexture Texture { get; set; }
        public Color Color { get; set; } = Color.White;
        public float Rotation { get; set; } = 0.0f;
        public Vector2 Scale { get; set; } = Vector2.One;
        public Vector2 Origin { get; set; } = Vector2.Zero;
        public Vector2 Position { get; set; } = Vector2.Zero;
        public SpriteEffects Effects { get; set; } = SpriteEffects.None;
        public float LayerDepth { get; set; } = 0.0f;

        public float Width => Texture.Width * Scale.X;
        public float Height => Texture.Height * Scale.Y;

        public Sprite() {}

        public Sprite(MTexture texture)
        {
            Texture = texture ?? throw new ArgumentNullException(nameof(texture));
        }

        public void CenterOrigin()
        {
            Origin = new Vector2(Texture.Width, Texture.Height) * 0.5f;
        }

        public void Draw() => Texture.Draw(Position, Color, Rotation, Origin, Scale, Effects, LayerDepth);

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Position = position;
            Texture.Draw(Position, Color, Rotation, Origin, Scale, Effects, LayerDepth);
        }

        /// <summary>
        /// Gets the effective source rectangle for this sprite.
        /// Falls back to full texture if null.
        /// </summary>
        public Rectangle SourceRectangle => Texture.SourceRectangle ?? new Rectangle(0, 0, Texture.Width, Texture.Height);
    }
}
