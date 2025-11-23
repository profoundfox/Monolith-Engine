using System;
using System.Collections.Generic;
using System.Linq;

namespace Monolith.Helpers
{
    public class DictionaryHelper
    {
        public static Dictionary<TKey, List<TValue>> CloneDictionaryOfLists<TKey, TValue>(
            Dictionary<TKey, List<TValue>> original,
            Func<TValue, TValue> cloneFunc
        )
        {
            return original.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Select(cloneFunc).ToList()
            );
        }

    }
}
