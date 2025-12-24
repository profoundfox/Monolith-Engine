using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Nodes;
using Monolith.Util;

namespace Monolith.Graphics
{

    public record class ParallaxLayerConfig : SpatialNodeConfig
    {
        public MTexture Texture { get; set; }
        public Vector2 ParallaxFactor { get; set; } = Vector2.One;
    }
    /// <summary>
    /// Represents a single infinite scrolling parallax layer.
    /// </summary>
    public class ParallaxLayer : Node2D
    {
        public MTexture Texture { get; set; }
        public Vector2 ParallaxFactor { get; set; }

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
        }

        /// <summary>
        /// Updates the parallax offset based on the camera position.
        /// </summary>
        /// <param name="cameraPosition">Camera world position.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            offset = MCamera.CurrentCameraInstance.Position * ParallaxFactor;

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

            int tilesX = Engine.Instance.ScreenSize.X / Texture.Width + 2;
            int tilesY = Engine.Instance.ScreenSize.Y / Texture.Height + 2;

            for (int x = 0; x < tilesX; x++)
            {
                for (int y = 0; y < tilesY; y++)
                {
                    Vector2 drawPos = new Vector2(
                        x * Texture.Width - offset.X,
                        y * Texture.Height - offset.Y
                    );
                    
                    Texture.Draw(drawPos, Color.White, 0f, Vector2.Zero, null, SpriteEffects.None, null, 1.0f);

                }
            }
        }
    }
}
