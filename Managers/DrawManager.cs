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

    public sealed partial class DrawManager
    {
        private readonly Dictionary<SpriteBatchConfig, SpriteBatch> _spriteBatches = new();
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


        private void DrawInternal(SpriteBatch sb, in DrawCall call)
        {
            if (call.Texture == null) return;

            Rectangle src = call.SourceRectangle ?? new Rectangle(0, 0, call.Texture.Width, call.Texture.Height);            

            sb.Draw(
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



        /// <summary>
        /// Flush all queued draws to the SpriteBatch.
        /// </summary>
        public void Flush()
        {
            foreach (DrawLayer layer in Enum.GetValues(typeof(DrawLayer)))
            {
                var queue = _queues[layer];
                if (queue.Count == 0) continue;

                var groups = queue.GroupBy(c => c.SpriteBatchConfig);

                foreach (var group in groups)
                {
                    if (!_spriteBatches.TryGetValue(group.Key, out var sb))
                    {
                        sb = new SpriteBatch(_spriteBatch.GraphicsDevice);
                        _spriteBatches[group.Key] = sb;
                    }

                    Matrix transform =
                        layer != DrawLayer.UI && group.Any(c => c.UseCamera)
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

                    Console.WriteLine(cfg);

                    foreach (var call in group)
                        DrawInternal(sb, call);

                    sb.End();
                }

                queue.Clear();
            }
        }
    }
}
