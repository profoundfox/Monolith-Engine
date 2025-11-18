using ConstructEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ConstructEngine.Managers
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
            Queue(new DrawCall
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
            Queue(new DrawCall
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
            Queue(new DrawCall
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
    }
}