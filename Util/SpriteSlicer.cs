using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monolith.Graphics;

public class SpriteSlicer
{
    /// <summary>
    /// Slices a MTexture into a collection of textures.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="frameWidth"></param>
    /// <param name="frameHeight"></param>
    /// <returns></returns>
    public static List<MTexture> Slice(
        MTexture source,
        int frameWidth,
        int frameHeight,
        int startCol,
        int endCol,
        int startRow,
        int endRow)
    {
        List<MTexture> frames = new();

        for (int y = startRow; y <= endRow; y++)
        {
            for (int x = startCol; x <= endCol; x++)
            {
                Rectangle region = new(
                    x * frameWidth,
                    y * frameHeight,
                    frameWidth,
                    frameHeight);

                frames.Add(source.CreateSubTexture(region));
            }
        }

        return frames;
    }

}