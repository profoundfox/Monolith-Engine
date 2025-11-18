using ConstructEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ConstructEngine.Managers
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
        /// A list of tilemaps that will be drawn alongside queued draw calls.
        /// </summary>
        public List<Tilemap> Tilemaps = new List<Tilemap>();

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
                var queue = _drawQueues[(DrawLayer)layer];
                if (queue.Count == 0) continue;

                _spriteBatch.Begin(
                    SpriteSortMode.BackToFront,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,
                    transformMatrix: layer == DrawLayer.UI ? Matrix.Identity : _cameraTransform
                );

                foreach (var call in queue)
                {
                    if (call.Texture != null)
                    {
                        _spriteBatch.Draw(
                            call.Texture,
                            call.Position,
                            call.SourceRectangle,
                            call.Color,
                            call.Rotation,
                            call.Origin,
                            call.Scale,
                            call.Effects,
                            call.LayerDepth
                        );
                    }
                }

                _spriteBatch.End();
                queue.Clear();
            }
        }
    }
}
