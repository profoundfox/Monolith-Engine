using System.Collections.Generic;

namespace Monolith.Helpers
{
    public static class ValueHelper
    {
        public static Dictionary<string, object> ToDictionary(object obj)
        {
            if (obj == null) return new Dictionary<string, object>();
            var dict = new Dictionary<string, object>();
            foreach (var prop in obj.GetType().GetProperties())
            {
                dict[prop.Name] = prop.GetValue(obj);
            }
            return dict;
        }
    }
}
