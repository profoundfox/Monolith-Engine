using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using ConstructEngine.Graphics;
using ConstructEngine.Managers;
using ConstructEngine.Nodes;
using ConstructEngine.Region;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ConstructEngine.Directory
{
    public static class OgmoParser
    {
        /// <summary>
        /// Loads a json file and extracts the OgmoFile information from it.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private static OgmoFileInfo.Root LoadJson(string filename)
        {
            string json = File.ReadAllText(filename);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<OgmoFileInfo.Root>(json, options);
        }
        /// <summary>
        /// Takes in a string and finds a type that shares a name with it, returns that type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="className"></param>
        /// <returns></returns>

        private static Type GetTypeByName<T>(string className)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.Name == className && typeof(T).IsAssignableFrom(t));
        }

        /// <summary>
        /// Parses values from a dictionary with a JsonElement as its key, turns it into a regular object.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private static Dictionary<string, object> ParseValues(Dictionary<string, JsonElement> values)
        {
            return values.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.ValueKind switch
                {
                    JsonValueKind.String => (object)kvp.Value.GetString(),
                    JsonValueKind.Number => kvp.Value.TryGetInt64(out long l) ? l : kvp.Value.GetDouble(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Null => null!,
                    JsonValueKind.Object => JsonSerializer.Deserialize<Dictionary<string, object>>(kvp.Value.GetRawText()),
                    JsonValueKind.Array => JsonSerializer.Deserialize<List<object>>(kvp.Value.GetRawText()),
                    _ => kvp.Value.GetRawText()
                }
            );
        }

        /// <summary>
        /// Searches for images within the level file.
        /// WIP
        /// </summary>
        /// <param name="filename"></param>
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

                    Texture2D texture = Engine.Content.Load<Texture2D>(texturePathWithoutExtension);
                    var atlas = new TextureAtlas(texture);
                    atlas.AddRegion(textureName, 0, 0, texture.Width, texture.Height);

                    var sprite = atlas.CreateSprite(textureName);
                    sprite.CenterOrigin();
                    sprite.Name = textureName;
                    sprite.Position = new Vector2(decal.x, decal.y);

                    Engine.SpriteManager.AddSprite(sprite);
                }
            }
        }

        /// <summary>
        /// Searches for Nodes within the enties layer in the level file, sets the object's values from the file.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="Player"></param>

        public static void SearchForObjects(string filename)
        {
            var root = LoadJson(filename);

            foreach (var entity in root.layers
                .Where(l => l.entities != null)
                .SelectMany(l => l.entities))
            {
                Dictionary<string, object> normalDict = entity.values != null 
                    ? ParseValues(entity.values) 
                    : new Dictionary<string, object>();

                Type type = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a =>
                    {
                        try
                        {
                            return a.GetTypes();
                        }
                        catch (ReflectionTypeLoadException ex)
                        {
                            return ex.Types.Where(t => t != null);
                        }
                    })
                    .FirstOrDefault(t => t.IsSubclassOf(typeof(Node)) && t.Name == entity.name);

                if (type != null)
                {
                    var obj = (Node)Activator.CreateInstance(type);
                    obj.Root = Engine.SceneManager.GetCurrentScene();
                    obj.Shape = new RectangleShape2D(entity.x, entity.y, entity.width, entity.height);
                    obj.Location = new(entity.x, entity.y);
                    obj.Name = entity.name;
                    obj.Values = normalDict;
                }
                else
                {
                    Console.WriteLine($"Type '{entity.name}' not found or not a subclass of node.");
                }
            }
        }

        /// <summary>
        /// Creates a tilemap from the level's data list, uses the texture region system to make it work with atlases. 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="filename"></param>
        /// <param name="textureName"></param>
        /// <param name="region"></param>
        /// <exception cref="Exception"></exception>
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
                Texture2D texture = content.Load<Texture2D>(contPath);
                var textureRegion = new TextureRegion(texture, x, y, width, height);
                var tileset = new Tileset(textureRegion, layer.gridCellWidth, layer.gridCellHeight);

                var tilemap = new Tilemap(tileset, layer.gridCellsX, layer.gridCellsY, 0.1f);
                for (int row = 0; row < layer.gridCellsY; row++)
                {
                    for (int col = 0; col < layer.gridCellsX; col++)
                    {
                        int index = row * layer.gridCellsX + col;
                        int tile = (index >= 0 && index < layer.data.Count) ? layer.data[index] : 0;
                        tilemap.SetTile(col, row, tile == -1 ? 0 : tile);
                    }
                }

                tilemap.LayerDepth = float.Parse(layer.name, CultureInfo.InvariantCulture);
            }
        }
        
        /// <summary>
        /// Instanitates all the entites, loads all the objects, decals and tilemaps if the required paramteres are present.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="Player"></param>
        /// <param name="content"></param>
        /// <param name="defaultTileTexture"></param>
        /// <param name="defaultTileRegion"></param>
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
