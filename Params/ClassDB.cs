using System;
using System.Collections.Generic;
using System.Reflection;

namespace Monolith.Params
{
  public static class ClassDB
  {
    private static readonly Dictionary<Type, Dictionary<string, PropertyMeta>> _types = new();

    public static void Register(Type t, Dictionary<string, PropertyMeta> properties)
    {
      _types[t] = properties;
    }

    public static Dictionary<string, PropertyMeta> Get(Type t)
    {
      _types.TryGetValue(t, out var result);
      return result;
    }
  }

  public static class ClassDBInitializer
  {
    public static void Initialize(Assembly assembly)
    {
      foreach (var t in assembly.GetTypes())
      {
        RegisterType(t);
      }
    }
    
    private static void RegisterType(Type type)
    {
        var map = new Dictionary<string, PropertyMeta>();

        foreach (var prop in type.GetProperties())
        {
            if (!prop.IsDefined(typeof(ExportAttribute), true))
                continue;

            map[prop.Name] = BuildProperty(prop);
        }

        ClassDB.Register(type, map);
    }

    private static PropertyMeta BuildProperty(PropertyInfo prop)
    {
      return new PropertyMeta
      {
        Get = CreateGetter(prop),
        Set = CreateSetter(prop)
      };
    }

    private static Func<object, object> CreateGetter(PropertyInfo prop)
      => (obj) => prop.GetValue(obj);

    private static Action<object, object> CreateSetter(PropertyInfo prop)
      => (obj, value) => prop.SetValue(obj, value);
    
  }
}
