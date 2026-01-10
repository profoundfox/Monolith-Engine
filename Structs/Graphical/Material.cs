using Microsoft.Xna.Framework.Graphics;

namespace Monolith.Structs
{
    public readonly record struct Material : IProperty<Material>
    {
        public Effect Shader { get; init; }
        public SpriteEffects SpriteEffects { get; init; }

        public static readonly Material Identity =
            new(null, SpriteEffects.None);

        public Material(Effect shader, SpriteEffects spriteEffects)
        {
            Shader = shader;
            SpriteEffects = spriteEffects;
        }

        public static Material Combine(in Material parent, in Material child)
        {
            var shader = child.Shader ?? parent.Shader;
            return new Material(shader, child.SpriteEffects);
        }
    }
}