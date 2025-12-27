using Monolith.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Monolith.Structs;

namespace Monolith.Managers
{   

    public enum DrawLayer
    {
        Background,
        Middleground,
        Foreground,
        UI
    }

    public sealed partial class DrawManager
    {
        private readonly Dictionary<SpriteBatchConfig, SpriteBatch> _spriteBatches = new();
        private readonly SpriteBatch _spriteBatch;
        private readonly Dictionary<DrawLayer, List<DrawCall>> _queues;
        private readonly Dictionary<DrawLayer, List<TextDrawCall>> _textQueues = new();
        private Matrix _camera = Matrix.Identity;

        public DrawManager(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch ?? throw new ArgumentNullException(nameof(spriteBatch));
            _queues = new Dictionary<DrawLayer, List<DrawCall>>();

            foreach (DrawLayer l in Enum.GetValues(typeof(DrawLayer)))
            {
                _queues[l] = new List<DrawCall>();
                _textQueues[l] = new List<TextDrawCall>();
            }
        }

        /// <summary>
        /// Updates the camera transform used for non-UI layers.
        /// </summary>
        public void SetCamera(Matrix transform) => _camera = transform;

        /// <summary>
        /// Queue a draw using DrawParams.
        /// </summary>
        public void Draw(in DrawParams p, DrawLayer layer = DrawLayer.Middleground)
        {
            var call = new DrawCall(p);
            _queues[layer].Add(call);
        }

        public void DrawString(
            string text,
            Vector2 position,
            Color color,
            DrawLayer layer = DrawLayer.UI,
            int depth = 0,
            SpriteBatchConfig? config = null,
            bool useCamera = false)
        {
            var cfg = config ?? SpriteBatchConfig.Default;

            _textQueues[layer].Add(
                new TextDrawCall(
                    Engine.Instance.Font,
                    text,
                    position,
                    color,
                    depth,
                    cfg,
                    useCamera
                )
            );
        }



        private void DrawInternal(SpriteBatch sb, in DrawCall call)
        {
            if (call.Texture == null) return;

            Rectangle src = call.SourceRectangle ?? new Rectangle(0, 0, call.Texture.Width, call.Texture.Height);  

            int minDepth = -100;
            int maxDepth = 100;

            float layerDepth = 1.0f - ((float)(call.Depth - minDepth) / (maxDepth - minDepth));         

            sb.Draw(
                call.Texture,
                call.Position,
                src,
                call.Color,
                call.Rotation,
                call.Origin,
                call.Scale,
                call.Effects,
                layerDepth
            );
        }

        /// <summary>
        /// Flush all queued draws to the SpriteBatch.
        /// </summary>
        public void Flush()
        {
            foreach (DrawLayer layer in Enum.GetValues(typeof(DrawLayer)))
            {
                var spriteQueue = _queues[layer];
                var textQueue = _textQueues[layer];

                if (spriteQueue.Count == 0 && textQueue.Count == 0)
                    continue;

                var combinedGroups = spriteQueue
                    .Select(s => (IsText: false, Sprite: s, Text: default(TextDrawCall)))
                    .Concat(textQueue.Select(t => (IsText: true, Sprite: default(DrawCall), Text: t)))
                    .GroupBy(x => x.IsText ? x.Text.SpriteBatchConfig : x.Sprite.SpriteBatchConfig);

                foreach (var group in combinedGroups)
                {
                    if (!_spriteBatches.TryGetValue(group.Key, out var sb))
                        sb = _spriteBatches[group.Key] = new SpriteBatch(_spriteBatch.GraphicsDevice);

                    Matrix transform = layer != DrawLayer.UI && group.Any(x => x.IsText ? x.Text.UseCamera : x.Sprite.UseCamera)
                        ? _camera
                        : Matrix.Identity;

                    var cfg = group.Key;

                    sb.Begin(
                        cfg.SortMode,
                        cfg.BlendState,
                        cfg.SamplerState,
                        cfg.DepthStencilState,
                        cfg.RasterizerState,
                        cfg.Effect,
                        transform
                    );

                    foreach (var item in group.OrderBy(x => x.IsText ? x.Text.Depth : x.Sprite.Depth))
                    {
                        if (item.IsText)
                        {
                            int minDepth = -100;
                            int maxDepth = 100;
                            float layerDepth = 1.0f - ((item.Text.Depth - minDepth) / (float)(maxDepth - minDepth));

                            sb.DrawString(
                                item.Text.Font,
                                item.Text.Text,
                                item.Text.Position,
                                item.Text.Color,
                                item.Text.Rotation,
                                item.Text.Origin,
                                item.Text.Scale,
                                item.Text.Effects,
                                layerDepth
                            );
                        }
                        else
                        {
                            DrawInternal(sb, item.Sprite);
                        }
                    }

                    sb.End();
                }

                spriteQueue.Clear();
                textQueue.Clear();
            }

        }
    }
}
