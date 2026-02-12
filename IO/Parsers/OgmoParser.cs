using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Monolith.Graphics;
using Monolith.Managers;
using Monolith.Nodes;
using Monolith.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Monolith.Helpers;

namespace Monolith.IO
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OgmoAttribute : Attribute
    {
        public string Key { get; }
        public OgmoAttribute(string key) => Key = key;
    }

    public static class OgmoParser
    {
        private static OgmoFileInfo.Root LoadJson(string filename)
        {
            string json = File.ReadAllText(filename);

            return JsonSerializer.Deserialize<OgmoFileInfo.Root>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            )!;
        }

        private static Dictionary<string, object> ParseValues(Dictionary<string, JsonElement> values)
        {
            return values.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.ValueKind switch
                {
                    JsonValueKind.String => (object)kvp.Value.GetString()!,
                    JsonValueKind.Number => kvp.Value.TryGetInt64(out long l) ? l : kvp.Value.GetDouble(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Null => null!,
                    JsonValueKind.Object => JsonSerializer.Deserialize<Dictionary<string, object>>(kvp.Value.GetRawText())!,
                    JsonValueKind.Array => JsonSerializer.Deserialize<List<object>>(kvp.Value.GetRawText())!,
                    _ => kvp.Value.GetRawText()
                }
            );
        }

        public static void SearchForDecals(string filename)
        {
            var root = LoadJson(filename);

            foreach (var layer in root.Layers.Where(l => !string.IsNullOrEmpty(l.Folder) && l.Decals != null))
            {
                foreach (var decal in layer.Decals)
                {
                    string textureName = Path.GetFileNameWithoutExtension(decal.Texture);
                    string texturePathWithoutExtension = Path.Combine(
                        Path.GetDirectoryName(decal.Texture) ?? string.Empty,
                        Path.GetFileNameWithoutExtension(decal.Texture)
                    );

                    MTexture texture = new(texturePathWithoutExtension);

                    Sprite2D sprite2D = new Sprite2D(new SpriteConfig
                    {
                        Parent = null,
                        Name = $"Decal: {texturePathWithoutExtension}",
                        LocalPosition = new Vector2(decal.X, decal.X),
                        Texture = new MTexture(texturePathWithoutExtension)
                    });
                }
            }
        }

        public static List<Node> LoadNodes(string fileName)
        {
            var root = LoadJson(fileName);

            List<Node> nodes = new();

            foreach (var l in root.Layers)
            {
                if (l.Entities == null) continue;
                
                foreach (var e in l.Entities)
                {
                    Dictionary<string, object> values = new Dictionary<string, object>();

                    if (e.Values != null)
                        values = ParseValues(e.Values);
                    
                    var m = NodeFactory.CreateNode(e.Name, new RectangleShape2D(e.X , e.Y, e.Width, e.Height), values);
                    nodes.Add(m);

                    if (e.Nodes == null)
                        continue;
                    
                    foreach (var n in e.Nodes)
                    {
                        var nm  = NodeFactory.CreateNode(e.Name, new RectangleShape2D(n.X , n.Y, e.Width, e.Height), values);
                        nodes.Add(nm);
                    }
                }
            }
            
            return nodes;
            
        }

        public static Tilemap LoadTilemap(
            MTexture texture,
            Rectangle gridRegion,
            List<int> tileData,
            int depth)
        {
            MTexture textureRegion = texture;

            var tileset = new Tileset(textureRegion, gridRegion.Width, gridRegion.Height);

            var tMap = new Tilemap(new TilemapConfig
            {
                Tileset = tileset,
                Columns = gridRegion.X,
                Rows = gridRegion.Y
            });

            tMap.SetData(gridRegion, tileData);

            tMap.LocalOrdering = tMap.LocalOrdering with { Depth = depth };
            
            return tMap;
        }

        public static List<Tilemap> LoadTilemapFromJson(
            string filename,
            string texturePath)
        {
            var root = LoadJson(filename);
            List<Tilemap> tMaps = new();

            foreach (var l in root.Layers)
            {
                if (l.Tileset == null)
                    continue;
                    
                int depth = int.Parse(l.Name, CultureInfo.InvariantCulture);

                var t = LoadTilemap(
                    texture: new MTexture(texturePath),
                    gridRegion: new Rectangle(l.GridCellsX, l.GridCellsY, l.GridCellWidth, l.GridCellHeight),
                    tileData: l.Data,
                    depth: depth
                );

                tMaps.Add(t);
            }

            return tMaps;
        }
        
        /// <summary>
        /// Instantiates all entities, decals, and tilemaps if parameters are provided
        /// </summary>
        public static void FromFile(
            string filename,
            string defaultTileTexture = null)
        {
            LoadNodes(filename);
            SearchForDecals(filename);

            if (!string.IsNullOrEmpty(defaultTileTexture))
            {
                LoadTilemapFromJson(filename, defaultTileTexture);
            }
        }
    }
}
