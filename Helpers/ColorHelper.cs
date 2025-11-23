
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
    }

}