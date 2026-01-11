
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monolith.Graphics
{
    public sealed class BitmapFont
    {
        public Texture2D Texture { get; }
        public int CharWidth { get; }
        public int CharHeight { get; }

        readonly Dictionary<char, int> map = new();
        readonly int columns;

        public BitmapFont(Texture2D texture, int charWidth, int charHeight)
        {
            Texture = texture;
            CharWidth = charWidth;
            CharHeight = charHeight;
            columns = texture.Width / charWidth;
        }

        public void AddMap(string charOrder)
        {
            for (int i = 0; i < charOrder.Length; i++)
                map[charOrder[i]] = i;
        }

        public bool TryGetSource(char c, out Rectangle source)
        {
            if (!map.TryGetValue(c, out int index))
            {
                source = default;
                return false;
            }

            int col = index % columns;
            int row = index / columns;

            source = new Rectangle(
                col * CharWidth,
                row * CharHeight,
                CharWidth,
                CharHeight
            );

            return true;
        }
    }

}