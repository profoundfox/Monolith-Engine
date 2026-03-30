
using System.Collections.Generic;
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
    }
}