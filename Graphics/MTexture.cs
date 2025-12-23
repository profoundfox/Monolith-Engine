using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monolith.Graphics
{
    /// <summary>
    /// Represents a texture or a rectangular region (sprite) of a texture.
    /// Can be created from scratch or as a subregion of an existing texture.
    /// </summary>
    public class MTexture : IDisposable
    {
        /// <summary>
        /// Internal texture used for rendering.
        /// </summary>
        public Texture2D Texture { get; private set; }

        /// <summary>
        /// Optional sub-region of the texture to render.
        /// If null, the entire texture is used.
        /// </summary>
        public Rectangle? SourceRectangle { get; private set; }

        /// <summary>
        /// World or screen position where the texture is drawn.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Tint color applied to the texture.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Optional shader effect used when rendering the texture.
        /// </summary>
        public Effect Shader { get; set; }

        /// <summary>
        /// Sprite flipping effects applied during rendering.
        /// </summary>
        public SpriteEffects Effects { get; set; }

        /// <summary>
        /// Rotation of the texture in radians.
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        /// Origin point for rotation and scaling, relative to the texture.
        /// </summary>
        public Vector2 Origin { get; set; }

        /// <summary>
        /// Scale factor applied to the texture.
        /// </summary>
        public Vector2 Scale { get; set; }

        /// <summary>
        /// Depth value used for draw order sorting.
        /// </summary>
        public float LayerDepth { get; set; }

        /// <summary>
        /// Width of the rendered texture or source rectangle.
        /// </summary>
        public int Width => SourceRectangle?.Width ?? Texture.Width;

        /// <summary>
        /// Height of the rendered texture or source rectangle.
        /// </summary>
        public int Height => SourceRectangle?.Height ?? Texture.Height;

        /// <summary>
        /// Size of the rendered texture in pixels.
        /// </summary>
        public Point Size => new(Width, Height);

        /// <summary>
        /// Center point of the rendered texture in local space.
        /// </summary>
        public Vector2 Center => new(Width / 2f, Height / 2f);


        /// <summary>
        /// Creates a blank texture.
        /// </summary>
        public MTexture(int width, int height, bool mipMap = false, SurfaceFormat format = SurfaceFormat.Color)
        {
            Texture = new Texture2D(Engine.GraphicsDevice, width, height, mipMap, format);
            SourceRectangle = null;
        }

        /// <summary>
        /// Creates a texture from a color array.
        /// </summary>
        public MTexture(int width, int height, Color[] data, bool mipMap = false, SurfaceFormat format = SurfaceFormat.Color)
        {
            if (data.Length != width * height)
                throw new ArgumentException("Color array length does not match width*height");

            Texture = new Texture2D(Engine.GraphicsDevice, width, height, mipMap, format);
            Texture.SetData(data);
            SourceRectangle = null;
        }

        /// <summary>
        /// Creates an MTexture from an existing Texture2D.
        /// </summary>
        public MTexture(Texture2D texture, Rectangle? region = null)
        {
            Texture = texture ?? throw new ArgumentNullException(nameof(texture));
            SourceRectangle = region;
        }

        /// <summary>
        /// Creates an MTexture from a file.
        /// </summary>
        /// <param name="texturePath"></param>
        /// <param name="region"></param>
        /// <exception cref="ArgumentNullException"></exception>

        public MTexture(string texturePath, Rectangle? region = null)
        {
            Texture2D texture = Engine.Instance.ContentProvider.Load<Texture2D>(texturePath);
            Texture = texture ?? throw new ArgumentNullException(nameof(texture));
            SourceRectangle = region;
        }

        /// <summary>
        /// Creates a subtexture of this MTexture.
        /// </summary>
        public MTexture CreateSubTexture(Rectangle region)
        {
            if (SourceRectangle.HasValue)
            {
                Rectangle parent = SourceRectangle.Value;
                region.Offset(parent.X, parent.Y);
            }
            return new MTexture(Texture, region);
        }

        public void Draw()
        {
            Engine.DrawManager.Draw(new Managers.DrawParams(
                texture: this,
                position: Position,
                source: SourceRectangle,
                color: Color,
                rotation: Rotation,
                origin: Origin,
                scale: Scale,
                effects: Effects,
                effect: Shader,
                layerDepth: LayerDepth
            ));
        }

        public void Draw(Vector2 position)
        {
            Engine.DrawManager.Draw(new Managers.DrawParams(
                texture: this,
                position: position,
                source: SourceRectangle,
                color: Color,
                rotation: Rotation,
                origin: Origin,
                scale: Scale,
                effects: Effects,
                effect: Shader,
                layerDepth: LayerDepth
            ));
        }

        /// <summary>
        /// Draws the texture with given parameters.
        /// </summary>
        public void Draw(Vector2 position, Color color, float rotation = 0f, Vector2 origin = default, Vector2? scale = null, SpriteEffects effects = SpriteEffects.None, Effect shader = null, float layerDepth = 0f)
        {
            Engine.DrawManager.Draw(new Managers.DrawParams
            (
                texture: this,
                position: position,
                source: SourceRectangle,
                color: color,
                rotation: rotation,
                origin: origin,
                scale: scale ?? Vector2.One,
                effects: effects,
                effect: shader,
                layerDepth: layerDepth
            ));

        }

        /// <summary>
        /// Sets the pixel data of the texture.
        /// </summary>
        public void SetData(Color[] data)
        {
            if (data.Length != Texture.Width * Texture.Height)
                throw new ArgumentException("Data length does not match texture size");
            Texture.SetData(data);
        }

        /// <summary>
        /// Gets the pixel data of the texture.
        /// </summary>
        public Color[] GetData()
        {
            Color[] data = new Color[Texture.Width * Texture.Height];
            Texture.GetData(data);
            return data;
        }

        /// <summary>
        /// Dispose the underlying Texture2D.
        /// </summary>
        public void Dispose()
        {
            Texture?.Dispose();
            Texture = null;
        }
    }
}
