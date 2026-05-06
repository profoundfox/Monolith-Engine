using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Geometry;

namespace Monolith.Graphics
{
  /// <summary>
  /// Represents a texture or a rectangular region of a texture.
  /// Can be created from scratch or as a subregion of an existing texture.
  /// </summary>
  public class MTexture : BaseObject, IDisposable
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
    /// The bounds of this texture.
    /// Represented as integer coordiantes.
    /// </summary>
    public Extent Bounds
    {
      get => new Extent
      (
          SourceRectangle?.Width ?? Texture.Width,
          SourceRectangle?.Height ?? Texture.Height
      );
    }

    /// <summary>
    /// Center point of the rendered texture in local space.
    /// </summary>
    public Vector2 Center => new(Bounds.Width / 2f, Bounds.Height / 2f);


    /// <summary>
    /// Creates an MTexture from an existing Texture2D.
    /// </summary>
    public MTexture(Texture2D texture, Rectangle? region = null)
    {
      Texture = texture ?? throw new ArgumentNullException(nameof(texture));
      SourceRectangle = region;
    }

    /// <summary>
    /// Creates a blank texture.
    /// </summary>
    public MTexture(int width, int height, bool mipMap = false, SurfaceFormat format = SurfaceFormat.Color)
    {
      Texture = new Texture2D(Core.GraphicsDevice, width, height, mipMap, format);
      SourceRectangle = null;
    }

    /// <summary>
    /// Creates a texture from a color array.
    /// </summary>
    public MTexture(int width, int height, Color[] data, bool mipMap = false, SurfaceFormat format = SurfaceFormat.Color)
    {
      if (data.Length != width * height)
        throw new ArgumentException("Color array length does not match width*height");

      Texture = new Texture2D(Core.GraphicsDevice, width, height, mipMap, format);
      Texture.SetData(data);
      SourceRectangle = null;
    }

    /// <summary>
    /// Creates an MTexture from a file.
    /// </summary>
    /// <param name="texturePath"></param>
    /// <param name="region"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public MTexture(string texturePath, Rectangle? region = null)
    {
      Texture2D texture = Core.Resource.Load<Texture2D>(texturePath);
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
