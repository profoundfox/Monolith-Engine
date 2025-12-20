using Microsoft.Xna.Framework;
using Monolith.Managers;

namespace Monolith.Util
{
    public class MCamera
    {
        public Vector2 Position;
        public float Zoom { get; set; } = 1f;
        public static MCamera CurrentCameraInstance { get; set; }

        public Matrix Transform
        {
            get
            {
                var cfg = Engine.Instance.Config;

                return
                    Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0f)) *
                    Matrix.CreateScale(Zoom) *
                    Matrix.CreateTranslation(new Vector3(cfg.RenderWidth * 0.5f, cfg.RenderHeight * 0.5f, 0f));
            }
        }


        public Matrix GetTransform() => Transform;

        public MCamera()
        {
            CurrentCameraInstance = this;
        }

        public void Update(GameTime gameTime)
        {
            Engine.DrawManager.SetCamera(Transform);
        }

        public Rectangle GetWorldViewRectangle()
        {
            var cfg = Engine.Instance.Config;

            Matrix inverse = Matrix.Invert(Transform);

            Vector2 topLeft = Vector2.Transform(Vector2.Zero, inverse);
            Vector2 bottomRight = Vector2.Transform(
                new Vector2(cfg.RenderWidth, cfg.RenderHeight),
                inverse
            );

            return new Rectangle(
                (int)topLeft.X,
                (int)topLeft.Y,
                (int)(bottomRight.X - topLeft.X),
                (int)(bottomRight.Y - topLeft.Y)
            );
        }

    }
}