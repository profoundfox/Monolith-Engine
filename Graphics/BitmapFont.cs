
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace Monolith.Graphics
{
    public class BitmapFont
    {
        public MTexture MapTexture { get; private set; }
        public Dictionary<char, int> Map { get; private set; } = new();

        int charWidth, charHeight;
        int columns, rows;

        public BitmapFont(MTexture mapTexture, int charWidth, int charHeight)
        {
            MapTexture = mapTexture;
            this.charWidth = charWidth;
            this.charHeight = charHeight;

            columns = MapTexture.Width / this.charWidth;
            rows = MapTexture.Height / this.charHeight;

        }

        public void AddMap(string charOrder)
        {
            for (int i = 0; i < charOrder.Length; i++)
            {
                Map[charOrder[i]] = i;
            }
        }

        public void DrawString(string text, Vector2 position, Color color)
        {
            Vector2 cursor = position;

            foreach (char c in text)
            {
                if (c == '\n')
                {
                    cursor.X = position.X;
                    cursor.Y += charHeight;
                    continue;
                }

                if (!Map.TryGetValue(c, out int index))
                    continue;

                int col = index % columns;
                int row = index / columns;

                Rectangle region = new Rectangle(
                    col * charWidth,
                    row * charHeight,
                    charWidth,
                    charHeight
                );

                Console.WriteLine(region.ToString());

                MTexture character = MapTexture.CreateSubTexture(region);

                character.Draw(cursor, color);

                cursor.X += charWidth;
            }
}



    }
}