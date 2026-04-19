using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Monolith.Graphics;
using Monolith.Util;
using Monolith.Managers;
using Monolith.Params;

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

    public static Preferences Prefs { get; set; }

    public static TimeOwner Time { get; private set; }
    public static TreeServer2D Tree { get; private set; }
    public static ResourceManager Resource { get; private set; }
    public static CanvasHandler Canvas { get; private set; }
    public static StageManager Stage { get; private set; }
    public static InputManager Input { get; private set; }
    public static PhysicsServer2D Physics { get; private set; }

    public static float FPS { get; private set; }
    private int _fpsFrames;
    private double _fpsTimer;

    public Engine()
    {
      if (Instance != null)
        throw new InvalidOperationException("Only one Engine instance can exist.");

      Instance = this;
      Graphics = new GraphicsDeviceManager(this)
      {
        PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
        PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height
      };
      Graphics.ApplyChanges();

      Window.AllowUserResizing = true;

      IsFixedTimeStep = false;
      Graphics.SynchronizeWithVerticalRetrace = true;
    }

    protected override void Initialize()
    {

      ClassDB.Initialize(typeof(Engine).Assembly);

      GraphicsDevice = base.GraphicsDevice;

      Time = new TimeOwner(TimeSpan.FromSeconds(1.0 / 60.0));

      Prefs = new Preferences();
      Resource = new ResourceManager();
      Stage = new StageManager();
      Tree = new TreeServer2D();
      Physics = new PhysicsServer2D();
      Input = new InputManager();

      base.Initialize();

      SpriteBatch = new SpriteBatch(GraphicsDevice);
      Pixel = new MTexture(1, 1, new[] { Color.White });

      Canvas = new CanvasHandler(SpriteBatch);

      Canvas.Initialize();
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
      TimeSpan frameDelta = gameTime.ElapsedGameTime;

      int physicsSteps = Time.Update(frameDelta);

      var context = Time.GetContext();

      Input.Update(gameTime);

      Tree.Update(context, physicsSteps);

      if (Input.Keyboard.IsKeyDown(Keys.Escape) || Input.CurrentGamePad.WasButtonJustPressed(Buttons.Start))
        Exit();

      _fpsTimer += (float)frameDelta.TotalSeconds;
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
      GraphicsDevice.SetRenderTarget(Canvas.RenderTarget);
      GraphicsDevice.Clear(Color.CornflowerBlue);

      Canvas.Flush();

      GraphicsDevice.SetRenderTarget(null);
      GraphicsDevice.Clear(Color.Black);

      SpriteBatch.Begin(
          SpriteSortMode.Immediate,
          BlendState.AlphaBlend,
          SamplerState.PointClamp,
          effect: PostProcessingShader);

      SpriteBatch.Draw(Canvas.RenderTarget, Canvas.Destination, Color.White);

      SpriteBatch.End();

      base.Draw(gameTime);
    }

    public static void Quit() => Instance.Exit();
  }
}
