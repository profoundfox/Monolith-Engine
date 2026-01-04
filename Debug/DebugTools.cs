
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
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
                foreach(CollisionShape2D node in Engine.Node.GetNodesByT<CollisionShape2D>()) 
                    node.Shape.Draw();
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