using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;


namespace Monolith.Graphics
{
    public class RuntimeLoader : IAssetLoader
    {
        private GraphicsDevice _gfx;

        public RuntimeLoader(GraphicsDevice gfx)
        {
            _gfx = gfx;
        }

        public Texture2D LoadTexture(string path)
        {
            using (var stream = File.OpenRead(path))
                return Texture2D.FromStream(_gfx, stream);
        }

        public BitmapFont LoadFont(string path)
        {
            string pngPath = Path.ChangeExtension(path, ".png");
            return new BitmapFont(_gfx, path, pngPath);
        }

        public SoundEffect LoadSound(string path)
        {
            using (var stream = File.OpenRead(path))
                return SoundEffect.FromStream(stream);
        }
    }
}
