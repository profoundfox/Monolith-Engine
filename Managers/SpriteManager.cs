
using System.Collections.Generic;
using Monolith.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace Monolith.Managers
{
    public class SpriteManager
    {
        public List<Sprite> Sprites = new();
        public Dictionary<string, Sprite> SpriteMap = new Dictionary<string, Sprite>();
        
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
            SpriteMap.Add(sprite.Name, sprite);
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

        public void DrawAllSprites(SpriteBatch spriteBatch)
        {
            foreach (var s in Sprites) 
            {
                Engine.DrawManager.Draw(s);
            }
        }

    }
}