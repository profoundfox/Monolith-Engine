using Monolith.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Monolith.Managers
{
    public enum DrawLayer
    {
        Background,
        Middleground,
        Foreground,
        UI
    }

    /// <summary>
    /// Lightweight container used by callers to describe what to draw.
    /// Provides sensible defaults and keeps Draw() call-sites compact.
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
        public float LayerDepth { get; init; }
        public Effect Effect { get; init; }
        public bool UseCamera { get; init; }

        public DrawParams(
            MTexture texture,
            Vector2 position,
            Color? color = null,
            float rotation = 0f,
            Vector2? origin = null,
            Vector2? scale = null,
            Rectangle? source = null,
            SpriteEffects effects = SpriteEffects.None,
            float layerDepth = 0f,
            Effect effect = null,
            bool useCamera = true)
        {
            Texture = texture.Texture ?? throw new ArgumentNullException(nameof(texture));
            Position = position;
            SourceRectangle = source;
            Color = color ?? Color.White;
            Rotation = rotation;
            Origin = origin ?? Vector2.Zero;
            Scale = scale ?? Vector2.One;
            Effects = effects;
            LayerDepth = layerDepth;
            Effect = effect;
            UseCamera = useCamera;
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
        public float LayerDepth;
        public Effect Effect;
        public bool UseCamera;

        public bool LoopX;
        public bool LoopY;
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
            LayerDepth = p.LayerDepth;
            Effect = p.Effect;
            UseCamera = p.UseCamera;

            LoopX = false;
            LoopY = false;
            Offset = Vector2.Zero;
        }
    }

    /// <summary>
    /// Responsible for queueing draw calls and flushing them to a SpriteBatch.
    /// </summary>
    public sealed partial class DrawManager
    {
        private readonly Dictionary<Effect, SpriteBatch> _spriteBatches = new();
        private readonly SpriteBatch _spriteBatch;
        private readonly Dictionary<DrawLayer, List<DrawCall>> _queues;
        private Matrix _camera = Matrix.Identity;

        public DrawManager(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch ?? throw new ArgumentNullException(nameof(spriteBatch));
            _queues = new Dictionary<DrawLayer, List<DrawCall>>();

            foreach (DrawLayer l in Enum.GetValues(typeof(DrawLayer)))
                _queues[l] = new List<DrawCall>();
        }

        /// <summary>
        /// Update the camera transform used for non-UI layers.
        /// </summary>
        public void SetCamera(Matrix transform) => _camera = transform;

        /// <summary>
        /// Queue a draw using a compact DrawParams structure.
        /// </summary>
        public void Draw(in DrawParams p, DrawLayer layer = DrawLayer.Middleground)
        {
            var call = new DrawCall(p);
            _queues[layer].Add(call);
        }

        /// <summary>
        /// Queues a tiled texture.
        /// </summary>
        private void EnqueueLooping(MTexture texture, Rectangle source, Vector2 position, Vector2 offset,
            DrawLayer layer, Color color, float layerDepth, bool useCamera)
        {
            int screenW = _spriteBatch.GraphicsDevice.Viewport.Width;
            int screenH = _spriteBatch.GraphicsDevice.Viewport.Height;

            int tileW = source.Width;
            int tileH = source.Height;

            int tilesX = screenW / tileW + 2;
            int tilesY = screenH / tileH + 2;

            float offsetX = offset.X % tileW;
            float offsetY = offset.Y % tileH;

            for (int x = 0; x < tilesX; x++)
            {
                for (int y = 0; y < tilesY; y++)
                {
                    var call = new DrawCall(new DrawParams(
                        texture,
                        new Vector2(x * tileW - offsetX + position.X, y * tileH - offsetY + position.Y),
                        color,
                        0f,
                        Vector2.Zero,
                        Vector2.One,
                        source,
                        SpriteEffects.None,
                        layerDepth,
                        effect: null,
                        useCamera: useCamera));

                    call.LoopX = true;
                    call.LoopY = true;
                    call.Offset = offset;

                    _queues[layer].Add(call);
                }
            }
        }

        public void DrawLooping(MTexture texture, Vector2 position, Vector2 offset,
            DrawLayer layer = DrawLayer.Middleground, Color? color = null, float layerDepth = 0f)
        {
            Rectangle source = texture.SourceRectangle ?? new Rectangle(0, 0, texture.Width, texture.Height);
            EnqueueLooping(texture, source, position, offset, layer, color ?? Color.White, layerDepth, useCamera: false);
        }

        private void DrawInternal(SpriteBatch sb, in DrawCall call)
        {
            if (call.Texture == null) return;

            Rectangle src = call.SourceRectangle ?? new Rectangle(0, 0, call.Texture.Width, call.Texture.Height);

            if (call.LoopX || call.LoopY)
            {
                int tileW = src.Width;
                int tileH = src.Height;

                int screenW = _spriteBatch.GraphicsDevice.Viewport.Width;
                int screenH = _spriteBatch.GraphicsDevice.Viewport.Height;

                int startX = call.LoopX ? (int)(-call.Offset.X % tileW) - tileW : 0 - tileW;
                int startY = call.LoopY ? (int)(-call.Offset.Y % tileH) - tileH : 0 - tileH;

                for (int x = startX; x < screenW + tileW; x += tileW)
                {
                    for (int y = startY; y < screenH + tileH; y += tileH)
                    {
                        sb.Draw(
                            call.Texture,
                            new Vector2(call.Position.X + x, call.Position.Y + y),
                            src,
                            call.Color,
                            call.Rotation,
                            call.Origin,
                            call.Scale,
                            call.Effects,
                            call.LayerDepth);
                    }
                }

                return;
            }

            sb.Draw(
                call.Texture,
                call.Position,
                src,
                call.Color,
                call.Rotation,
                call.Origin,
                call.Scale,
                call.Effects,
                call.LayerDepth);
        }


        /// <summary>
        /// Flush all queued draws to the SpriteBatch. The flush order is:
        /// for each DrawLayer (back to front): group by UseCamera -> Effect and draw.
        /// This keeps Begin/End calls minimal but safe when switching shader effects.
        /// </summary>
        public void Flush()
        {
            foreach (DrawLayer layer in Enum.GetValues(typeof(DrawLayer)))
            {
                var queue = _queues[layer];
                if (queue.Count == 0) continue;

                var groups = queue.GroupBy(c => (UseCamera: layer != DrawLayer.UI && c.UseCamera, Effect: c.Effect));

                foreach (var group in groups)
                {
                    SpriteBatch sb;
                    Effect effect = group.Key.Effect;

                    if (effect == null)
                    {
                        sb = _spriteBatch;
                    }
                    else
                    {
                        if (!_spriteBatches.TryGetValue(effect, out sb))
                        {
                            sb = new SpriteBatch(_spriteBatch.GraphicsDevice);
                            _spriteBatches[effect] = sb;
                        }
                    }

                    sb.Begin(
                        SpriteSortMode.BackToFront,
                        BlendState.AlphaBlend,
                        SamplerState.PointClamp,
                        transformMatrix: group.Key.UseCamera ? _camera : Matrix.Identity,
                        effect: effect
                    );

                    foreach (var call in group)
                        DrawInternal(sb, call);

                    sb.End();
                }

                queue.Clear();
            }
        }


    }
}
