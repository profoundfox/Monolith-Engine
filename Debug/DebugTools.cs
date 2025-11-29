
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monolith.Managers;
using Monolith.Nodes;

namespace Monolith
{
    public class DebugTools
    {
        public static Dictionary<Keys, Action> DebugShortcuts = new Dictionary<Keys, Action>();
        
        private static bool drawRegions;

        public static void AddShortcut(Keys key, Action action)
        {
            DebugShortcuts.Add(key, action);
        }
        public static void DrawRegions()
        {
            if (drawRegions)
                foreach(Node2D node in NodeManager.AllInstances) node.DrawShapeHollow(Color.Red);
        }

        public static void ToggleRegions()
        {
            drawRegions = !drawRegions;
        }

        public static void CheckForShorcuts()
        {
            foreach(var kvp in DebugShortcuts)
            {
                if (Engine.Input.Keyboard.WasKeyJustPressed(kvp.Key))
                    kvp.Value();
            }
        }

        public static void Update()
        {
            CheckForShorcuts();
            DrawRegions();
        }
    }
}