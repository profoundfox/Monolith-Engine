using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Graphics;

namespace Monolith.Tools
{
  public static class GraphicsE
  {
    public static MTexture LoadBase64Premultiplied(this string base64String)
    {
      if (base64String.Contains(",")) base64String = base64String.Split(',')[1];
      byte[] imageBytes = Convert.FromBase64String(base64String);
      
      Texture2D texture;

      using (MemoryStream ms = new MemoryStream(imageBytes))
      {
        texture = Texture2D.FromStream(Engine.GraphicsDevice, ms);
      }

      Color[] pixels = new Color[texture.Width * texture.Height];
      texture.GetData(pixels);

      for (int i = 0; i < pixels.Length; i++)
      {
        Color p = pixels[i];
        float alpha = p.A / 255f;
        
        pixels[i] = new Color(
            (byte)(p.R * alpha),
            (byte)(p.G * alpha),
            (byte)(p.B * alpha),
            p.A
        );
      }

      texture.SetData(pixels);

      return texture.ToMTexture();
    }

    public static MTexture ToMTexture(this Texture2D texture)
    {
      return new MTexture(texture);
    }

    public static Texture2D ToTexture(this MTexture texture)
    {
      return texture.Texture;
    }
  }
}
