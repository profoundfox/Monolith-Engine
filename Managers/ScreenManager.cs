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

    public sealed class ScreenManager
    {
        private readonly Dictionary<SpriteBatchConfig, SpriteBatch> _spriteBatches = new();
        private readonly SpriteBatch _spriteBatch;
        private readonly Dictionary<DrawLayer, List<IDrawCall>> _queues;
        private Matrix _camera = Matrix.Identity;

        public ScreenManager(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch ?? throw new ArgumentNullException(nameof(spriteBatch));

            _queues = new Dictionary<DrawLayer, List<IDrawCall>>();
            foreach (DrawLayer l in Enum.GetValues(typeof(DrawLayer)))
                _queues[l] = new List<IDrawCall>();
        }

        /// <summary>
        /// Updates the camera transform used for non-UI layers.
        /// </summary>
        public void SetCamera(Matrix transform) => _camera = transform;

        /// <summary>
        /// Queue a TextureDrawCall directly.
        /// </summary>
        public void Draw(TextureDrawCall call, DrawLayer layer = DrawLayer.Middleground)
        {
            if (call == null) throw new ArgumentNullException(nameof(call));
            _queues[layer].Add(call);
        }

        /// <summary>
        /// Queue a TextDrawCall.
        /// </summary>
        public void DrawString(TextDrawCall call, DrawLayer layer = DrawLayer.Middleground)
        {
            if (call == null) throw new ArgumentNullException(nameof(call));
            _queues[layer].Add(call);
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

                var groups = queue.GroupBy(x => x.SpriteBatchConfig);

                foreach (var group in groups)
                {
                    if (!_spriteBatches.TryGetValue(group.Key, out var sb))
                        sb = _spriteBatches[group.Key] = new SpriteBatch(_spriteBatch.GraphicsDevice);

                    Matrix transform = layer != DrawLayer.UI && group.Any(x => x.UseCamera)
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

                    foreach (var call in group.OrderBy(x => x.Depth))
                        call.Draw(sb);

                    sb.End();
                }

                queue.Clear();
            }
        }
    }
}
