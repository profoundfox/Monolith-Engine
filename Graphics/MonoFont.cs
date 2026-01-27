using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Text;

namespace Monolith.Graphics
{
    public class MonoFont : IFont
    {
        private readonly SpriteFont _font;

        public MonoFont(SpriteFont font)
        {
            _font = font ?? throw new System.ArgumentNullException(nameof(font));
        }

        // --- Properties mirrored from SpriteFont ---
        public IReadOnlyList<char> Characters => _font.Characters;
        public char? DefaultCharacter
        {
            get => _font.DefaultCharacter;
            set => _font.DefaultCharacter = value;
        }

        public int LineSpacing
        {
            get => _font.LineSpacing;
            set => _font.LineSpacing = value;
        }

        public float Spacing
        {
            get => _font.Spacing;
            set => _font.Spacing = value;
        }

        public Vector2 MeasureString(string text) => _font.MeasureString(text);
        public Vector2 MeasureString(StringBuilder text) => _font.MeasureString(text);

        Vector2 IFont.MeasureString(string text) => MeasureString(text);

        public void DrawString(SpriteBatch spriteBatch, string text, Vector2 position, Color color)
            => spriteBatch.DrawString(_font, text, position, color);

        public void DrawString(SpriteBatch spriteBatch, string text, Vector2 position, Color color,
                               float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
            => spriteBatch.DrawString(_font, text, position, color, rotation, origin, scale, effects, layerDepth);

        public void DrawString(SpriteBatch spriteBatch, StringBuilder text, Vector2 position, Color color)
            => spriteBatch.DrawString(_font, text, position, color);

        public void DrawString(SpriteBatch spriteBatch, StringBuilder text, Vector2 position, Color color,
                               float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
            => spriteBatch.DrawString(_font, text, position, color, rotation, origin, scale, effects, layerDepth);

        void IFont.DrawString(SpriteBatch spriteBatch, string text, Vector2 position, Color color)
            => DrawString(spriteBatch, text, position, color);

        public SpriteFont InnerFont => _font;
    }
}
