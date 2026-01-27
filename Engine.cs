using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Monolith.Graphics;
using Monolith.IO;
using Monolith.Managers;

namespace Monolith
{
    public class Engine : Game
    {
        public static Engine Instance { get; set; }

        public static GraphicsDeviceManager Graphics { get; private set; }
        public static new GraphicsDevice GraphicsDevice { get; private set; }
        public static RenderTarget2D RenderTarget { get; internal set; }

        public static SpriteBatch SpriteBatch { get; private set; }
        public static SpriteFont Font { get; private set; }
        public static BitmapFont BitmapFont { get; private set; }
        public static Effect PostProcessingShader { get; set; }
        public static MTexture Pixel { get; private set; }

        public static TweenManager Tween { get; private set; }
        public static ScreenManager Screen { get; private set; }
        public static StageManager Stage { get; private set; }
        public static InputManager Input { get; private set; }
        public static NodeManager Node { get; private set; }
        public static TimerManager Timer { get; private set; }

        public static float FPS { get; private set; }
        private int _fpsFrames;
        private double _fpsTimer;

        public static float DeltaTime { get; private set; }

        public static IContentProvider ContentManager { get; set; }

        internal Rectangle Destination { get; set; }
        internal int RenderWidth { get; set; } = 640;
        internal int RenderHeight { get; set; } = 360;
        internal bool IntegerScaling { get; set; } = true;
        internal string ContentRoot { get; set; } = "Content";

        public Engine()
        {
            if (Instance != null)
                throw new InvalidOperationException("Only one Engine instance can exist.");

            Instance = this;
            Graphics = new GraphicsDeviceManager(this);

            Graphics.PreferredBackBufferWidth = 640;
            Graphics.PreferredBackBufferHeight = 360;
            Graphics.ApplyChanges();

            Window.AllowUserResizing = true;
            IsFixedTimeStep = true;
            Graphics.SynchronizeWithVerticalRetrace = true;
        }

        protected override void Initialize()
        {
            GraphicsDevice = base.GraphicsDevice;

            Content.RootDirectory = "Content";

            Tween = new TweenManager();
            Stage = new StageManager();
            Node = new NodeManager();
            Timer = new TimerManager();
            Input = new InputManager();

            base.Initialize();

            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Pixel = new MTexture(1, 1, new[] { Color.White });

            Screen = new ScreenManager(SpriteBatch);

                        
            CreateRenderTarget();
            Window.ClientSizeChanged += (_, _) => UpdateTransform();
            UpdateTransform();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            ContentManager = new ContentPipelineLoader();

            var assembly = typeof(Engine).Assembly;
            using var stream = assembly.GetManifestResourceStream("Monolith.Graphics.Font.bitmap_font.png");
            if (stream == null)
                throw new InvalidOperationException("Embedded resource not found: Monolith.Graphics.Font.bitmap_font.png");

            var texture = Texture2D.FromStream(GraphicsDevice, stream);
            BitmapFont = new BitmapFont(texture, 6, 10);
            BitmapFont.AddMap("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+-=()[]{}<>/*:#%!?.,'\"@&$");
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
            BitmapFont.Texture.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Tween.Update(gameTime);
            Timer.Update(gameTime);
            Input.Update(gameTime);

            if (Input.Keyboard.IsKeyDown(Keys.Escape))
                Exit();

            Stage.UpdateCurrentStage(gameTime);

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
            GraphicsDevice.SetRenderTarget(RenderTarget);
            GraphicsDevice.Clear(Color.MonoGameOrange);

            Stage.DrawCurrentStage(SpriteBatch);

            Screen.Flush();

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            SpriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                effect: PostProcessingShader);

            SpriteBatch.Draw(RenderTarget, Destination, Color.White);

            SpriteBatch.End();

            base.Draw(gameTime);
        }

        internal void CreateRenderTarget()
        {
            RenderTarget?.Dispose();

            RenderTarget = new RenderTarget2D(
                GraphicsDevice,
                RenderWidth,
                RenderHeight,
                false,
                SurfaceFormat.Color,
                DepthFormat.None);
        }

        internal void UpdateTransform()
        {
            var pp = GraphicsDevice.PresentationParameters;

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

        public static void Quit() => Instance.Exit();
    }
}
