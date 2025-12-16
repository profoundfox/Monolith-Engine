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
using Monlith.Nodes;

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
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<OgmoFileInfo.Root>(json, options)!;
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

            foreach (var layer in root.layers.Where(l => !string.IsNullOrEmpty(l.folder) && l.decals != null))
            {
                foreach (var decal in layer.decals)
                {
                    string textureName = Path.GetFileNameWithoutExtension(decal.texture);
                    string texturePathWithoutExtension = Path.Combine(
                        Path.GetDirectoryName(decal.texture) ?? string.Empty,
                        Path.GetFileNameWithoutExtension(decal.texture)
                    );

                    MTexture texture = new(texturePathWithoutExtension);

                    Sprite2D sprite2D = new Sprite2D(new SpriteConfig
                    {
                        Parent = null,
                        Name = $"Decal: {texturePathWithoutExtension}",
                        Position = new Vector2(decal.x, decal.y),
                        Texture = new MTexture(texturePathWithoutExtension)
                    });
                }
            }
        }

        public static void LoadNodes(string fileName)
        {
            var root = LoadJson(fileName);

            foreach (var l in root.layers)
            {
                if (l.entities == null) continue;
                
                foreach (var e in l.entities)
                {
                    Dictionary<string, object> values = new Dictionary<string, object>();

                    if (e.values != null)
                        values = ParseValues(e.values);
                    
                    NodeFactory.CreateNode(e.name, new RectangleShape2D(e.x , e.y, e.width, e.height), values);
                }
            }
        }

        public static void LoadTilemap(
            MTexture texture,
            Rectangle region,
            Rectangle gridRegion,
            IReadOnlyList<int> tileData,
            float layerDepth)
        {
            MTexture textureRegion = texture;

            if (region != default)
                textureRegion = texture.CreateSubTexture(region);

            var tileset = new Tileset(textureRegion, gridRegion.Width, gridRegion.Height);
            var tilemap = new Tilemap(tileset, gridRegion.X, gridRegion.Y, 0.1f);

            for (int row = 0; row < gridRegion.Y; row++)
            {
                for (int col = 0; col < gridRegion.X; col++)
                {
                    int index = row * gridRegion.X + col;
                    int tile = (index >= 0 && index < tileData.Count) ? tileData[index] : 0;
                    tilemap.SetTile(col, row, tile);
                }
            }

            tilemap.LayerDepth = layerDepth;
        }

        public static void LoadTilemapFromJson(
            ContentManager content,
            string filename,
            string texturePath,
            Rectangle region)
        {
            var root = LoadJson(filename);

            var rect = region;

            var layer = root.layers.FirstOrDefault(l => l.data != null && l.tileset != null)
                ?? throw new Exception("No tile layer found in JSON.");

            float layerDepth = float.Parse(layer.name, CultureInfo.InvariantCulture);

            LoadTilemap(
                texture: new MTexture(texturePath),
                region: rect,
                gridRegion: new Rectangle(layer.gridCellsX, layer.gridCellsY, layer.gridCellWidth, layer.gridCellHeight),
                tileData: layer.data,
                layerDepth: layerDepth
            );
        }
        
        /// <summary>
        /// Instantiates all entities, decals, and tilemaps if parameters are provided
        /// </summary>
        public static void FromFile(
            string filename,
            string defaultTileTexture = null,
            Rectangle defaultTileRegion = default)
        {
            LoadNodes(filename);
            SearchForDecals(filename);

            if (!string.IsNullOrEmpty(defaultTileTexture))
            {
                LoadTilemapFromJson(Engine.ContentManager, filename, defaultTileTexture, defaultTileRegion);
            }
        }
    }
}
