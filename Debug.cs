using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monolith;
using Monolith.Graphics;
using Monolith.Helpers;
using Monolith.Nodes;
using Monolith.Util;

public static class Debug
{
    private static Vector2 padding = new Vector2(10, 10);
    private static float lineHeight = 18f;
    private static bool drawRegions;
    private static bool showDebugScreen;

   public static void DrawGeneralNodeInfo()
    {
        if (!showDebugScreen)
            return;
        Dictionary<string, Color> leftLines = new Dictionary<string, Color>
        {
            {$"Total Nodes: {Node.AllInstances.Count}", Color.Lime},
            {$"Node Types: {Node.AllInstancesDetailed.Count}", Color.Aqua}
        };

        Dictionary<string, Color> rightLines = new Dictionary<string, Color>
        {
            {"T: Draw regions, R: Reload scene, U: Show overlay", Color.Yellow}
        };

        DrawOnScreen(leftLines, rightLines);

    }

    public static void DrawOnScreen(Dictionary<string, Color> leftLines, Dictionary<string, Color>  rightLines)
    {
        if (!showDebugScreen)
            return;
        var edges = MCamera.CurrentCamera.GetScreenEdges();

        Vector2 leftPos = edges.TopLeft + padding;
        foreach (var line in leftLines)
        {
            DrawHelper.DrawString(line.Key, line.Value, leftPos);
            leftPos.Y += lineHeight;
        }

        Vector2 rightPos = edges.TopRight + new Vector2(-padding.X, padding.Y);
        foreach (var line in rightLines)
        {
            Vector2 textSize = Engine.Font.MeasureString(line.Key);
            Vector2 drawPos = new Vector2(rightPos.X - textSize.X, rightPos.Y);
            DrawHelper.DrawString(line.Key, line.Value, drawPos);
            rightPos.Y += lineHeight;
        }
    }

    public static void DebugLogic()
    {
        if (Engine.Input.Keyboard.WasKeyJustPressed(Keys.T))
            drawRegions = !drawRegions;
        if (Engine.Input.Keyboard.WasKeyJustPressed(Keys.R))
            Engine.SceneManager.ReloadCurrentScene();
        if (Engine.Input.Keyboard.WasKeyJustPressed(Keys.U))
            showDebugScreen = !showDebugScreen;
        if (drawRegions)
            foreach(var node in Node.AllInstances) node.DrawShapeHollow(Color.Red);
    }
}
