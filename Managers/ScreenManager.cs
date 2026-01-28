using Monolith.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Monolith.Attributes;

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
        private Matrix _matrix = Matrix.Identity;

        internal Point RenderSize { get; set; } = new Point(640, 360);
        internal bool IntScaling { get; set; } = true;
        internal Rectangle Destination { get; set; }

        public Matrix Matrix { get => _matrix; }

        public RenderTarget2D RenderTarget { get; internal set; }

        public ScreenManager(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch ?? throw new ArgumentNullException(nameof(spriteBatch));

            _queues = new Dictionary<DrawLayer, List<IDrawCall>>();
            foreach (DrawLayer l in Enum.GetValues(typeof(DrawLayer)))
                _queues[l] = new List<IDrawCall>();
        }

        /// <summary>
        /// Initializes the screen, creates a render target.
        /// </summary>
        public void Initialize()
        {
            CreateRenderTarget();
            Engine.Instance.Window.ClientSizeChanged += (_, _) => UpdateTransform();
            UpdateTransform();
        }

        /// <summary>
        /// Updates the camera transform used for non-UI layers.
        /// </summary>
        public void SetMatrix(Matrix transform) => _matrix = transform;
        

        /// <summary>
        /// Queues a call directly.
        /// </summary>
        public void Call(IDrawCall call, DrawLayer layer = DrawLayer.Middleground)
        {
            if (call == null) throw new ArgumentNullException(nameof(call));
            _queues[layer].Add(call);
        }

        /// <summary>
        /// Returns the rectangle of world space currently visible by this camera
        /// </summary>
        public Rectangle GetWorldViewRectangle()
        {

            Matrix inverse = Matrix.Invert(_matrix);

            Vector2 topLeft = Vector2.Transform(Vector2.Zero, inverse);
            Vector2 bottomRight = Vector2.Transform(
                new Vector2(RenderTarget.Width, RenderTarget.Height),
                inverse
            );

            return new Rectangle(
                (int)topLeft.X,
                (int)topLeft.Y,
                (int)(bottomRight.X - topLeft.X),
                (int)(bottomRight.Y - topLeft.Y)
            );
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
                        ? _matrix
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

        internal void CreateRenderTarget()
        {
            RenderTarget?.Dispose();

            RenderTarget = new RenderTarget2D(
                Engine.GraphicsDevice,
                RenderSize.X,
                RenderSize.Y,
                false,
                SurfaceFormat.Color,
                DepthFormat.None);
        }

        internal void UpdateTransform()
        {
            var pp = Engine.GraphicsDevice.PresentationParameters;

            float scale = Math.Min(
                pp.BackBufferWidth / (float)RenderSize.X,
                pp.BackBufferHeight / (float)RenderSize.Y);

            if (IntScaling)
                scale = Math.Max(1, MathF.Floor(scale));

            int w = (int)(RenderSize.X * scale);
            int h = (int)(RenderSize.Y * scale);

            int x = (pp.BackBufferWidth - w) / 2;
            int y = (pp.BackBufferHeight - h) / 2;

            Destination = new Rectangle(x, y, w, h);
        }

    }
}
