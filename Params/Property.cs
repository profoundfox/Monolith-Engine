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

    ///<summary>
    /// Direcyly sets properties within an instance.
    ///<para>Example:</para>
    ///<code>
    /// var node = Engine.Table.Create&lt;Node&gt;().Set(n =&gt; { });
    ///</code>
    ///</summary>
    ///<param name="inst">A reference to the instance.</param
    ///<param name="config">The configuration.</param>
    public static T Set<T>(this T inst, Action<T> config)
      where T : Instance
    {
      config(inst);
      return inst;
    }

    ///<summary>
    /// Dynamically sets properties within an instance.
    ///</summary>
    ///<remarks>
    /// This is case senstive, it uses a direct lookup to a registry in <see cref="ClassDB"/>.
    /// </remarks>
    /// <remarks>
    /// If the name is typed wrong or the assigned value's type does not match the property; it will be ignored.
    ///</remarks>
    ///<param name="inst">A reference to the instance.</param>
    ///<param name="name">The name of the property which will be changed.</param>
    ///<param name="value">The new value.</param>
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

    ///<summary>
    /// Dynamically gets a property from its name.
    ///</summary>
    ///<remarks>
    /// This is case sensitive, it uses direct lookup to a registry in <see cref="ClassDB"/>.
    ///</remarks>
    ///<remarks>
    /// If the name is typed wrong or the property is not registered, it will return null.
    ///</remarks>
    ///<param name="inst">A reference to the instance.</param>
    ///<param name="name">The name of the property in search of.</param>
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
