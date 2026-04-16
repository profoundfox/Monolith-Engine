
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Monolith.Tools
{
    public static class ArrayExtension
    {
        public static List<Point> ToPoint(this List<Vector2> baseData)
        {
            return baseData.ConvertAll(vec => vec.ToPoint());
        }

        public static List<Vector2> ToVec2(this List<Point> baseData)
        {
            return baseData.ConvertAll(point => point.ToVector2());
        }

        public static int[] ClampArray(this int[] list, int min, int max)
        {
            return list.Select(x => Math.Clamp(x, min, max)).ToArray();
        }

        public static bool IsEmpty<T>(this ICollection<T> collection)
        {
            return collection.Count == 0;
        }
    }
}