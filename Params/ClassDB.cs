using System;
using System.Collections.Generic;
using System.Reflection;

namespace Monolith.Params
{
  public static class ClassDB
  {
    private static readonly Dictionary<Type, Dictionary<string, PropertyMeta>> _types = new();

    ///<summary>
    /// Registers all properties in the current assembly.
    ///</summary>
    ///<remarks>
    /// The property has to have an <see cref="ExportAttribute"/>.
    /// This funtion has to be called for each asssembly; that is to say each project.
    ///</remarks>
    ///<param name="assembly">The current assembly.</param>
    public static void Initialize(Assembly assembly)
    {
      foreach (var t in assembly.GetTypes())
      {
        RegisterType(t);
      }
    }

    public static void Register(Type t, Dictionary<string, PropertyMeta> properties)
    {
      _types[t] = properties;
    }

    ///<summary>
    /// Gets the properties accessible from a specified type. 
    ///</summary>
    ///<remarks>
    /// It has to include an <see cref="ExportAttribute"/> for each property you want accessible.
    ///</remarks>
    ///<param name="type">The type it will search.</param>
    public static Dictionary<string, PropertyMeta> Get(Type type)
    {
      _types.TryGetValue(type, out var result);
      return result;
    }

    ///<summary>
    /// Registers a type.
    ///</summary>
    ///<param name="type">The type in question</param>
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

    ///<summary>
    /// Builds property meta for a property, assigns their getters and setters.
    ///</summary>
    ///<param name="prop">The property in question</param>
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

