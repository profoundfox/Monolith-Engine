using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Monolith.Tools
{
  public static class DictionaryExtension
  {
    public static Dictionary<TKey, List<TValue>> CloneDictionaryOfLists<TKey, TValue>(
          Dictionary<TKey, List<TValue>> original,
          Func<TValue, TValue> cloneFunc)
    {
      return original.ToDictionary(
          kvp => kvp.Key,
          kvp => kvp.Value.Select(cloneFunc).ToList()
      );
    }


    public static bool TryConvert(this object? value, Type targetType, out object? result)
    {
      result = null;

      if (value == null)
        return true;

      var type = Nullable.GetUnderlyingType(targetType) ?? targetType;

      if (type.IsInstanceOfType(value))
      {
        result = value;
        return true;
      }

      try
      {
        if (type.IsEnum)
        {
          if (value is string s)
          {
            result = Enum.Parse(type, s, ignoreCase: true);
            return true;
          }

          result = Enum.ToObject(type, value);
          return true;
        }

        result = Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
        return true;
      }
      catch
      {
        return false;
      }
    }
  }
}
