using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith;
using Monolith.Graphics;
using Monolith.Managers;

public static class DebugOverlay
{   
    public enum Side
    {
        Left,
        Right
    }
    
    public static bool IsEnabled = true;
    private static Dictionary<string, DebugEntry> debugInfos = new();

    private class DebugEntry
    {
        public Func<string> TextFunc;
        public Color Color;
        public Side Side;

    }

    public static void ToggleOverlay()
    {
        IsEnabled = !IsEnabled;
    }

    public static void AddInfo(string key, Func<string> infoFunc, Color color, Side side = Side.Left)
    {
        debugInfos[key] = new DebugEntry
        {
            TextFunc = infoFunc,
            Color = color,
            Side = side
        };
    }

    public static void AddInfo(string key, Func<string> infoFunc, Side side = Side.Left)
    {
        AddInfo(key, infoFunc, Color.White, side);
    }

    public static void RemoveInfo(string key)
    {
        debugInfos.Remove(key);
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        if (!IsEnabled || Engine.Instance.Font == null)
            return;

        spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

        int yLeft = 10;
        int yRight = 10;
        int screenWidth = Engine.GraphicsDevice.Viewport.Width;

        foreach (var entry in debugInfos.Values)
        {
            string text;
            try
            {
                text = entry.TextFunc.Invoke() ?? "";
            }
            catch (Exception ex)
            {
                text = $"<error: {ex.Message}>";
            }

            switch (entry.Side)
            {
                case Side.Left:

                    Engine.Screen.DrawString(new TextDrawCall
                    {
                        Font = Engine.Instance.Font,
                        Text = text, 
                        Position = new Vector2(10, yLeft), 
                        Color = entry.Color, 
                        Depth = 100
                    }, DrawLayer.UI);
    
                    yLeft += 20;
                    break;

                case Side.Right:
                    Vector2 textSize = Engine.Instance.Font.MeasureString(text);
                    
                    Engine.Screen.DrawString(new TextDrawCall
                    {
                        Font = Engine.Instance.Font,
                        Text = text, 
                        Position = new Vector2(screenWidth - textSize.X - 10, yRight), 
                        Color = entry.Color, 
                        Depth = 100
                    }, DrawLayer.UI);

                    yRight += 20;
                    break;
            }
        }

        spriteBatch.End();
    }


}
