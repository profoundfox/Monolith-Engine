using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Gum.Wireframe;
using RenderingLibrary;
using MonoGameGum;

using Monolith.UI;
using Monolith.Managers;
using Monolith.Graphics;
using Monolith.Nodes;
using Monolith.Helpers;
using Monolith.Util;
using System.Diagnostics;

namespace Monolith
{
    /// <summary>
    /// The core game engine class that manages the main game loop, rendering, input, scenes, and UI.
    /// Implements a singleton pattern to ensure only one instance exists.
    /// </summary>
    public class Engine : Game
    {
        /// <summary>
        /// Singleton instance of the engine.
        /// </summary>
        public static Engine Instance { get; private set; }

        /// <summary>
        /// Configuration for the engine, such as screen size, fullscreen mode, and content directories.
        /// </summary>
        public EngineConfig Config { get; }

        /// <summary>
        /// Manages the graphics device and display settings.
        /// </summary>
        public static GraphicsDeviceManager Graphics { get; private set; }

        /// <summary>
        /// The main graphics device used for rendering.
        /// </summary>
        public static new GraphicsDevice GraphicsDevice { get; private set; }

        /// <summary>
        /// Render target used for rendering the scene before applying post-processing or scaling.
        /// </summary>
        public static RenderTarget2D RenderTarget { get; private set; }

        /// <summary>
        /// Manager responsible for handling tweens and animations.
        /// </summary>
        public static TweenManager TweenManager { get; private set; }

        /// <summary>
        /// Manager responsible for handling sprite rendering.
        /// </summary>
        public static SpriteManager SpriteManager { get; private set; }

        /// <summary>
        /// Manager responsible for draw logic.
        /// </summary>
        public static DrawManager DrawManager {get; private set; }

        /// <summary>
        /// Content manager used for loading assets such as textures, fonts, and effects.
        /// </summary>
        public static new ContentManager Content { get; private set; }

        /// <summary>
        /// The main SpriteBatch used for drawing sprites.
        /// </summary>
        public static SpriteBatch SpriteBatch { get; private set; }

        /// <summary>
        /// Default font loaded for the engine.
        /// </summary>
        public static SpriteFont Font { get; private set; }

        /// <summary>
        /// Optional post-processing shader applied when rendering the scene.
        /// </summary>
        public Effect PostProcessingShader { get; set; }

        /// <summary>
        /// The main player character or kinematic entity in the scene.
        /// </summary>
        public static Node MainCharacter { get; set; }

        /// <summary>
        /// Current FPS.
        /// </summary>
        public static float FPS { get; private set; }

        /// <summary>
        /// Manager responsible for scene handling and switching between scenes.
        /// </summary>
        public static SceneManager SceneManager { get; private set; }

        /// <summary>
        /// Manager handling input from keyboard, mouse, and other devices.
        /// </summary>
        public static InputManager Input { get; private set; }

        /// <summary>
        /// Gum UI service for managing user interfaces built with Gum.
        /// </summary>
        public GumService GumUI { get; private set; }

        /// <summary>
        /// Time elapsed since the last frame, in seconds.
        /// </summary>
        public static float DeltaTime { get; private set; }

        private int finalWidth, finalHeight;
        private int offsetX, offsetY;
        private float currentScale;
        private bool quit;
        private int _fpsFrames;
        private bool drawRegions;
        private double _fpsTimer;


        /// <summary>
        /// Initializes a new instance of the Engine class with the specified configuration.
        /// </summary>
        /// <param name="config">Engine configuration settings.</param>
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

            Content = base.Content;
            Content.RootDirectory = Config.RootContentDirectory;

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

        private void InitializeDebug()
        {
            DebugOverlay.AddInfo("FPS", () => $"Current FPS: {Math.Round(FPS)}", Color.LimeGreen);
            DebugOverlay.AddInfo("NodeCount", () => $"Total Nodes: {NodeManager.AllInstances.Count}", Color.Green);
            DebugOverlay.AddInfo("NodeTypes", () => $"Node Types: {NodeManager.AllInstancesDetailed.Count}", Color.Aqua);
            DebugOverlay.AddInfo("Scene", () => $"Current Scene: {SceneManager.GetCurrentScene()}", Color.LightBlue);

            DebugOverlay.AddInfo("Spacer", () => "", Color.White);

            DebugOverlay.AddInfo("PlayerPosition", () => NodeManager.GetNodeByName("Player") is var p2 && p2 != null ? $"Player Position: {p2.Location}" : "Player Position: <null>", Color.Aqua);
            
            DebugOverlay.AddInfo("Spacer", () => "", Color.White);

            DebugOverlay.AddInfo("KeybindU", () => "U: Toggle Debug", Color.Yellow, DebugOverlay.Side.Right);
            DebugOverlay.AddInfo("KeybindR", () => "R: Reload Scene", Color.Yellow, DebugOverlay.Side.Right);
            DebugOverlay.AddInfo("KeybindT", () => "T: Toggle Regions", Color.Yellow, DebugOverlay.Side.Right);
        }


