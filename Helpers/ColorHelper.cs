
using Microsoft.Xna.Framework;

namespace Monolith.Helpers
{
    public class ColorHelper
    {
        public static Color GetOppositeColor(Color original)
        {
            byte r = (byte)(255 - original.R);
            byte g = (byte)(255 - original.G);
            byte b = (byte)(255 - original.B);


            byte a = original.A;

            return new Color(r, g, b, a);
        }

        public static Color Multiply(Color a, Color b)
        {
            return new Color(
                (byte)(a.R * b.R / 255),
                (byte)(a.G * b.G / 255),
                (byte)(a.B * b.B / 255),
                (byte)(a.A * b.A / 255)
            );
        }
    }
}