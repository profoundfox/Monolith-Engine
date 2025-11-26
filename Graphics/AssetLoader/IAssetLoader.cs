using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Monolith.Graphics
{

    public interface IAssetLoader
    {
        Texture2D LoadTexture(string path);
        SpriteFont LoadFont(string path);

        SoundEffect LoadSound(string path);
        Song LoadMusic(string path);

        // Data
        string LoadText(string path);
        T LoadJson<T>(string path);

        Effect LoadEffect(string path);
        byte[] LoadRaw(string path);

        void Unload(string path);
        void ClearCache();
    }

}
