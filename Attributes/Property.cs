using System;
using System.Collections.Generic;
using Monolith.Managers;

namespace Monolith.Attributes
{
    public interface IProperty<TSelf>
        where TSelf : struct
    {
        static abstract TSelf Combine(in TSelf parent, in TSelf child);
    }

    public static class Properties
    {
        public static T SetProperties<T>(this T node, Action<T> config)
            where T : Instance
        {
            config(node);
            return node;
        }

        public static T SetPropertiesDynamic<T>(this T node, Dictionary<string, object> values)
            where T : Instance
        {
            var type = typeof(T);
            foreach (var kv in values)
            {
                var prop = type.GetProperty(kv.Key);
                if (prop != null && prop.CanWrite)
                    prop.SetValue(node, kv.Value);
            }
            return node;
        }
    }
}