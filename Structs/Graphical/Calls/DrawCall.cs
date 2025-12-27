using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Graphics;

namespace Monolith.Structs
{
    /// <summary>
    /// Lightweight container to describe what to draw.
    /// </summary>
    public readonly struct DrawParams
    {
        public Texture2D Texture { get; init; }
        public Vector2 Position { get; init; }
        public Rectangle? SourceRectangle { get; init; }
        public Color Color { get; init; }
        public float Rotation { get; init; }
        public Vector2 Origin { get; init; }
        public Vector2 Scale { get; init; }
        public SpriteEffects Effects { get; init; }
        public int Depth { get; init; }
        public Effect Effect { get; init; }
        public bool UseCamera { get; init; }
        public SpriteBatchConfig SpriteBatchConfig { get; init; }

        public DrawParams(
            MTexture texture,
            Vector2 position,
            Color? color = null,
            float rotation = 0f,
            Vector2? origin = null,
            Vector2? scale = null,
            Rectangle? source = null,
            SpriteEffects effects = SpriteEffects.None,
            int depth = 0,
            Effect effect = null,
            bool useCamera = true,
            SpriteBatchConfig? spriteBatchConfig = null
            )
        {
            if (texture == null || texture.Texture == null)
                throw new ArgumentNullException(nameof(texture));
            Texture = texture.Texture;

            Position = position;

            SourceRectangle = source;

            if (color == null)
                Color = Color.White;
            else
                Color = color.Value;

            Rotation = rotation;

            if (origin == null)
                Origin = Vector2.Zero;
            else
                Origin = origin.Value;

            if (scale == null)
                Scale = Vector2.One;
            else
                Scale = scale.Value;

            Effects = effects;
            
            Depth = depth;

            Effect = effect;

            UseCamera = useCamera;

            if (spriteBatchConfig == null)
                SpriteBatchConfig = new SpriteBatchConfig();
            else
                SpriteBatchConfig = spriteBatchConfig.Value;
        }
    }

    /// <summary>
    /// Internal structure representing a queued draw call. Kept separate so DrawParams
    /// remains small and focused on user input.
    /// </summary>
    internal struct DrawCall
    {
        public Texture2D Texture;
        public Vector2 Position;
        public Rectangle? SourceRectangle;
        public Color Color;
        public float Rotation;
        public Vector2 Origin;
        public Vector2 Scale;
        public SpriteEffects Effects;
        public int Depth;
        public Effect Effect;
        public bool UseCamera;

        public SpriteBatchConfig SpriteBatchConfig;

        public Vector2 Offset;

        public DrawCall(in DrawParams p)
        {
            Texture = p.Texture;
            Position = p.Position;
            SourceRectangle = p.SourceRectangle;
            Color = p.Color;
            Rotation = p.Rotation;
            Origin = p.Origin;
            Scale = p.Scale;
            Effects = p.Effects;
            Depth = p.Depth;
            Effect = p.Effect;
            UseCamera = p.UseCamera;
            SpriteBatchConfig = p.SpriteBatchConfig;

            Offset = Vector2.Zero;
        }
    }
}