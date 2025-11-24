using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monolith.Helpers;
using Monolith.Nodes;
using Monolith.Util;

namespace Monolith.Debugger
{
    public static class Debug
    {
        private static bool visible;
        private static List<IDebugPanel> panels = new();
        private static List<DebugCommand> commands = new();

        private static readonly Vector2 Padding = new(10, 10);
        private const float LineHeight = 18f;

        public static void Initialize()
        {
            panels.Add(new NodePanel());
            panels.Add(new RegionPanel());

            commands.Add(new DebugCommand(Keys.U, () => visible = !visible));
            commands.Add(new DebugCommand(Keys.R, () => Engine.SceneManager.ReloadCurrentScene()));
            commands.Add(new DebugCommand(Keys.T, () => RegionPanel.Enabled = !RegionPanel.Enabled));
        }

        public static void Update()
        {
            foreach (var cmd in commands)
                cmd.Check();

            if (!visible)
                return;

            foreach (var panel in panels)
                panel.Update();
        }

        public static void Draw()
        {
            if (!visible)
                return;

            foreach (var panel in panels)
                panel.Draw();
        }


        private class DebugCommand
        {
            private Keys key;
            private Action action;

            public DebugCommand(Keys key, Action action)
            {
                this.key = key;
                this.action = action;
            }

            public void Check()
            {
                if (Engine.Input.Keyboard.WasKeyJustPressed(key))
                    action();
            }
        }

        private interface IDebugPanel
        {
            void Update();
            void Draw();
        }

        private class NodePanel : IDebugPanel
        {
            private readonly List<(string, Color)> left = new();
            private readonly List<(string, Color)> right = new();

            public void Update()
            {
                int FPS = (int)Math.Round(Engine.FPS);

                Color fpsColor;

                if (FPS >= 60)
                    fpsColor = Color.LimeGreen;
                else if (FPS >= 30)
                    fpsColor = Color.Yellow;
                else
                    fpsColor = Color.Red;

                left.Clear();
                right.Clear();
                
                left.Add(($"Current FPS: {FPS}", fpsColor));
                left.Add(($"Total Nodes: {Node.AllInstances.Count}", Color.Green));
                left.Add(($"Node Types: {Node.AllInstancesDetailed.Count}", Color.Aqua));
                left.Add(($"Current Scene: {Engine.SceneManager.GetCurrentScene()}", Color.LightBlue));
                left.Add(("", Color.White));
                left.Add(($"Player Position: {Engine.MainCharacter?.Location} ", Color.LimeGreen));

                right.Add(("U: Toggle Debug", Color.Yellow));
                right.Add(("R: Reload Scene", Color.Yellow));
                right.Add(("T: Toggle Regions", Color.Yellow));
            }

            public void Draw()
            {
                var edges = MCamera.CurrentCamera.GetScreenEdges();
                DrawLines(left, edges.TopLeft + Padding, alignRight: false);
                DrawLines(right, edges.TopRight - new Vector2(Padding.X, -Padding.Y), alignRight: true);
            }

            private void DrawLines(List<(string text, Color color)> lines, Vector2 pos, bool alignRight)
            {
                foreach (var (text, color) in lines)
                {
                    Vector2 p = pos;

                    if (alignRight)
                    {
                        var size = Engine.Font.MeasureString(text);
                        p.X -= size.X;
                    }

                    DrawHelper.DrawString(text, color, p);
                    pos.Y += LineHeight;
                }
            }
        }

        private class RegionPanel : IDebugPanel
        {
            public static bool Enabled { get; set; }

            public void Update() {}

            public void Draw()
            {
                if (!Enabled)
                    return;

                foreach (var n in Node.AllInstances)
                    n.DrawShapeHollow(Color.Red);
            }
        }
    }
}