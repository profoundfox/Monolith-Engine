using Monolith.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Monolith.Managers
{
    /// <summary>
    /// An enum representing four different layers, back to front.
    /// </summary>
    public enum DrawLayer
    {
        Background,
        Middleground,
        Foreground,
        UI
    }

    /// <summary>
    /// Represents a single batched sprite draw call containing all rendering
    /// parameters needed to draw a texture using SpriteBatch.
    /// </summary>
    /// <remarks>
    /// This struct stores texture reference, draw positioning, source region,
    /// tint color, rotation, origin pivot, scaling, sprite effects, depth layering,
    /// and optional shader effects.
    /// </remarks>
    public struct DrawCall
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

        public DrawCall(bool initializeDefaults)
        {
            Texture = null;
            Position = Vector2.Zero;
            SourceRectangle = null;
            Color = Color.White;
            Rotation = 0f;
            Origin = Vector2.Zero;
            Scale = Vector2.One;
            Effects = SpriteEffects.None;
            LayerDepth = 0f;
            Effect = null;
            LoopX = false;
            LoopY = false;
            Offset = Vector2.Zero;
            UseCamera = true;
        }
    }

    /// <summary>
    /// The manager responsible for batching, organizing, and drawing queued sprites
    /// across multiple draw layers in the correct order.
    /// </summary>
    public partial class DrawManager
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly Dictionary<DrawLayer, List<DrawCall>> _drawQueues;
        private Matrix _cameraTransform = Matrix.Identity;

        /// <summary>
        /// Creates a new DrawManager using the provided SpriteBatch.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch used for rendering.</param>
        public DrawManager(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
            _drawQueues = new Dictionary<DrawLayer, List<DrawCall>>();

            foreach (DrawLayer layer in Enum.GetValues(typeof(DrawLayer)))
                _drawQueues[layer] = new List<DrawCall>();
        }

        /// <summary>
        /// Sets the camera transformation matrix applied to non-UI draw layers.
        /// </summary>
        /// <param name="transform">A Matrix representing the camera transform.</param>
        public void SetCamera(Matrix transform)
        {
            _cameraTransform = transform;
        }

        /// <summary>
        /// Adds a sprite draw call to the specified draw layer’s queue.
        /// </summary>
        /// <param name="drawCall">The draw call to enqueue.</param>
        /// <param name="layer">The draw layer to queue it in. Defaults to Middleground.</param>
        public void Queue(DrawCall drawCall, DrawLayer layer = DrawLayer.Middleground)
        {
            _drawQueues[layer].Add(drawCall);
        }

    

        /// <summary>
        /// Draws all queued sprites to the screen, grouped by layer and then cleared.
        /// </summary>
        /// <remarks>
        /// Layers are drawn from back to front, and SpriteBatch is begun and ended
        /// once per layer. UI layer is drawn without camera transform.
        /// </remarks>
        
        public void Flush()
        {
            foreach (DrawLayer layer in Enum.GetValues(typeof(DrawLayer)))
            {
                var queue = _drawQueues[layer];
                if (queue.Count == 0) continue;

                var cameraCalls = queue.FindAll(c => c.UseCamera);
                var screenCalls = queue.FindAll(c => !c.UseCamera);

                if (cameraCalls.Count > 0)
                {
                    _spriteBatch.Begin(
                        SpriteSortMode.BackToFront,
                        BlendState.AlphaBlend,
                        SamplerState.PointClamp,
                        transformMatrix: layer == DrawLayer.UI ? Matrix.Identity : _cameraTransform
                    );

                    foreach (var call in cameraCalls)
                        DrawInternal(call);

                    _spriteBatch.End();
                }

                // 2. Draw screen-space sprites (no camera)
                if (screenCalls.Count > 0)
                {
                    _spriteBatch.Begin(
                        SpriteSortMode.BackToFront,
                        BlendState.AlphaBlend,
                        SamplerState.PointClamp,
                        transformMatrix: Matrix.Identity
                    );

                    foreach (var call in screenCalls)
                        DrawInternal(call);

                    _spriteBatch.End();
                }

                queue.Clear();
            }
        }

        private void DrawInternal(DrawCall call)
        {
            Rectangle src = call.SourceRectangle ??
                            new Rectangle(0, 0, call.Texture.Width, call.Texture.Height);

            if (call.LoopX)
                src.X = ((int)call.Offset.X) % call.Texture.Width;
            if (call.LoopY)
                src.Y = ((int)call.Offset.Y) % call.Texture.Height;

            _spriteBatch.Draw(
                call.Texture,
                call.Position,
                src,
                call.Color,
                call.Rotation,
                call.Origin,
                call.Scale,
                call.Effects,
                call.LayerDepth
            );
        }

    }
}
