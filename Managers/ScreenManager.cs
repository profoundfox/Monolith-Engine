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
        /// Queues a call directly.
        /// </summary>
        public void Draw(IDrawCall call, DrawLayer layer = DrawLayer.Middleground)
        {
            if (call == null) throw new ArgumentNullException(nameof(call));
            _queues[layer].Add(call);
        }

        /// <summary>
        /// Flush all queued draws to the SpriteBatch.
        /// </summary>
        public void Flush()
        {
            var groupedQueue = new Dictionary<SpriteBatchConfig, List<IDrawCall>>();

            foreach (DrawLayer layer in Enum.GetValues(typeof(DrawLayer)))
            {
                var queue = _queues[layer];
                if (queue.Count == 0)
                    continue;

                groupedQueue.Clear();

                foreach (var call in queue)
                {
                    if (!groupedQueue.TryGetValue(call.SpriteBatchConfig, out var list))
                    {
                        list = new List<IDrawCall>();
                        groupedQueue[call.SpriteBatchConfig] = list;
                    }
                    list.Add(call);
                }

                foreach (var kvp in groupedQueue)
                {
                    var config = kvp.Key;
                    var calls = kvp.Value;

                    if (calls.Count > 1)
                        calls.Sort((a, b) => a.Depth.CompareTo(b.Depth));

                    Matrix transform = (layer != DrawLayer.UI)
                        ? _camera
                        : Matrix.Identity;

                    if (!_spriteBatches.TryGetValue(config, out var sb))
                        sb = _spriteBatches[config] = new SpriteBatch(_spriteBatch.GraphicsDevice);

                    sb.Begin(
                        config.SortMode,
                        config.BlendState,
                        config.SamplerState,
                        config.DepthStencilState,
                        config.RasterizerState,
                        config.Effect,
                        transform
                    );

                    for (int i = 0; i < calls.Count; i++)
                        calls[i].Draw(sb);

                    sb.End();
                }

                queue.Clear();
            }
        }

    }
}
