
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monlith.Nodes;
using Monolith.Helpers;
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
            {
                foreach(CollisionShape2D node in NodeManager.GetNodesByType<CollisionShape2D>()) 
                    DrawHelper.DrawRegionShapeHollow(node.Shape, Color.Red, 2);
            }
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