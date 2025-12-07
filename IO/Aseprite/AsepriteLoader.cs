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
        public static Dictionary<string, List<MTexture>> LoadAnimations(
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

            Dictionary<string, List<MTexture>> animations = new();

            foreach (var tag in data.meta.frameTags)
            {
                List<MTexture> anim = new();

                for (int i = tag.from; i <= tag.to; i++)
                {
                    var f = frames[i].frame;

                    Rectangle rect = new Rectangle(f.x, f.y, f.w, f.h);
                    anim.Add(source.CreateSubTexture(rect));
                }

                animations[tag.name] = anim;
            }

            return animations;
        }
    }
}