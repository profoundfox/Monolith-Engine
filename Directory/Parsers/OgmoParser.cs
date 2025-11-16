using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using ConstructEngine.Components.Entity;
using ConstructEngine.Graphics;
using ConstructEngine.Objects;
using ConstructEngine.Util;
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

            foreach (var layer in root.layers.Where(l => l.folder != null && l.decals != null))
            {
                foreach (var decal in layer.decals)
                {
                    string path = layer.folder + "/" + decal.texture;
                    int index = path.IndexOf("Assets", StringComparison.OrdinalIgnoreCase);
                    path = index >= 0 ? path.Substring(index) : path;
                    path = Path.ChangeExtension(path, null);

                    Texture2D texture = Engine.Content.Load<Texture2D>(path);
                    var atlas = new TextureAtlas(texture);
                    atlas.AddRegion(path, 0, 0, texture.Width, texture.Height);

                    var sprite = atlas.CreateSprite(path);
                    sprite.StartPosition = new Vector2(decal.x, decal.y);
                    Sprite.SpriteList.Add(sprite);
                }
            }
        }

        /// <summary>
        /// Finds an entity in the level file extracts the position and parses the type, returns a dictionary with its position and the entity.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>

        public static Dictionary<Entity, Vector2> GetEntityData(string filename)
        {
            var root = LoadJson(filename);
            var entityDict = new Dictionary<Entity, Vector2>();

            foreach (var entity in root.layers.Where(l => l.entities != null).SelectMany(l => l.entities))
            {
                Type type = GetTypeByName<Entity>(entity.name);
                if (type != null)
                {
                    Entity instance = (Entity)Activator.CreateInstance(type);
                    entityDict[instance] = new Vector2(entity.x, entity.y);
                }
            }

            return entityDict;
        }

        /// <summary>
        /// Searches for ConstructObjects within the enties layer in the level file, sets the object's values from the file.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="Player"></param>

        public static void SearchForObjects(string filename, Entity Player = null)
        {
            var root = LoadJson(filename);

            foreach (var entity in root.layers.Where(l => l.entities != null).SelectMany(l => l.entities))
            {
                if (entity.values == null) continue;

                var normalDict = ParseValues(entity.values);

                Type type = GetTypeByName<ConstructObject>(entity.name);
                if (type != null)
                {
                    var obj = (ConstructObject)Activator.CreateInstance(type);
                    obj.Rectangle = new(entity.x, entity.y, entity.width, entity.height);
                    obj.Name = entity.name;
                    obj.Values = normalDict;
                    obj.Player = Player;
                    obj.CurrentSceneManager = Engine.SceneManager;
                    ConstructObject.LoadObjects();
                }
            }
        }

        /// <summary>
        /// Instantiates all the entites from the GetEntityData dictionary and sets their position.
        /// </summary>
        /// <param name="filePath"></param>

        public static void InstantiateEntities(string filePath)
        {
            var entityDict = GetEntityData(filePath);
            foreach (var kv in entityDict)
            {
                Entity.EntityList.Add(kv.Key);
                kv.Key.Load();
                kv.Key.KinematicBase.Collider.Rect.X = (int)kv.Value.X;
                kv.Key.KinematicBase.Collider.Rect.Y = (int)kv.Value.Y;
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
                Tilemap.Tilemaps.Add(tilemap);
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
            Entity Player = null, 
            string defaultTileTexture = null, 
            string defaultTileRegion = null)
        {
            // 1. Instantiate all entities
            InstantiateEntities(filename);

            // 2. Load ConstructObjects
            SearchForObjects(filename, Player);

            // 3. Load decals
            SearchForDecals(filename);

            // 4. Load tilemaps if ContentManager and defaults are provided
            if (!string.IsNullOrEmpty(defaultTileTexture) && !string.IsNullOrEmpty(defaultTileRegion))
            {
                LoadTilemap(Engine.Content, filename, defaultTileTexture, defaultTileRegion);
            }
        }

    }

}
