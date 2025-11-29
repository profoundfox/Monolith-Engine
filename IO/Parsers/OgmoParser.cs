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
using Monolith.Region;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

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
                    Sprite sprite = new Sprite(texture.CreateSubTexture(new Microsoft.Xna.Framework.Rectangle(0, 0, texture.Width, texture.Height)));

                    Sprite2D sprite2D = new Sprite2D(new Node2DConfig
                    {
                        Parent = null,
                        Name = $"Decal: {texturePathWithoutExtension}",
                        Position = new Microsoft.Xna.Framework.Vector2(decal.x, decal.y)
                    });

                    sprite2D.Texture = new MTexture(texturePathWithoutExtension);
                }
            }
        }

        /// <summary>
        /// Updated SearchForObjects supporting direct property mapping and [Ogmo] attributes
        /// </summary>
        public static void SearchForObjects(string filename)
        {
            var root = LoadJson(filename);

            foreach (var entity in root.layers
                .Where(l => l.entities != null)
                .SelectMany(l => l.entities))
            {
                Type nodeType = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a =>
                    {
                        try { return a.GetTypes(); }
                        catch (ReflectionTypeLoadException ex) { return ex.Types.Where(t => t != null); }
                    })
                    .FirstOrDefault(t => t.IsSubclassOf(typeof(Node2D)) && t.Name == entity.name);

                if (nodeType == null)
                {
                    Console.WriteLine($"Type '{entity.name}' not found or not a subclass of Node.");
                    continue;
                }

                Node2D node = (Node2D)Activator.CreateInstance(nodeType, new Node2DConfig
                {
                    Parent = null,
                    Shape = new RectangleShape2D(entity.x, entity.y, entity.width, entity.height),
                    Name = entity.name
                })!;

                if (entity.values != null)
                {
                    var dict = ParseValues(entity.values);

                    foreach (var prop in nodeType.GetProperties().Where(p => p.CanWrite))
                    {
                        var attr = prop.GetCustomAttribute<OgmoAttribute>();
                        string key = attr?.Key ?? prop.Name;

                        if (dict.TryGetValue(key, out var raw))
                        {
                            object val = Convert.ChangeType(raw, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                            prop.SetValue(node, val);
                        }
                    }
                }
            }
        }

        public static void LoadTilemap(ContentManager content, string filename, string textureName, string region)
        {
            var root = LoadJson(filename);

            var tileLayer = root.layers.FirstOrDefault(l => l.data != null && l.tileset != null)
                ?? throw new Exception("No tile layer found in JSON.");

            var split = region.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
            int x = split[0], y = split[1], width = split[2], height = split[3];

            foreach (var layer in root.layers.Where(l => l.tileset != null))
            {
                string contPath = textureName;
                MTexture texture = new(contPath);

                var textureRegion = texture.CreateSubTexture(new Microsoft.Xna.Framework.Rectangle(x, y, width, height));
                var tileset = new Tileset(textureRegion, layer.gridCellWidth, layer.gridCellHeight);

                var tilemap = new Tilemap(tileset, layer.gridCellsX, layer.gridCellsY, 0.1f);
                for (int row = 0; row < layer.gridCellsY; row++)
                {
                    for (int col = 0; col < layer.gridCellsX; col++)
                    {
                        int index = row * layer.gridCellsX + col;
                        int tile = (index >= 0 && index < layer.data.Count) ? layer.data[index] : 0;
                        tilemap.SetTile(col, row, tile);
                    }
                }

                tilemap.LayerDepth = float.Parse(layer.name, CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Instantiates all entities, decals, and tilemaps if parameters are provided
        /// </summary>
        public static void FromFile(
            string filename,
            string defaultTileTexture = null,
            string defaultTileRegion = null)
        {
            SearchForObjects(filename);
            SearchForDecals(filename);

            if (!string.IsNullOrEmpty(defaultTileTexture) && !string.IsNullOrEmpty(defaultTileRegion))
            {
                LoadTilemap(Engine.Content, filename, defaultTileTexture, defaultTileRegion);
            }
        }
    }
}
