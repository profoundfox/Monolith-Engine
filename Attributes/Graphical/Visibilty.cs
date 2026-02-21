using System;
using Microsoft.Xna.Framework;
using Monolith.Helpers;

namespace Monolith.Attributes
{
    public readonly record struct Visibility : IProperty<Visibility>
    {   
        public bool Visibile { get; init; }
        public Color Modulate { get; init; }

        public static readonly Visibility Identity =
            new(true, Color.White);

        public Visibility(bool visibile, Color modulate)
        {
            Visibile = visibile;
            Modulate = modulate;
        }

        public static Visibility Combine(in Visibility parent, in Visibility child)
        {
            return new Visibility(
                parent.Visibile && child.Visibile,
                ColorHelper.Multiply(parent.Modulate, child.Modulate)
            );
        }

    }
}