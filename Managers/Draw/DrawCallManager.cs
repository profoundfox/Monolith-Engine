using Monolith.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monolith.Managers
{
    /// <summary>
    /// The manager responsible for batching, organizing, and drawing queued sprites
    /// across multiple draw layers in the correct order.
    /// </summary>
    public partial class DrawManager
    {
        public void Draw(
            Texture2D texture,
            Vector2 position,
            Color color,
            DrawLayer layer = DrawLayer.Middleground,
            float rotation = 0f,
            Vector2? origin = null,
            Vector2? scale = null,
            SpriteEffects effects = SpriteEffects.None,
            float layerDepth = 0f,
            Effect effect = null,
            Rectangle? sourceRect = null
        )
        {
            Queue(new DrawCall(true)
            {
                Texture = texture,
                Position = position,
                Color = color,
                Rotation = rotation,
                Origin = origin ?? Vector2.Zero,
                Scale = scale ?? Vector2.One,
                Effects = effects,
                LayerDepth = layerDepth,
                Effect = effect,
                SourceRectangle = sourceRect
            }, layer);
        }
        public void Draw(Sprite sprite, DrawLayer layer = DrawLayer.Middleground)
        {
            Queue(new DrawCall(true)
            {
                Texture = sprite.Region.Texture,
                Position = sprite.Position,
                SourceRectangle = sprite.Region.SourceRectangle,
                Color = sprite.Color,
                Rotation = sprite.Rotation,
                Origin = sprite.Origin,
                Scale = sprite.Scale,
                Effects = sprite.Effects,
                LayerDepth = sprite.LayerDepth,
                Effect = null
            }, layer);
        }

        public void Draw(
            TextureRegion region,
            Vector2 position,
            Color color,
            DrawLayer layer = DrawLayer.Middleground,
            float rotation = 0f,
            Vector2? origin = null,
            Vector2? scale = null,
            SpriteEffects effects = SpriteEffects.None,
            float layerDepth = 0f,
            Effect effect = null
        )
        {
            Queue(new DrawCall(true)
            {
                Texture = region.Texture,
                Position = position,
                Color = color,
                Rotation = rotation,
                Origin = origin ?? Vector2.Zero,
                Scale = scale ?? Vector2.One,
                Effects = effects,
                LayerDepth = layerDepth,
                Effect = effect,
                SourceRectangle = region.SourceRectangle
            }, layer);
        }

        /// <summary>
        /// Queues a looping background automatically, tiled across the screen with an offset.
        /// </summary>
        /// <param name="texture">The texture to loop.</param>
        /// <param name="position">Base position offset on screen.</param>
        /// <param name="offset">Scrolling offset (e.g., time * speed).</param>
        /// <param name="layer">The draw layer to queue the background in.</param>
        /// <param name="color">Optional tint color.</param>
        /// <param name="layerDepth">Layer depth for sorting.</param>
        public void DrawLooping(Sprite sprite, Vector2 position, Vector2 offset, DrawLayer layer = DrawLayer.Middleground, Color? color = null, float layerDepth = 0.0f)
        {
            Color c = color ?? Color.White;
            int screenWidth = _spriteBatch.GraphicsDevice.Viewport.Width;
            int screenHeight = _spriteBatch.GraphicsDevice.Viewport.Height;

            int tilesX = (screenWidth / (int)sprite.Width) + 2;
            int tilesY = (screenHeight / (int)sprite.Height) + 2;

            float offsetX = offset.X % (int)sprite.Width;
            float offsetY = offset.Y % (int)sprite.Height;

            for (int x = 0; x < tilesX; x++)
            {
                for (int y = 0; y < tilesY; y++)
                {
                    Queue(new DrawCall(true)
                    {
                        Texture = sprite.Region.Texture,
                        Position = new Vector2(x * (int)sprite.Width - offsetX + position.X, y * (int)sprite.Height - offsetY + position.Y),
                        Color = c,
                        Scale = Vector2.One,
                        LayerDepth = layerDepth,
                        LoopX = true,
                        LoopY = true,
                        Offset = offset
                    }, layer);
                }
            }
        }

        public void DrawLooping(Texture2D texture, Vector2 position, Vector2 offset, DrawLayer layer = DrawLayer.Middleground, Color? color = null, float layerDepth = 0.0f)
        {
            Color c = color ?? Color.White;
            int screenWidth = _spriteBatch.GraphicsDevice.Viewport.Width;
            int screenHeight = _spriteBatch.GraphicsDevice.Viewport.Height;

            int tilesX = (screenWidth / texture.Width) + 2;
            int tilesY = (screenHeight / texture.Height) + 2;

            float offsetX = offset.X % texture.Width;
            float offsetY = offset.Y % texture.Height;

            for (int x = 0; x < tilesX; x++)
            {
                for (int y = 0; y < tilesY; y++)
                {
                    Queue(new DrawCall(true)
                    {
                        Texture = texture,
                        Position = new Vector2(x * (int)texture.Width - offsetX + position.X, y * texture.Height - offsetY + position.Y),
                        Color = c,
                        Scale = Vector2.One,
                        LayerDepth = layerDepth,
                        LoopX = true,
                        LoopY = true,
                        UseCamera = false,
                        Offset = offset
                    }, layer);
                }
            }
        }
    }
}