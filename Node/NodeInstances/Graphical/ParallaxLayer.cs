using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Managers;
using Monolith.Nodes;
using Monolith.Util;
using RenderingLibrary.Graphics;

namespace Monolith.Graphics
{

    public record class ParallaxLayerConfig : SpatialNodeConfig
    {
        public MTexture Texture { get; set; }
        public Vector2 ParallaxFactor { get; set; } = Vector2.One;
        public int LoopTimes { get; set; } = 1;
        public float LayerDepth { get; set; } = 1f;
    }
    /// <summary>
    /// Represents a single infinite scrolling parallax layer.
    /// </summary>
    public class ParallaxLayer : Node2D
    {
        public MTexture Texture { get; set; }
        public Vector2 ParallaxFactor { get; set; }
        public int LoopTimes { get; set; }
        public float LayerDepth { get; set; }

        private Vector2 offset = Vector2.Zero;

        /// <summary>
        /// Creates a parallax layer with a texture and parallax factor.
        /// </summary>
        /// <param name="texture">Tileable texture.</param>
        /// <param name="parallaxFactor">How fast the layer moves relative to camera. (0 = static, 1 = full speed)</param>
        public ParallaxLayer(ParallaxLayerConfig cfg) : base(cfg)
        {
            Texture = cfg.Texture ?? throw new ArgumentNullException(nameof(cfg.Texture));
            ParallaxFactor = cfg.ParallaxFactor;
            LoopTimes = cfg.LoopTimes;
            LayerDepth = cfg.LayerDepth;
        }

        /// <summary>
        /// Updates the parallax offset based on the camera position.
        /// </summary>
        /// <param name="cameraPosition">Camera world position.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            offset = Camera2D.CurrentCameraInstance.GlobalPosition * ParallaxFactor;

            offset.X %= Texture.Width;
            offset.Y %= Texture.Height;

            if (offset.X < 0) offset.X += Texture.Width;
            if (offset.Y < 0) offset.Y += Texture.Height;
        }

        /// <summary>
        /// Draws the parallax layer to fill the screen.
        /// </summary>
        /// <param name="screenSize">Size of the viewport / screen in pixels.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            var camera = Camera2D.CurrentCameraInstance;
            Rectangle view = camera.GetWorldViewRectangle();

            int texW = Texture.Width;
            int texH = Texture.Height;

            Vector2 parallaxPos = camera.GlobalPosition * ParallaxFactor;

            int startX = (int)Math.Floor((view.Left + parallaxPos.X) / texW) - 1;
            int startY = (int)Math.Floor((view.Top + parallaxPos.Y) / texH) - 1;

            int endX = (int)Math.Ceiling((view.Right + parallaxPos.X) / texW) + 1;
            int endY = (int)Math.Ceiling((view.Bottom + parallaxPos.Y) / texH) + 1;

            for (int y = startY; y < endY; y++)
            {
                for (int x = startX; x < endX; x++)
                {
                    Vector2 worldPos = new Vector2(
                        x * texW - parallaxPos.X,
                        y * texH - parallaxPos.Y
                    );

                    Texture.Draw(worldPos, Color.White, layerDepth: LayerDepth);
                }
            }
        }
    }
}
