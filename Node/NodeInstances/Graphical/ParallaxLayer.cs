using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Graphics;
using Monolith.Managers;
using Monolith.Nodes;
using Monolith.Structs;
using Monolith.Util;

namespace Monolith.Nodes
{
    public enum LoopAxis
    {
        None = 0,
        X = 1 << 0,
        Y = 1 << 1,
        Both = X | Y
    }

    public record class ParallaxLayerConfig : SpatialNodeConfig
    {
        public MTexture Texture { get; set; }
        public Vector2 MotionScale { get; set; } = Vector2.One;
        public LoopAxis LoopAxis { get; set; } = LoopAxis.Both;
    }
    /// <summary>
    /// Represents a single infinite scrolling parallax layer.
    /// </summary>
    public class ParallaxLayer : Node2D
    {
        public MTexture Texture { get; set; }
        public Vector2 MotionScale { get; set; }
        public LoopAxis LoopAxes { get; set; }
        private Vector2 offset;

        public ParallaxLayer(ParallaxLayerConfig cfg) : base(cfg)
        {
            Texture = cfg.Texture;
            MotionScale = cfg.MotionScale;
            LoopAxes = cfg.LoopAxis;
        }

        public void ApplyCameraDelta(Vector2 cameraDelta)
        {
            offset += cameraDelta * MotionScale;

            if (LoopAxes.HasFlag(LoopAxis.X))
                offset.X = Mod(offset.X, Texture.Width);

            if (LoopAxes.HasFlag(LoopAxis.Y))
                offset.Y = Mod(offset.Y, Texture.Height);

            if (!LoopAxes.HasFlag(LoopAxis.X))
                offset.X = 0;
            if (!LoopAxes.HasFlag(LoopAxis.Y))
                offset.Y = 0;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var camera = Camera2D.CurrentCameraInstance;
            Rectangle view = camera.GetWorldViewRectangle();

            int texW = Texture.Width;
            int texH = Texture.Height;

            Vector2 basePos = new Vector2(
                LoopAxes.HasFlag(LoopAxis.X)
                    ? GlobalPosition.X - Mod(GlobalPosition.X - offset.X, Texture.Width)
                    : GlobalPosition.X,
                LoopAxes.HasFlag(LoopAxis.Y)
                    ? GlobalPosition.Y - Mod(GlobalPosition.Y - offset.Y, Texture.Height)
                    : GlobalPosition.Y
            );

            int startX = LoopAxes.HasFlag(LoopAxis.X)
                ? (int)Math.Floor((double)view.Left / texW) - 1
                : 0;

            int startY = LoopAxes.HasFlag(LoopAxis.Y)
                ? (int)Math.Floor((double)view.Top / texH) - 1
                : 0;

            int endX = LoopAxes.HasFlag(LoopAxis.X)
                ? (int)Math.Ceiling((double)view.Right / texW) + 1
                : 1;

            int endY = LoopAxes.HasFlag(LoopAxis.Y)
                ? (int)Math.Ceiling((double)view.Bottom / texH) + 1
                : 1;

            for (int y = startY; y < endY; y++)
            {
                for (int x = startX; x < endX; x++)
                {
                    Vector2 pos = new(
                        x * texW + basePos.X,
                        y * texH + basePos.Y
                    );

                    Texture.Draw(pos, Color.White);
                }
            }
        }

        private static float Mod(float x, float m)
        {
            return (x % m + m) % m;
        }
    }

}