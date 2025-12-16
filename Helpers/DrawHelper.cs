using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Graphics;
using Monolith.Managers;
using Monolith.Geometry;
using Monolith.Util;

namespace Monolith.Helpers
{
    public static class DrawHelper
    {
        private static Texture2D _pixel;
        private static Dictionary<int, Texture2D> _circleCache = new();

        private static Texture2D GetPixel(GraphicsDevice device)
        {
            if (_pixel == null)
            {
                _pixel = new Texture2D(device, 1, 1);
                _pixel.SetData(new[] { Color.White });
            }
            return _pixel;
        }

        private static Texture2D GetCircleTexture(GraphicsDevice device, int radius)
        {
            if (_circleCache.TryGetValue(radius, out var cached))
                return cached;

            var texture = new Texture2D(device, radius, radius);
            var data = new Color[radius * radius];

            float diam = radius / 2f;
            float diamsq = diam * diam;

            for (int x = 0; x < radius; x++)
            {
                for (int y = 0; y < radius; y++)
                {
                    int index = x * radius + y;
                    Vector2 pos = new Vector2(x - diam, y - diam);
                    data[index] = pos.LengthSquared() <= diamsq ? Color.White : Color.Transparent;
                }
            }

            texture.SetData(data);
            _circleCache[radius] = texture;
            return texture;
        }

        public static void DrawRegionShape(
            IRegionShape2D regionShape,
            Color color,
            float layerDepth = 0.1f,
            DrawLayer layer = DrawLayer.Middleground)
        {
            if (regionShape == null) return;

            if (regionShape is RectangleShape2D rect)
                DrawRectangle(rect, color, layerDepth, layer);

            if (regionShape is CircleShape2D circle)
                DrawCircle(circle, color, layerDepth, layer);
        }

        public static void DrawRegionShapeHollow(
            IRegionShape2D regionShape,
            Color color,
            int thickness = 2,
            float layerDepth = 0.1f,
            DrawLayer layer = DrawLayer.Middleground)
        {
            if (regionShape == null) return;

            if (regionShape is RectangleShape2D rect)
                DrawRectangleHollow(rect, color, thickness, layerDepth, layer);

            if (regionShape is CircleShape2D circle)
                DrawCircleHollow(circle, color, thickness, layerDepth, layer);
        }

        public static void DrawRectangle(
            RectangleShape2D rect,
            Color color,
            float layerDepth = 0.1f,
            DrawLayer layer = DrawLayer.Middleground)
        {
            var pixel = new MTexture(GetPixel(Engine.GraphicsDevice));

            Engine.DrawManager.Draw(
                new DrawParams(
                    texture: pixel,
                    position: new Vector2(rect.X, rect.Y))
                {
                    Color = color,
                    Scale = new Vector2(rect.BoundingBox.Width, rect.BoundingBox.Height),
                    LayerDepth = layerDepth
                },
                layer
            );
        }

        public static void DrawRectangleHollow(
            RectangleShape2D rect,
            Color color,
            int thickness = 1,
            float layerDepth = 0.1f,
            DrawLayer layer = DrawLayer.Middleground)
        {
            var pixel = new MTexture(GetPixel(Engine.GraphicsDevice));

            Engine.DrawManager.Draw(
                new DrawParams(pixel, new Vector2(rect.X, rect.Y))
                {
                    Color = color,
                    Scale = new Vector2(rect.BoundingBox.Width, thickness),
                    LayerDepth = layerDepth
                },
                layer
            );

            Engine.DrawManager.Draw(
                new DrawParams(pixel, new Vector2(rect.X, rect.BoundingBox.Bottom - thickness))
                {
                    Color = color,
                    Scale = new Vector2(rect.BoundingBox.Width, thickness),
                    LayerDepth = layerDepth
                },
                layer
            );

            Engine.DrawManager.Draw(
                new DrawParams(pixel, new Vector2(rect.X, rect.Y))
                {
                    Color = color,
                    Scale = new Vector2(thickness, rect.BoundingBox.Height),
                    LayerDepth = layerDepth
                },
                layer
            );

            Engine.DrawManager.Draw(
                new DrawParams(pixel, new Vector2(rect.BoundingBox.Right - thickness, rect.Y))
                {
                    Color = color,
                    Scale = new Vector2(thickness, rect.BoundingBox.Height),
                    LayerDepth = layerDepth
                },
                layer
            );
        }

        public static void DrawCircle(
            CircleShape2D circ,
            Color color,
            float layerDepth = 0.1f,
            DrawLayer layer = DrawLayer.Middleground)
        {
            if (circ == null) return;

            MTexture texture = new MTexture(GetCircleTexture(Engine.GraphicsDevice, circ.Radius));

            Engine.DrawManager.Draw(
                new DrawParams(
                    texture: texture,
                    position: new Vector2(circ.Location.X, circ.Location.Y))
                {
                    Color = color,
                    Origin = new Vector2(texture.Width / 2f, texture.Height / 2f),
                    LayerDepth = layerDepth
                },
                layer
            );
        }

        public static void DrawCircleHollow(
            CircleShape2D circle,
            Color color,
            int thickness = 1,
            float layerDepth = 0.1f,
            DrawLayer layer = DrawLayer.Middleground)
        {
            if (circle == null) return;

            MTexture pixel = new MTexture(GetPixel(Engine.GraphicsDevice));

            Vector2 center = new Vector2(circle.Location.X, circle.Location.Y);
            float radius = circle.Radius;

            const int segments = 64;
            float increment = MathF.Tau / segments;
            float angle = 0f;

            Vector2 prev = center + new Vector2(MathF.Cos(0f), MathF.Sin(0f)) * radius;

            for (int i = 1; i <= segments; i++)
            {
                angle += increment;
                Vector2 next = center + new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * radius;

                Vector2 edge = next - prev;
                float length = edge.Length();
                float rotation = MathF.Atan2(edge.Y, edge.X);

                Engine.DrawManager.Draw(
                    new DrawParams(pixel, prev)
                    {
                        Color = color,
                        Rotation = rotation,
                        Scale = new Vector2(length, thickness),
                        LayerDepth = layerDepth
                    },
                    layer
                );

                prev = next;
            }
        }

        public static void DrawRay(
            RayCast2D ray,
            Color color,
            float thickness = 1f,
            float layerDepth = 0.1f,
            DrawLayer layer = DrawLayer.Middleground)
        {
            var pixel = new MTexture(GetPixel(Engine.GraphicsDevice));

            Color drawColor = ray.HasHit
                ? ColorHelper.GetOppositeColor(color)
                : color;

            Vector2 end = ray.Position + ray.Direction * ray.Length;
            Vector2 edge = end - ray.Position;
            float angle = (float)Math.Atan2(edge.Y, edge.X);

            Engine.DrawManager.Draw(
                new DrawParams(pixel, ray.Position)
                {
                    Color = drawColor,
                    Rotation = angle,
                    Scale = new Vector2(edge.Length(), thickness),
                    LayerDepth = layerDepth
                },
                layer
            );
        }

        public static void DrawString(string input, Color color, Vector2 pos, float layerDepth = 0.9f)
        {
            Engine.Instance.SpriteBatch.Begin();
            Engine.Instance.SpriteBatch.DrawString(Engine.Instance.Font, input, pos, color);
            Engine.Instance.SpriteBatch.End();
        }
    }
}
