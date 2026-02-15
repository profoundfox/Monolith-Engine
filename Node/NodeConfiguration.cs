using System;
using System.Collections.Generic;

namespace Monolith.Nodes
{
    public static class Properties
    {
        public static T SetProperties<T>(this T node, Action<T> config)
            where T : Node
        {
            config(node);
            return node;
        }

        public static T SetPropertiesDynamic<T>(this T node, Dictionary<string, object> values)
            where T : Node
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