using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using ConstructEngine.Input;
using ConstructEngine.Graphics;
using ConstructEngine.UI;
using ConstructEngine.Area;
using ConstructEngine.Components;
using ConstructEngine.Util;

using Gum.Wireframe;
using RenderingLibrary;
using MonoGameGum;
using ConstructEngine.Objects;
using ConstructEngine.Util.Tween;

namespace ConstructEngine
{
    public class Engine : Game
    {
        public static Engine Instance { get; private set; }

        public static GraphicsDeviceManager Graphics { get; private set; }
        public static new GraphicsDevice GraphicsDevice { get; private set; }
        public static RenderTarget2D RenderTarget { get; private set; }
        public static TweenManager TweenManager {get; private set;}
        public static new ContentManager Content { get; private set; }
        public static SpriteBatch SpriteBatch { get; private set; }
        public static SpriteFont Font { get; private set; }
        public Effect PostProcessingShader { get; set; }
        public static KinematicEntity MainCharacter {get; set;}
        public static SceneManager SceneManager { get; private set; }
        public static InputManager Input { get; private set; }
        public GumService GumUI { get; private set; }
        

        public static string Title;
        public static int VirtualWidth;
        public static int VirtualHeight;
        public static bool Fullscreen;
        public static bool IntegerScaling;
        public static bool AllowUserResizing;
        public static bool IsBorderless;
        public static bool SynchronizeWithVerticalRetrace;
        public static bool ExitOnEscape;
        public static string FontPath;
        public static string GumProject;

        public static float DeltaTime { get; private set; }

        private int finalWidth, finalHeight;
        private int offsetX, offsetY;
        private float currentScale;

        private static bool quit;


        public Engine(EngineConfig config)
        {
            if (Instance != null)
                throw new InvalidOperationException("Only one Engine instance can exist.");

            Instance = this;

            Title = config.Title;
            VirtualWidth = config.VirtualWidth;
            VirtualHeight = config.VirtualHeight;
            Fullscreen = config.Fullscreen;
            IntegerScaling = config.IntegerScaling;
            AllowUserResizing = config.AllowUserResizing;
            IsBorderless = config.IsBorderless;
            IsFixedTimeStep = config.IsFixedTimeStep;
            IsMouseVisible = config.IsMouseVisible;
            SynchronizeWithVerticalRetrace = config.SynchronizeWithVerticalRetrace;
            ExitOnEscape = config.ExitOnEscape;
            FontPath = config.FontPath;
            GumProject = config.GumProject;


            Graphics = new GraphicsDeviceManager(this)
            {
                IsFullScreen = Fullscreen,
            };

            Content = base.Content;
            Content.RootDirectory = config.RootContentDirectory;


            Window.AllowUserResizing = AllowUserResizing;
            Window.IsBorderless = IsBorderless;

            Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            Graphics.SynchronizeWithVerticalRetrace = SynchronizeWithVerticalRetrace;
            Graphics.ApplyChanges();

            Window.Position = new Point(0, 0);
        }

        protected override void Initialize()
        {   
            TweenManager = new TweenManager();
            SceneManager = new SceneManager();
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

            if (!string.IsNullOrEmpty(FontPath))
                Font = Content.Load<SpriteFont>(FontPath);

            if (!string.IsNullOrEmpty(GumProject))
                InitializeGum(GumProject);

            LoadRenderTarget();
            UpdateRenderTargetTransform();
        }

        protected override void Update(GameTime gameTime)
        {
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Input.Update(gameTime);

            if ((ExitOnEscape && Input.Keyboard.IsKeyDown(Keys.Escape)) || quit)
                Exit();

            TweenManager.Update();
            SceneManager.UpdateCurrentScene(gameTime);
            GumManager.UpdateAll(gameTime);
            GumUI?.Update(this, gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            SetRenderTarget();

            GraphicsDevice.Clear(Color.DarkSlateGray);
            SceneManager.DrawCurrentScene(SpriteBatch);

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

        public void SetRenderTarget() =>
            GraphicsDevice.SetRenderTarget(RenderTarget);

        public void LoadRenderTarget()
        {
            RenderTarget = new RenderTarget2D(
                GraphicsDevice,
                VirtualWidth,
                VirtualHeight,
                false,
                SurfaceFormat.Color,
                DepthFormat.None);
        }

        public void UpdateRenderTargetTransform()
        {
            var pp = GraphicsDevice.PresentationParameters;

            currentScale = Math.Min(
                pp.BackBufferWidth / (float)VirtualWidth,
                pp.BackBufferHeight / (float)VirtualHeight
            );

            if (IntegerScaling)
                currentScale = Math.Max(1, MathF.Floor(currentScale));

            finalWidth = (int)(VirtualWidth * currentScale);
            finalHeight = (int)(VirtualHeight * currentScale);

            offsetX = (pp.BackBufferWidth - finalWidth) / 2;
            offsetY = (pp.BackBufferHeight - finalHeight) / 2;
        }

        public void InitializeGum(string gumProject)
        {
            GumUI = GumHelper.GumInitialize(this, gumProject);
            UpdateRenderTargetTransform();
            UpdateGumCamera();
        }

        public void UpdateGumCamera()
        {
            if (GumUI == null) return;

            var cam = SystemManagers.Default.Renderer.Camera;
            cam.Zoom = currentScale;
            cam.X = -offsetX / currentScale;
            cam.Y = -offsetY / currentScale;

            GraphicalUiElement.CanvasWidth = VirtualWidth;
            GraphicalUiElement.CanvasHeight = VirtualHeight;

            GumHelper.UpdateScreenLayout();
        }

        public void ToggleFullscreen()
        {
            Fullscreen = !Fullscreen;
            Graphics.IsFullScreen = Fullscreen;
            Graphics.ApplyChanges();
        }

        public static void ClearSceneLists()
        {
            ParallaxBackground.BackgroundList.Clear();
            Tilemap.Tilemaps.Clear();
            KinematicEntity.EntityList.Clear();
            Area2D.AreaList.Clear();
            ConstructObject.ObjectList.Clear();
            Ray2D.RayList.Clear();
        }

        public static void Quit() => quit = true;
    }
}
