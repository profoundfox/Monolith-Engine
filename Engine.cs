using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monolith.Geometry;
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

        public static SpriteBatch SpriteBatch { get; private set; }
        public static SpriteFont Font { get; private set; }
        public static BitmapFont BitmapFont { get; private set; }
        public static Effect PostProcessingShader { get; set; }
        public static MTexture Pixel { get; private set; }
        public static EngineTime Time { get; private set; }
        
        public static TweenManager Tween { get; private set; }
        public static ResourceManager Resource { get; private set; }
        public static ScreenManager Screen { get; private set; }
        public static StageManager Stage { get; private set; }
        public static InputManager Input { get; private set; }
        public static NodeManager Node { get; private set; }
        public static TimerManager Timer { get; private set; }

        public static float FPS { get; private set; }
        private int _fpsFrames;
        private double _fpsTimer;

        public static float DeltaTime { get; private set; }

        public Engine()
        {
            if (Instance != null)
                throw new InvalidOperationException("Only one Engine instance can exist.");

            Instance = this;
            Graphics = new GraphicsDeviceManager(this);

            Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            Graphics.ApplyChanges();

            Window.AllowUserResizing = true;
            IsFixedTimeStep = false;
            Graphics.SynchronizeWithVerticalRetrace = true;
        }

        protected override void Initialize()
        {
            GraphicsDevice = base.GraphicsDevice;

            Time = new EngineTime(1f / 60f);

            Resource = new ResourceManager();
            Tween = new TweenManager();
            Stage = new StageManager();
            Node = new NodeManager();
            Timer = new TimerManager();
            Input = new InputManager();

            base.Initialize();

            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Pixel = new MTexture(1, 1, new[] { Color.White });

            Screen = new ScreenManager(SpriteBatch);

            Screen.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

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
            float frameDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            DeltaTime = frameDelta;

            int physicsSteps = Time.Update(frameDelta);

            Input.Update(gameTime);

            Tween.Update(frameDelta);

            for (int i = 0; i < physicsSteps; i++)
            {
                Timer.PhysicsUpdate(Time.FixedDelta);
                Stage.PhysicsUpdate(Time.FixedDelta);
            }

            Stage.ProcessUpdate(Time.FrameDelta);
            Stage.SubmitCallCurrentStage();

            if (Input.Keyboard.IsKeyDown(Keys.Escape))
                Exit();

            _fpsTimer += frameDelta;
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
            GraphicsDevice.SetRenderTarget(Screen.RenderTarget);
            GraphicsDevice.Clear(Color.Black);

            Screen.Flush();

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            SpriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                effect: PostProcessingShader);

            SpriteBatch.Draw(Screen.RenderTarget, Screen.Destination, Color.White);

            SpriteBatch.End();

            base.Draw(gameTime);
        }

        public static void Quit() => Instance.Exit();
    }
}
