using Microsoft.Xna.Framework;

namespace Monolith.Geometry
{
    public struct AABB
    {
        public Vector2 Min;
        public Vector2 Max;

        public Vector2 Size => Max - Min;
        public float Left => Min.X;
        public float Right => Max.X;
        public float Top => Min.Y;
        public float Bottom => Max.Y;

        public AABB(Vector2 min, Vector2 max)
        {
            Min = min;
            Max = max;
        }
    }
}