using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Monolith.IO
{
    public class RuntimeContentLoader : IContentProvider
    {
        private readonly Dictionary<string, Texture2D> _textureCache = new();
        private readonly Dictionary<string, SoundEffect> _soundCache = new();
        private readonly Dictionary<string, Song> _musicCache = new();
        private readonly Dictionary<string, string> _textCache = new();
        private readonly HashSet<string> _loadedRelativePaths = new();
        private readonly string _rawContentPath;
        private readonly bool _allowAbsolutePaths;
        private bool _canReload;
        private bool _waitForReload;

        /// <summary>
        /// Creates a new RuntimeContentLoader
        /// </summary>
        /// <param name="rawContentPath">Base directory for assets.</param>
        /// <param name="allowAbsolutePaths">If true, can load files from absolute paths on the system. Intended for development/testing only.</param>
        /// <remarks> 
        /// Intended for non release builds as the pipeline produces cleaner and faster files,
        /// release builds also need to have all files relative to the game's exe.
        /// </remarks>
        public RuntimeContentLoader(string rawContentPath, bool allowAbsolutePaths = false)
        {
            _rawContentPath = rawContentPath ?? throw new ArgumentNullException(nameof(rawContentPath));
            _allowAbsolutePaths = allowAbsolutePaths;
        }

        /// <summary>
        /// Normalizes a given path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        private string NormalizePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));

            if (!_allowAbsolutePaths && Path.IsPathRooted(_rawContentPath))
                throw new InvalidOperationException(
                    "Absolute rawContentPath is not allowed unless allowAbsolutePaths is enabled."
                );

            if (Path.IsPathRooted(path))
                throw new InvalidOperationException(
                    "Asset paths must be relative. Do not pass absolute paths to the loader."
                );

            return Path.GetFullPath(Path.Combine(_rawContentPath, path));
        }


        /// <summary>
        /// Loads a texture from a file at runtime.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public Texture2D LoadTexture(string path)
        {
            string comPath = NormalizePath(path);

            if (_textureCache.TryGetValue(comPath, out var cached))
                return cached;

            if (!File.Exists(comPath))
                throw new FileNotFoundException($"Texture file not found: {comPath}");

            using var stream = File.OpenRead(comPath);
            var texture = Texture2D.FromStream(Engine.GraphicsDevice, stream);
            _textureCache[comPath] = texture;

            _loadedRelativePaths.Add(path);

            return texture;
        }

        /// <summary>
        /// Loads sound from a file at runtime.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public SoundEffect LoadSound(string path)
        {
            string comPath = NormalizePath(path);

            if (_soundCache.TryGetValue(comPath, out var cached))
                return cached;

            if (!File.Exists(comPath))
                throw new FileNotFoundException($"Sound file not found: {comPath}");

            using var stream = File.OpenRead(comPath);
            var sound = SoundEffect.FromStream(stream);
            _soundCache[comPath] = sound;

            _loadedRelativePaths.Add(path);

            return sound;
        }

        /// <summary>
        /// Loads music from a file at runtime.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public Song LoadMusic(string path)
        {
            string comPath = NormalizePath(path);
            if (_musicCache.TryGetValue(comPath, out var cached))
                return cached;

            if (!File.Exists(comPath))
                throw new FileNotFoundException($"Music file not found: {comPath}");

            var song = Song.FromUri(Path.GetFileName(comPath), new Uri(Path.GetFullPath(comPath)));
            _musicCache[comPath] = song;

            _loadedRelativePaths.Add(path);

            return song;
        }

        /// <summary>
        /// Loads text from a file at runtime.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public string LoadText(string path)
        {
            string comPath = NormalizePath(path);
            if (_textCache.TryGetValue(comPath, out var cached))
                return cached;

            if (!File.Exists(comPath))
                throw new FileNotFoundException($"Text file not found: {comPath}");

            var text = File.ReadAllText(comPath);
            _textCache[comPath] = text;

            _loadedRelativePaths.Add(path);

            return text;
        }

        /// <summary>
        /// Loads text asynchronous from a file at runtime.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public async Task<string> LoadTextAsync(string path)
        {
            string comPath = NormalizePath(path);
            if (_textCache.TryGetValue(comPath, out var cached))
                return cached;

            if (!File.Exists(comPath))
                throw new FileNotFoundException($"Text file not found: {comPath}");

            var text = await File.ReadAllTextAsync(comPath);
            _textCache[comPath] = text;

            _loadedRelativePaths.Add(path);

            return text;
        }

        /// <summary>
        /// Loads a Json file at runtime.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public T LoadJson<T>(string path)
        {
            string comPath = NormalizePath(path);
            string json = LoadText(path);

            try
            {
                _loadedRelativePaths.Add(path);
                return JsonSerializer.Deserialize<T>(json) ?? throw new Exception("Deserialized object was null.");
            }
            catch (JsonException e)
            {
                throw new InvalidOperationException($"Failed to deserialize JSON from {comPath}", e);
            }
        }

        /// <summary>
        /// Function for loading raw information from a file at runtime.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public byte[] LoadRaw(string path)
        {
            string comPath = NormalizePath(path);

            if (!File.Exists(comPath))
                throw new FileNotFoundException($"Raw file not found: {comPath}");

            _loadedRelativePaths.Add(path);

            return File.ReadAllBytes(comPath);
        }

        /// <summary>
        /// Function for loading a font at runtime.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <remarks> 
        /// Runtime font loading does not exist.
        /// </remarks>
        public SpriteFont LoadFont(string path)
        {
            throw new NotImplementedException("Runtime SpriteFont loader not implemented.");
        }

        /// <summary>
        /// Function for loading an effect at runtime.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <remarks> 
        /// Runtime effect loading does not exist.
        /// </remarks>
        public Effect LoadEffect(string path)
        {
            throw new NotImplementedException("Runtime Effect loader not implemented. Use Content Pipeline instead.");
        }
        
        /// <summary>
        /// Unloads a texture from its location.
        /// </summary>
        /// <param name="path"></param>
        public void Unload(string path)
        {
            string comPath = NormalizePath(path);

            if (_textureCache.TryGetValue(comPath, out var tex))
            {
                tex.Dispose();
                _textureCache.Remove(comPath);
            }

            if (_soundCache.TryGetValue(comPath, out var snd))
            {
                snd.Dispose();
                _soundCache.Remove(comPath);
            }

            _musicCache.Remove(comPath);
            _textCache.Remove(comPath);
        }

        /// <summary>
        /// Clears the content cache.
        /// </summary>
        public void ClearCache()
        {
            foreach (var texture in _textureCache.Values)
                texture.Dispose();

            foreach (var sound in _soundCache.Values)
                sound.Dispose();

            _textureCache.Clear();
            _soundCache.Clear();
            _musicCache.Clear();
            _textCache.Clear();
        }

        /// <summary>
        /// <summary>
        /// Reloads all runtime content. **WARNING:** Only use if you know what you are doing.
        /// </summary>
        /// <remarks>
        /// This method forcibly reloads all textures, sounds, music, and text content.
        /// Misuse can cause resource leaks or inconsistent runtime behavior.
        /// </remarks>
        [Obsolete("ReloadAll is unsafe. Only use if you know what you are doing.", false)]
        public void ReloadAll()
        {
            Console.WriteLine("[Hot Reload] Reloading all runtime content...");

            foreach (var path in _loadedRelativePaths.ToArray())
            {
                if (_textureCache.TryGetValue(NormalizePath(path), out var tex))
                {
                    tex.Dispose();
                    _textureCache[NormalizePath(path)] = LoadTexture(path);
                }

                if (_soundCache.TryGetValue(NormalizePath(path), out var snd))
                {
                    snd.Dispose();
                    _soundCache[NormalizePath(path)] = LoadSound(path);
                }

                if (_musicCache.ContainsKey(NormalizePath(path)))
                    _musicCache[NormalizePath(path)] = LoadMusic(path);

                if (_textCache.ContainsKey(NormalizePath(path)))
                    _textCache[NormalizePath(path)] = LoadText(path);
            }

            Console.WriteLine("[Hot Reload] Reload complete!");
        }


    }
}

