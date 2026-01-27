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

        private readonly Dictionary<Type, Func<string, object>> _genericLoaders =
            new Dictionary<Type, Func<string, object>>();

        public ContentPipelineLoader()
        {
            _genericLoaders[typeof(Texture2D)] = p => LoadTexture(p);
            _genericLoaders[typeof(SoundEffect)] = p => LoadSound(p);
            _genericLoaders[typeof(Song)] = p => LoadMusic(p);
            _genericLoaders[typeof(SpriteFont)] = p => LoadFont(p);
            _genericLoaders[typeof(Effect)] = p => LoadEffect(p);
            _genericLoaders[typeof(byte[])] = p => LoadRaw(p);

            _genericLoaders[typeof(string)] = p => LoadText(p);
        }

        /// <summary>
        /// Generic loader for the Instance.Content pipeline.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public T Load<T>(string path)
        {
            if (_genericLoaders.TryGetValue(typeof(T), out var loader))
                return (T)loader(path);

            if (!typeof(T).IsPrimitive && typeof(T) != typeof(string))
                return LoadJson<T>(path);

            throw new NotSupportedException($"No loader registered for type {typeof(T).Name}");
        }


        /// <summary>
        /// Loads a texture using the Instance.Content pipeline.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Texture2D LoadTexture(string path)
        {
            return Engine.Instance.Content.Load<Texture2D>(path);
        }

        /// <summary>
        /// Function for loading sound effects using the Instance.Content pipeline.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public SoundEffect LoadSound(string path)
        {
            return Engine.Instance.Content.Load<SoundEffect>(path);
        }

        /// <summary>
        /// Function for loading music using the Instance.Content pipeline.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Song LoadMusic(string path)
        {
            return Engine.Instance.Content.Load<Song>(path);
        }

        /// <summary>
        /// Function for loading text using the Instance.Content pipeline.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string LoadText(string path)
        {
            return Engine.Instance.Content.Load<string>(path);
        }

        /// <summary>
        /// Function for loading a Json file using the Instance.Content pipeline.
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
        /// Function for loading a font using the Instance.Content pipeline.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public SpriteFont LoadFont(string path)
        {
            return Engine.Instance.Content.Load<SpriteFont>(path);
        }

        /// <summary>
        /// Function for loading an effect using the Instance.Content pipeline.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Effect LoadEffect(string path)
        {
            return Engine.Instance.Content.Load<Effect>(path);
        }

        /// <summary>
        /// Function for loading raw file information using the Instance.Content pipeline.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public byte[] LoadRaw(string path)
        {
            return Engine.Instance.Content.Load<byte[]>(path);
        }

        /// <summary>
        /// Function for unloading a Instance.Content file.
        /// </summary>
        /// <param name="path"></param>
        /// <remarks>
        /// Individual unloading does not work when using the Instance.Content pipeline.
        /// </remarks>
        [Obsolete("Unloading does not work with the Instance.Content pipeline", false)]
        public void Unload(string path)
        {
            Console.WriteLine("You cannot unload individual items with the pipeline.");
        }

        /// <summary>
        /// Function for clearing the cache.
        /// </summary>
        public void ClearCache()
        {
            Engine.Instance.Content.Unload();
        }

        /// <summary>
        /// Function for reloading all Instance.Content files.
        /// </summary>
        /// <remarks>
        /// Reloading does not work with the Instance.Content pipeline.
        /// </remarks>
        [Obsolete("Reload all does not work with the Instance.Content pipeline", false)]
        public void ReloadAll()
        {
            Console.WriteLine("You cannot reload all items with the pipeline.");
        }
    }
}
