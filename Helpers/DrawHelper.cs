using System;
using System.Collections.Generic;
using ConstructEngine.Managers;
using ConstructEngine.Region;
using ConstructEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ConstructEngine.Helpers
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

        public static void DrawRegionShape(IRegionShape2D regionShape, Color color, float layerDepth = 0.1f, DrawLayer layer = DrawLayer.Middleground)
        {
            if (regionShape == null) return;

            if (regionShape is RectangleShape2D rect)
                DrawRectangle(rect, color, layerDepth, layer);
            if (regionShape is CircleShape2D circle)
                DrawCircle(circle, color, layerDepth, layer);
        }

        public static void DrawRegionShapeHollow(IRegionShape2D regionShape, Color color, int thickness = 2, float layerDepth = 0.1f, DrawLayer layer = DrawLayer.Middleground)
        {
            if (regionShape == null) return;

            if (regionShape is RectangleShape2D rect)
                DrawRectangleHollow(rect, color, thickness, layerDepth, layer);
            if (regionShape is CircleShape2D circle)
                DrawCircleHollow(circle, color, thickness, layerDepth, layer);
        }

        public static void DrawCircle(CircleShape2D circ, Color color, float layerDepth = 0.1f, DrawLayer layer = DrawLayer.Middleground)
        {
            if (circ == null) return;

            Texture2D texture = GetCircleTexture(Engine.GraphicsDevice, circ.Radius);

            Engine.DrawManager.Draw(
                texture,
                new Vector2(circ.X, circ.Y),
                color,
                layer,
                0f,
                new Vector2(texture.Width / 2f, texture.Height / 2f),
                Vector2.One,
                SpriteEffects.None,
                layerDepth
            );
        }

        public static void DrawRectangle(RectangleShape2D rect, Color color, float layerDepth = 0.1f, DrawLayer layer = DrawLayer.Middleground)
        {
            var pixel = GetPixel(Engine.GraphicsDevice);

            Engine.DrawManager.Draw(pixel,
                new Vector2(rect.X, rect.Y),
                color, layer, 0f, Vector2.Zero,
                new Vector2(rect.BoundingBox.Width, rect.BoundingBox.Height),
                SpriteEffects.None, layerDepth);
        }

        public static void DrawRectangleHollow(RectangleShape2D rect, Color color, int thickness = 1, float layerDepth = 0.1f, DrawLayer layer = DrawLayer.Middleground)
        {
            var pixel = GetPixel(Engine.GraphicsDevice);

            Engine.DrawManager.Draw(pixel,
                new Vector2(rect.X, rect.Y),
                color, layer, 0f, Vector2.Zero,
                new Vector2(rect.BoundingBox.Width, thickness),
                SpriteEffects.None, layerDepth);

            Engine.DrawManager.Draw(pixel,
                new Vector2(rect.X, rect.BoundingBox.Bottom - thickness),
                color, layer, 0f, Vector2.Zero,
                new Vector2(rect.BoundingBox.Width, thickness),
                SpriteEffects.None, layerDepth);


            Engine.DrawManager.Draw(pixel,
                new Vector2(rect.X, rect.Y),
                color, layer, 0f, Vector2.Zero,
                new Vector2(thickness, rect.BoundingBox.Height),
                SpriteEffects.None, layerDepth);

            Engine.DrawManager.Draw(pixel,
                new Vector2(rect.BoundingBox.Right - thickness, rect.Y),
                color, layer, 0f, Vector2.Zero,
                new Vector2(thickness, rect.BoundingBox.Height),
                SpriteEffects.None, layerDepth);
        }

        public static void DrawCircleHollow(
            CircleShape2D circle,
            Color color,
            int thickness = 1,
            float layerDepth = 0.1f,
            DrawLayer layer = DrawLayer.Middleground)
        {
            if (circle == null) return;

            Texture2D pixel = GetPixel(Engine.GraphicsDevice);

            Vector2 center = new Vector2(circle.X, circle.Y);
            float radius = circle.Radius;

            const int segments = 64; // same as your rectangle: no extra parameter clutter
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
                    pixel,
                    prev,
                    color,
                    layer,
                    rotation,
                    Vector2.Zero,
                    new Vector2(length, thickness),
                    SpriteEffects.None,
                    layerDepth);

                prev = next;
            }
        }

        public static void DrawRay(RayCast2D ray, Color color, float thickness = 1f, float layerDepth = 0.1f, DrawLayer layer = DrawLayer.Middleground)
        {
            var pixel = GetPixel(Engine.GraphicsDevice);

            Color drawColor = ray.HasHit
                ? ColorHelper.GetOppositeColor(color)
                : color;

            Vector2 end = ray.Position + ray.Direction * ray.Length;
            Vector2 edge = end - ray.Position;
            float angle = (float)Math.Atan2(edge.Y, edge.X);

            Engine.DrawManager.Draw(
                pixel,
                ray.Position,
                drawColor,
                layer,
                angle,
                Vector2.Zero,
                new Vector2(edge.Length(), thickness),
                SpriteEffects.None,
                layerDepth
            );
        }


        public static void DrawString(string input, Color color, Vector2 pos, float layerDepth = 0.9f)
        {
            Engine.SpriteBatch.Begin();
            Engine.SpriteBatch.DrawString(Engine.Font, input, pos, color);
            Engine.SpriteBatch.End();
        }
    }
}
