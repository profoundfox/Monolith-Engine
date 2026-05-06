using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System.Text.Json;
using Monolith.Graphics;

namespace Monolith.IO
{
  public class PipelineLoader : IContentProvider
  {
    private readonly Dictionary<Type, Func<string, object>> _genericLoaders = new();
    private readonly Dictionary<(Type, string), object> _cache = new();

    public PipelineLoader()
    {
      Register<MTexture>(p => new MTexture(Load<Texture2D>(p)));
    }

    /// <summary>
    /// Registers a loader to the index of generics.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="loader"></param>
    public void Register<T>(Func<string, T> loader)
    {
      _genericLoaders[typeof(T)] = path => loader(path)!;
    }

    /// <summary>
    /// Generic function for loading resources with the content pipeline.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public T Load<T>(string path)
    {
      var key = (typeof(T), path);

      if (_cache.TryGetValue(key, out var cached))
        return (T)cached;

      T result;

      if (_genericLoaders.TryGetValue(typeof(T), out var loader))
      {
        result = (T)loader(path);
      }
      else
      {
        try
        {
          result = Core.Tracked.Content.Load<T>(path);
        }
        catch
        {
          if (!typeof(T).IsPrimitive && typeof(T) != typeof(string))
            result = LoadJson<T>(path);
          else
            throw new NotSupportedException(
                $"No loader registered for type {typeof(T).FullName} (path: '{path}')");
        }
      }

      _cache[key] = result!;
      return result;
    }

    /// <summary>
    /// Loads text using the content pipeline.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public string LoadText(string path)
    {
      return Core.Tracked.Content.Load<string>(path);
    }

    /// <summary>
    /// Loads a JSON using the content pipeline.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public T LoadJson<T>(string path)
    {
      string json = LoadText(path);
      return JsonSerializer.Deserialize<T>(json)!;
    }

    [Obsolete("Unloading does not work with the Tracked.Content pipeline", false)]
    public void Unload(string path)
    {
      Console.WriteLine("You cannot unload individual items with the pipeline.");
    }

    /// <summary>
    /// Clears the content pipeline's cache.
    /// </summary>
    public void ClearCache()
    {
      _cache.Clear();
      Core.Tracked.Content.Unload();
    }
  }
}