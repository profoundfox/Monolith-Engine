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
        private readonly Dictionary<DrawLayer, List<IDrawCall>> _queues;
        private Matrix _camera = Matrix.Identity;

        public DrawManager(SpriteBatch spriteBatch)
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
        /// Queue a draw using DrawParams.
        /// </summary>
        public void Draw(in TextureDrawParams p, DrawLayer layer = DrawLayer.Middleground)
        {
            var call = new TextureDrawCall
            {
                Texture = p.Texture,
                SourceRectangle = p.SourceRectangle,
                Position = p.Position,
                Color = p.Color,
                Rotation = p.Rotation,
                Origin = p.Origin,
                Scale = p.Scale,
                Effects = p.Effects,
                Depth = p.Depth,
                Effect = p.Effect,
                UseCamera = p.UseCamera,
                SpriteBatchConfig = p.SpriteBatchConfig
            };
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

            _queues[layer].Add(
                new TextDrawCall
                {
                    Font = Engine.Instance.Font,
                    Text = text,
                    Position = position,
                    Color = color,
                    Depth = depth,
                    UseCamera = useCamera,
                    SpriteBatchConfig = cfg,
                    Origin = Vector2.Zero,
                    Scale = Vector2.One,
                    Effects = SpriteEffects.None,
                    Rotation = 0f
                }
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

                var groups = queue.GroupBy(x => x.SpriteBatchConfig);

                foreach (var group in groups)
                {
                    if (!_spriteBatches.TryGetValue(group.Key, out var sb))
                        sb = _spriteBatches[group.Key] = new SpriteBatch(_spriteBatch.GraphicsDevice);
                    
                    Matrix transform = layer != DrawLayer.UI && group.Any(x => x.UseCamera)
                        ? _camera
                        : Matrix.Identity;
                    
                    var cfg = group.Key;

                    sb.Begin(cfg.SortMode, cfg.BlendState, cfg.SamplerState,
                        cfg.DepthStencilState, cfg.RasterizerState, cfg.Effect, transform);
                    
                    foreach (var call in group.OrderBy(x => x.Depth))
                        call.Draw(sb);
                    
                    sb.End();
                }

                queue.Clear();
            }

        }
    }
}
