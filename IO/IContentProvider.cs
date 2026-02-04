using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Monolith.IO
{
    public interface IContentProvider
    {
        Texture2D LoadTexture(string path);
        SpriteFont LoadFont(string path);

        SoundEffect LoadSound(string path);
        Song LoadMusic(string path);

        string LoadText(string path);
        T LoadJson<T>(string path);

        Effect LoadEffect(string path);
        byte[] LoadRaw(string path);

        T Load<T>(string path); 

        void Unload(string path);
        void ClearCache();
        void ReloadAll();
    }


}
