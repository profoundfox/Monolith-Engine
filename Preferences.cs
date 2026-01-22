using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.IO;

namespace Monolith
{
    public class Preferences
    {
        internal Engine Engine { get; }

        public GraphicsPreferences Graphics { get; }
        public WindowPreferences Window { get; }
        public PerformancePreferences Performance { get; }
        public DebugPreferences Debug { get; }
        public ResourcePreferences Resources { get; }

        internal Preferences(Engine engine)
        {
            Engine = engine;

            Graphics = new GraphicsPreferences(engine);
            Window = new WindowPreferences(engine);
            Performance = new PerformancePreferences(engine);
            Debug = new DebugPreferences();
            Resources = new ResourcePreferences(engine);
        }

        internal void Initialize()
        {
            Resources.Apply();
            Window.Apply();
            Performance.Apply();
            Graphics.Apply();
        }
    }

    public class GraphicsPreferences
    {
        private readonly Engine _engine;

        private bool _fullscreen;

        public int RenderWidth { get; set; } = 640;
        public int RenderHeight { get; set; } = 360;
        public bool IntegerScaling { get; set; } = true;
        public Color BackgroundColor { get; set; } = Color.DarkSlateGray;

        public bool Fullscreen
        {
            get => _fullscreen;
            set
            {
                if (_fullscreen == value) return;
                _fullscreen = value;

                Engine.Graphics.IsFullScreen = value;
                Engine.Graphics.ApplyChanges();
            }
        }

        public Rectangle Destination { get; private set; }

        internal GraphicsPreferences(Engine engine)
        {
            _engine = engine;
        }

        internal void Apply()
        {
            CreateRenderTarget();
            UpdateTransform();
            Fullscreen = _fullscreen;
        }

        internal void CreateRenderTarget()
        {
            Engine.RenderTarget?.Dispose();

            Engine.RenderTarget = new RenderTarget2D(
                Engine.GraphicsDevice,
                RenderWidth,
                RenderHeight,
                false,
                SurfaceFormat.Color,
                DepthFormat.None);
        }

        internal void UpdateTransform()
        {
            var pp = Engine.GraphicsDevice.PresentationParameters;

            float scale = Math.Min(
                pp.BackBufferWidth / (float)RenderWidth,
                pp.BackBufferHeight / (float)RenderHeight);

            if (IntegerScaling)
                scale = Math.Max(1, MathF.Floor(scale));

            int w = (int)(RenderWidth * scale);
            int h = (int)(RenderHeight * scale);

            int x = (pp.BackBufferWidth - w) / 2;
            int y = (pp.BackBufferHeight - h) / 2;

            Destination = new Rectangle(x, y, w, h);
        }
    }

    public class WindowPreferences
    {
        private readonly Engine _engine;

        public string Title { get; set; } = "My Game";
        public bool AllowUserResizing { get; set; } = true;
        public bool Borderless { get; set; } = true;
        public bool MouseVisible { get; set; }
        public bool Maximised { get; set; } = true;

        internal WindowPreferences(Engine engine)
        {
            _engine = engine;
        }

        internal void Apply()
        {
            _engine.Window.Title = Title;
            _engine.Window.AllowUserResizing = AllowUserResizing;
            _engine.Window.IsBorderless = Borderless;
            _engine.IsMouseVisible = MouseVisible;

            if (Maximised)
            {
                var mode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
                Engine.Graphics.PreferredBackBufferWidth = mode.Width;
                Engine.Graphics.PreferredBackBufferHeight = mode.Height;
                Engine.Graphics.ApplyChanges();
            }
        }
    }

    public class PerformancePreferences
    {
        private readonly Engine _engine;

        public bool FixedTimeStep { get; set; }
        public bool VSync { get; set; } = true;

        internal PerformancePreferences(Engine engine)
        {
            _engine = engine;
        }

        internal void Apply()
        {
            _engine.IsFixedTimeStep = FixedTimeStep;
            Engine.Graphics.SynchronizeWithVerticalRetrace = VSync;
            Engine.Graphics.ApplyChanges();
        }
    }

    public class DebugPreferences
    {
        public bool ExitOnEscape { get; set; }
    }

    public class ResourcePreferences
    {
        private readonly Engine _engine;

        public string RootDirectory { get; set; } = "Content";
        public IContentProvider ContentManager { get; set; } = new ContentPipelineLoader();

        internal ResourcePreferences(Engine engine)
        {
            _engine = engine;
        }

        internal void Apply()
        {
            _engine.Content.RootDirectory = RootDirectory;
            Engine.ContentManager = ContentManager;

        }
    }

}
