using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public readonly record struct Visual2D
{
    public Effect Shader { get; init; }
    public SpriteEffects SpriteEffects { get; init; }

    public Color Modulate { get; init; }

    public bool FlipH { get; init; }
    public bool FlipY { get; init; }

    public bool Visible { get; init; }
    
    public float LayerDepth { get; init; }

    public static readonly Visual2D Identity = new()
    {
        Shader = null,
        SpriteEffects = SpriteEffects.None,
        Modulate = Color.White,
        FlipH = false,
        FlipY = false,
        Visible = true,
        LayerDepth = 0.5f
    };


}