        /// <summary>
        /// Initializes engine components, input, Gum UI, and render targets.
        /// </summary>
        protected override void Initialize()
        {
            TweenManager = new TweenManager();
            SceneManager = new SceneManager();
            SpriteManager = new SpriteManager();

            Input = new InputManager();
            Input.InitializeBinds(DefaultInput.Binds);

            base.Initialize();

            Window.ClientSizeChanged += (_, _) =>
            {
                UpdateRenderTargetTransform();
                UpdateGumCamera();
            };

            GraphicsDevice = base.GraphicsDevice;
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            DrawManager = new DrawManager(SpriteBatch);

            if (!string.IsNullOrEmpty(Config.FontPath))
                Font = Content.Load<SpriteFont>(Config.FontPath);

            if (!string.IsNullOrEmpty(Config.GumProject))
                InitializeGum(Config.GumProject);
            if (Config.DebugMode)
                InitializeDebug();
            
            LoadRenderTarget();
            UpdateRenderTargetTransform();
        }

        /// <summary>
        /// Updates the engine state each frame, including input, tweens, scenes, and UI.
        /// </summary>
        /// <param name="gameTime">Time elapsed since last update.</param>
        protected override void Update(GameTime gameTime)
        {   
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Input.Update(gameTime);

            if ((Config.ExitOnEscape && Input.Keyboard.IsKeyDown(Keys.Escape)) || quit)
                Exit();

            
            SceneManager.UpdateCurrentScene(gameTime);
            GumManager.UpdateAll(gameTime);
            GumUI?.Update(this, gameTime);


            _fpsTimer += gameTime.ElapsedGameTime.TotalSeconds;
            _fpsFrames++;

            if (_fpsTimer >= 1.0)
            {
                FPS =_fpsFrames / (float)_fpsTimer;
                _fpsFrames = 0;
                _fpsTimer = 0;
            }

            
            if (MCamera.CurrentCamera != null)
                DrawManager.SetCamera(MCamera.CurrentCamera.Transform);

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the current frame, including the scene, render target, post-processing, and UI.
        /// </summary>
        /// <param name="gameTime">Time elapsed since last draw.</param>
        protected override void Draw(GameTime gameTime)
        {
            SetRenderTarget();

            GraphicsDevice.Clear(Color.DarkSlateGray);

            DebugOverlay.Draw(SpriteBatch);
            
            SceneManager.DrawCurrentScene(SpriteBatch);
            
            if (drawRegions)
                foreach(var node in NodeManager.AllInstances) node.DrawShapeHollow(Color.Red);

            DrawManager.Flush();

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            SpriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                effect: PostProcessingShader);

            SpriteBatch.Draw(
                RenderTarget,
                new Rectangle(offsetX, offsetY, finalWidth, finalHeight),
                Color.White);

            SpriteBatch.End();

            GumUI?.Draw();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Sets the current render target to the engine's RenderTarget2D.
        /// </summary>
        public void SetRenderTarget() =>
            GraphicsDevice.SetRenderTarget(RenderTarget);

        /// <summary>
        /// Creates a new render target with the configured width and height.
        /// </summary>
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

        /// <summary>
        /// Updates the transform and scaling for rendering to handle different window sizes and integer scaling.
        /// </summary>
        public void UpdateRenderTargetTransform()
        {
            var pp = GraphicsDevice.PresentationParameters;

            currentScale = Math.Min(
                pp.BackBufferWidth / (float)Config.RenderWidth,
                pp.BackBufferHeight / (float)Config.RenderHeight
            );

            if (Config.IntegerScaling)
                currentScale = Math.Max(1, MathF.Floor(currentScale));

            finalWidth = (int)(Config.RenderWidth * currentScale);
            finalHeight = (int)(Config.RenderHeight * currentScale);

            offsetX = (pp.BackBufferWidth - finalWidth) / 2;
            offsetY = (pp.BackBufferHeight - finalHeight) / 2;
        }

        /// <summary>
        /// Initializes the Gum UI system using the specified project.
        /// </summary>
        /// <param name="gumProject">Path to the Gum project file.</param>
        public void InitializeGum(string gumProject)
        {
            GumUI = GumHelper.GumInitialize(this, gumProject);
            UpdateRenderTargetTransform();
            UpdateGumCamera();
        }  

        /// <summary>
        /// Updates the Gum UI camera based on current render target scale and offsets.
        /// </summary>
        public void UpdateGumCamera()
        {
            if (GumUI == null) return;

            var cam = SystemManagers.Default.Renderer.Camera;
            cam.Zoom = currentScale;
            cam.X = -offsetX / currentScale;
            cam.Y = -offsetY / currentScale;

            GraphicalUiElement.CanvasWidth = Config.RenderWidth;
            GraphicalUiElement.CanvasHeight = Config.RenderHeight;

            GumHelper.UpdateScreenLayout();
        }

        /// <summary>
        /// Toggles fullscreen mode on or off.
        /// </summary>
        public void ToggleFullscreen()
        {
            Graphics.IsFullScreen = !Graphics.IsFullScreen;
            Graphics.ApplyChanges();
        }

        /// <summary>
        /// Signals the engine to quit on the next update cycle.
        /// </summary>
        public static void Quit() => Instance.quit = true;
    }
}
