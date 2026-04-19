using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monolith.Params
{
  public record struct CanvasParams
  {
    public Vector2 Position { get; set; }
    public Color Color { get; set; }
    public float Rotation { get; set; }
    public Vector2 Origin { get; set; }
    public Vector2 Scale { get; set; }
    public SpriteEffects Effects { get; set; }

    public static readonly CanvasParams Identity = new(
        pos: Vector2.Zero, color: Color.White, rotation: 0f,
        origin: Vector2.Zero, scale: Vector2.One, effects: SpriteEffects.None);

    public CanvasParams(Vector2 pos, Color color, float rotation,
        Vector2 origin, Vector2 scale, SpriteEffects effects)
    {
      Position = pos;
      Color = color;
      Rotation = rotation;
      Origin = origin;
      Scale = scale;
      Effects = effects;
    }
  }
}
