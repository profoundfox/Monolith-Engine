using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Monolith.Graphics;

namespace Monolith.IO
{
    public static class AsepriteLoader
    {
        public static Dictionary<string, Animation> LoadAnimations(
            MTexture source, string jsonPath)
        {
            string json = File.ReadAllText(jsonPath);

            var data = JsonSerializer.Deserialize<AsepriteData>(json);

            List<AsepriteFrame> frames = data.frames
                .OrderBy(f =>
                {
                    var match = Regex.Match(f.Key, @"(\d+)(?=\.)");
                    return match.Success ? int.Parse(match.Value) : 0;
                })
                .Select(f => f.Value)
                .ToList();

            Dictionary<string, Animation> animations = new();

            foreach (var tag in data.meta.frameTags)
            {
                List<MTexture> animFrames = new();
                List<int> durations = new();

                for (int i = tag.from; i <= tag.to; i++)
                {
                    var f = frames[i];

                    Rectangle rect = new Rectangle(
                        f.frame.x, f.frame.y,
                        f.frame.w, f.frame.h);

                    animFrames.Add(source.CreateSubTexture(rect));

                    durations.Add(f.duration);
                }

                int speedMs = (int)durations.Average();

                animations[tag.name] = new Animation
                {
                    Frames = animFrames,
                    Delay = TimeSpan.FromMilliseconds(speedMs)
                };
            }

            return animations;
        }
    }
}
