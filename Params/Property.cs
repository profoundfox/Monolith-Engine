using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Monolith.Managers;
using Monolith.Tools;

namespace Monolith.Params
{
    public interface IProperty<TSelf>
        where TSelf : struct
    {
        static abstract TSelf Combine(in TSelf parent, in TSelf child);
    } 

    public static class Params
    {
      public static T SetParams<T>(this T inst, Action<T> config)
          where T : Instance
      {
          config(inst);
          return inst;
      }
        
      private static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> _propertyCache = new();

      private static Dictionary<string, PropertyInfo> GetPropertyMap(Type type)
      {
          if (_propertyCache.TryGetValue(type, out var map))
              return map;

          map = type
              .GetProperties(BindingFlags.Public | BindingFlags.Instance)
              .Where(p => p.CanWrite)
              .ToDictionary(p => p.Name, StringComparer.Ordinal);

          _propertyCache[type] = map;
          return map;
      }

    public static T TupleParams<T>(
        this T inst,
        params (string Key, object? Value)[] values)
        where T : Instance
    {
      var props = GetPropertyMap(typeof(T));

      foreach (var (key, value) in values)
      {
           if (!props.TryGetValue(key, out var prop))
              continue;
          if (!value.TryConvert(prop.PropertyType, out var converted))
              continue;

          try
          {
              prop.SetValue(inst, converted);
          }
          catch
          {
              
          }
      }

      return inst;
    }

    public static T Set<T>(this T inst, string name, object value)
      where T : Instance
    {
      var props = ClassDB.Get(typeof(T));

      if (props != null && props.TryGetValue(name, out var meta))
      {
        meta.Set(inst, value);
        return inst; 
      }
      
      inst.OnSetFallback?.Invoke(name, value);
      return inst;
    }

    public static object Get<T>(this T inst, string name)
      where T : Instance
    {
      var props = ClassDB.Get(typeof(T));

      if (props != null && props.TryGetValue(name, out var meta))
      {
        return meta.Get(inst);
      }

      inst.OnFallback?.Invoke(name);
      return null;
    }
  }
}
