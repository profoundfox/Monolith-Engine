
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monolith.Graphics
{
    public sealed class BitmapFont : IFont
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

        public Vector2 MeasureString(string text)
        {
            if (string.IsNullOrEmpty(text))
                return Vector2.Zero;

            int maxLineWidth = 0;
            int currentLineWidth = 0;
            int lineCount = 1;

            foreach (char c in text)
            {
                if (c == '\n')
                {
                    maxLineWidth = Math.Max(maxLineWidth, currentLineWidth);
                    currentLineWidth = 0;
                    lineCount++;
                    continue;
                }

                if (map.ContainsKey(c))
                    currentLineWidth += CharWidth;
            }

            maxLineWidth = Math.Max(maxLineWidth, currentLineWidth);

            return new Vector2(
                maxLineWidth,
                lineCount * CharHeight
            );
        }

        public void DrawString(SpriteBatch spriteBatch, string text, Vector2 position, Color color)
        => DrawString(spriteBatch, text, position, color, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);

        public void DrawString(SpriteBatch spriteBatch, string text, Vector2 position, Color color,
                            float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            Vector2 pos = position;

            foreach (char c in text)
            {
                if (c == '\n')
                {
                    pos.X = position.X;
                    pos.Y += CharHeight * scale.Y;
                    continue;
                }

                if (TryGetSource(c, out Rectangle source))
                {
                    spriteBatch.Draw(
                        Texture,
                        pos,
                        source,
                        color,
                        rotation,
                        origin,
                        scale,
                        effects,
                        layerDepth
                    );

                    pos.X += CharWidth * scale.X;
                }
            }
        }

    }
}