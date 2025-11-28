
using System.Collections.Generic;
using Monolith.Graphics;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Monolith.Managers
{
    public class SpriteManager
    {
        public List<Sprite> Sprites = new();
        public Dictionary<string, List<Sprite>> SpriteMap = new();
        
        /// <summary>
        /// Creates a SpriteManager.
        /// </summary>
        public SpriteManager() { }

        /// <summary>
        /// Adds a sprite to the list and map.
        /// </summary>
        /// <param name="sprite"></param>
        public void AddSprite(Sprite sprite)
        {
            Sprites.Add(sprite);

            string key = sprite.Texture.Name;

            if (!SpriteMap.TryGetValue(key, out var list))
            {
                list = new List<Sprite>();
                SpriteMap[key] = list;
            }

            list.Add(sprite);
        }



        /// <summary>
        /// Adds multiple sprites from a list.
        /// </summary>
        /// <param name="sprites"></param>
        public void AddSprites(Sprite[] sprites)
        {
            foreach (var s in sprites)
            {
                AddSprite(s);
            }
        } 

        /// <summary>
        /// Empties the sprites from the list and map.
        /// </summary>
        public void Empty()
        {
            Sprites.Clear();
            SpriteMap.Clear();
        }

        /// <summary>
        /// Draws all the currently loaded sprites.
        /// </summary>
        /// <param name="spriteBatch"></param>

        public void DrawAllSprites()
        {
            foreach (var s in Sprites) 
            {
                s.Draw();
            }
        }

    }
}