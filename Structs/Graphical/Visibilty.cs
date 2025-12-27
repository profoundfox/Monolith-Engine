using Microsoft.Xna.Framework;
using Monolith.Helpers;

namespace Monolith.Structs
{
    public readonly record struct Visibility 
    {   
        public bool Visibile { get; init; }
        public Color Modulate { get; init; }
        public Color SelfModulate { get; init; }

        public static readonly Visibility Identity =
            new(true, Color.White, Color.White);

        public Visibility(bool visibile, Color modulate, Color selfModulate)
        {
            Visibile = visibile;
            Modulate = modulate;
            SelfModulate = selfModulate;
        }

        public static Visibility Combine(
            in Visibility parent,
            in Visibility child
        )
        {
                bool combinedVisible = parent.Visibile && child.Visibile;
                Color combinedModulate = ColorHelper.Multiply(parent.Modulate, child.Modulate);
                Color combinedSelfModulate = child.SelfModulate;

                return new Visibility(combinedVisible, combinedModulate, combinedSelfModulate);
        }
    }
}