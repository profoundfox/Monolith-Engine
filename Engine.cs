using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.Diagnostics;
using System.Linq;

using Monolith.Managers;
using Monolith.Graphics;
using Monolith.Nodes;
using Monolith.Helpers;
using Monolith.Util;
using Monolith.IO;
using System.IO.Compression;


namespace Monolith
{
    /// <summary>
    /// The core game engine class that manages the main game loop, rendering, input, scenes, and UI.
    /// Implements a singleton pattern to ensure only one instance exists.
    /// </summary>
    public class Engine : Game
    {
        #region Singleton & Config

        public static Engine Instance { get; private set; }
        public EngineConfig Config { get; }

        #endregion

        #region Graphics

        public static GraphicsDeviceManager Graphics { get; private set; }
        public static new GraphicsDevice GraphicsDevice { get; private set; }
        public static RenderTarget2D RenderTarget { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
        public SpriteFont Font { get; private set; }
        public Effect PostProcessingShader { get; set; }
        public static MTexture Pixel { get; private set; }
        public Point ScreenSize => new Point(RenderTarget.Width, RenderTarget.Height);

        private int _finalWidth, _finalHeight;
        private int _offsetX, _offsetY;
        private float _currentScale;

        #endregion

        #region Managers

        public static TweenManager Tween { get; private set; }
        public static DrawManager Screen { get; private set; }
        public static SceneManager Scene { get; private set; }
        public static InputManager Input { get; private set; }
        public static NodeManager Node { get; private set; }


        #endregion

        #region Content

        public static IContentProvider Resources { get; set; }
        internal ContentManager ContentManager { get; private set; }

        #endregion

        #region Timing & Debug

        public DrawStageType CurrentDrawStage { get; private set; } = DrawStageType.None;
        public float FPS { get; private set; }
        public static float DeltaTime { get; private set; }

        private int _fpsFrames;
        private double _fpsTimer;
        private bool _quit;

        #endregion

        #region Constructor

        public Engine(EngineConfig config)
        {
            if (Instance != null)
                throw new InvalidOperationException("Only one Engine instance can exist.");

            Instance = this;
            Config = config;

            Graphics = new GraphicsDeviceManager(this)
            {
                IsFullScreen = Config.Fullscreen,
            };

            ContentManager = base.Content;
            Content.RootDirectory = Config.RootContentDirectory;

            Resources = Config.Resources;

            Window.AllowUserResizing = Config.AllowUserResizing;
            Window.IsBorderless = Config.IsBorderless;
            IsFixedTimeStep = Config.IsFixedTimeStep;

            if (Config.Maximised)
            {
                Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }

            Graphics.SynchronizeWithVerticalRetrace = Config.SynchronizeWithVerticalRetrace;
            Graphics.ApplyChanges();

            Window.Position = new Point(0, 0);
        }

        #endregion

        #region Initialization

        private void InitializeDebug()
        {
            DebugOverlay.AddInfo("FPS", () => $"Current FPS: {Math.Round(FPS)}", Color.LimeGreen);
            DebugOverlay.AddInfo("NodeCount", () => $"Total Nodes: {Node.AllInstances.Count}", Color.Green);
            DebugOverlay.AddInfo("NodeTypes", () => $"Node Types: {Node.AllInstancesDetailed.Count}", Color.Aqua);
            DebugOverlay.AddInfo("Scene", () => $"Current Scene: {Scene.GetCurrentScene()}", Color.LightBlue);

            DebugOverlay.AddInfo("Spacer", () => "", Color.White);

            DebugOverlay.AddInfo("KeybindU", () => "U: Toggle Debug", Color.Yellow, DebugOverlay.Side.Right);
            DebugOverlay.AddInfo("KeybindR", () => "R: Reload Scene", Color.Yellow, DebugOverlay.Side.Right);
            DebugOverlay.AddInfo("KeybindT", () => "T: Toggle Regions", Color.Yellow, DebugOverlay.Side.Right);

            DebugTools.AddShortcut(Keys.U, () => DebugOverlay.ToggleOverlay());
            DebugTools.AddShortcut(Keys.R, () => Scene.ReloadCurrentScene());
            DebugTools.AddShortcut(Keys.T, () => DebugTools.ToggleRegions());
        }

        protected override void Initialize()
        {
            Tween = new TweenManager();
            Scene = new SceneManager();
            Node = new NodeManager();
            Input = new InputManager();
            Input.InitializeBinds(Config.Actions);

            base.Initialize();

            Window.ClientSizeChanged += (_, _) => UpdateRenderTargetTransform();

            GraphicsDevice = base.GraphicsDevice;
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            Pixel = new MTexture(1, 1, new[] { Color.White });

            Screen = new DrawManager(SpriteBatch);

            if (!string.IsNullOrEmpty(Config.FontPath))
                Font = Content.Load<SpriteFont>(Config.FontPath);

            if (Config.DebugMode)
                InitializeDebug();

            LoadRenderTarget();
            UpdateRenderTargetTransform();
        }

        #endregion

        #region Update & Draw

        protected override void Update(GameTime gameTime)
        {
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Input.Update(gameTime);

            if ((Config.ExitOnEscape && Input.Keyboard.IsKeyDown(Keys.Escape)) || _quit)
                Exit();

            Scene.UpdateCurrentScene(gameTime);

            if (Config.DebugMode)
                DebugTools.Update();

            _fpsTimer += gameTime.ElapsedGameTime.TotalSeconds;
            _fpsFrames++;

            if (_fpsTimer >= 1.0)
            {
                FPS = _fpsFrames / (float)_fpsTimer;
                _fpsFrames = 0;
                _fpsTimer = 0;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            CurrentDrawStage = DrawStageType.Start;

            SetRenderTarget();
            GraphicsDevice.Clear(Config.BackgroundColor);

            CurrentDrawStage = DrawStageType.Scene;

            if (Config.DebugMode)
                DebugOverlay.Draw(SpriteBatch);
            Scene.DrawCurrentScene(SpriteBatch);

            CurrentDrawStage = DrawStageType.DrawManager;
            Screen.Flush();

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            CurrentDrawStage = DrawStageType.PostProcessing;
            SpriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                effect: PostProcessingShader);

            SpriteBatch.Draw(
                RenderTarget,
                new Rectangle(_offsetX, _offsetY, _finalWidth, _finalHeight),
                Color.White);

            SpriteBatch.End();

            CurrentDrawStage = DrawStageType.UI;
            CurrentDrawStage = DrawStageType.End;

            base.Draw(gameTime);

            CurrentDrawStage = DrawStageType.None;
        }

        #endregion

        #region RenderTarget & Scaling

        public void SetRenderTarget() =>
            GraphicsDevice.SetRenderTarget(RenderTarget);

        public void LoadRenderTarget()
        {
            RenderTarget = new RenderTarget2D(
                GraphicsDevice,
                Config.RenderWidth,
                Config.RenderHeight,
                false,
                SurfaceFormat.Color,
                DepthFormat.None);
        }

        public void UpdateRenderTargetTransform()
        {
            var pp = GraphicsDevice.PresentationParameters;

            _currentScale = Math.Min(
                pp.BackBufferWidth / (float)Config.RenderWidth,
                pp.BackBufferHeight / (float)Config.RenderHeight
            );

            if (Config.IntegerScaling)
                _currentScale = Math.Max(1, MathF.Floor(_currentScale));

            _finalWidth = (int)(Config.RenderWidth * _currentScale);
            _finalHeight = (int)(Config.RenderHeight * _currentScale);

            _offsetX = (pp.BackBufferWidth - _finalWidth) / 2;
            _offsetY = (pp.BackBufferHeight - _finalHeight) / 2;
        }

        #endregion

        #region Utilities

        public void ToggleFullscreen()
        {
            Graphics.IsFullScreen = !Graphics.IsFullScreen;
            Graphics.ApplyChanges();
        }

        public static void Quit() => Instance._quit = true;

        #endregion
    }
}
