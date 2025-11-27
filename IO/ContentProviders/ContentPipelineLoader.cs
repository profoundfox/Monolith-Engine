using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System.Text.Json;


namespace Monolith.IO
{
    public class ContentPipelineLoader : IContentProvider
    {

        public ContentPipelineLoader() {}
        
        /// <summary>
        /// Loads a texture using the content pipeline.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Texture2D LoadTexture(string path)
        {
            return Engine.Content.Load<Texture2D>(path);
        }

        /// <summary>
        /// Function for loading sound effects using the content pipeline.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public SoundEffect LoadSound(string path)
        {
            return Engine.Content.Load<SoundEffect>(path);
        }

        /// <summary>
        /// Function for loading music using the content pipeline.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Song LoadMusic(string path)
        {
            return Engine.Content.Load<Song>(path);
        }

        /// <summary>
        /// Function for loading text using the content pipeline.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string LoadText(string path)
        {
            return Engine.Content.Load<string>(path);
        }

        /// <summary>
        /// Function for loading a Json file using the content pipeline.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T LoadJson<T>(string path)
        {
            string json = LoadText(path);
            return JsonSerializer.Deserialize<T>(json);
        }

        /// <summary>
        /// Function for loading a font using the content pipeline.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public SpriteFont LoadFont(string path)
        {
            return Engine.Content.Load<SpriteFont>(path);
        }

        /// <summary>
        /// Function for loading an effect using the content pipeline.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Effect LoadEffect(string path)
        {
            return Engine.Content.Load<Effect>(path);
        }

        /// <summary>
        /// Function for loading raw file information using the content pipeline.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public byte[] LoadRaw(string path)
        {
            return Engine.Content.Load<byte[]>(path);
        }

        /// <summary>
        /// Function for unloading a content file.
        /// </summary>
        /// <param name="path"></param>
        /// <remarks>
        /// Individual unloading does not work when using the content pipeline.
        /// </remarks>
        [Obsolete("Unloading does not work with the content pipeline", false)]
        public void Unload(string path)
        {
            Console.WriteLine("You cannot unload individual items with the pipeline.");
        }

        /// <summary>
        /// Function for clearing the cache.
        /// </summary>
        public void ClearCache()
        {
            Engine.Content.Unload();
        }

        /// <summary>
        /// Function for reloading all content files.
        /// </summary>
        /// <remarks>
        /// Reloading does not work with the content pipeline.
        /// </remarks>
        [Obsolete("Reload all does not work with the content pipeline", false)]
        public void ReloadAll()
        {
            Console.WriteLine("You cannot reload all items with the pipeline.");
        }
    }
}
