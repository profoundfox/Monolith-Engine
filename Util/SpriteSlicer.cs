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
    public static List<MTexture> SliceGrid(MTexture source, int frameWidth, int frameHeight)
    {
        List<MTexture> frames = new();
        int cols = source.Width / frameWidth;
        int rows = source.Height / frameHeight;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
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

    public static List<MTexture> PickFrames(List<MTexture> frames, params int[] indexes)
    {
        List<MTexture> result = new();

        foreach (int i in indexes)
            result.Add(frames[i]);

        return result;
    }



}