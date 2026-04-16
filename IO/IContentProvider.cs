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
        T LoadJson<T>(string path);

        T Load<T>(string path);

        void Unload(string path);
        void ClearCache();
    }


}
