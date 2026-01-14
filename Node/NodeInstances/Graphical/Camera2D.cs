using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Managers;

namespace Monolith.Nodes
{
    public record class CameraConfig : SpatialNodeConfig
    {
        public float Zoom { get; set; } = 1f;
    }

    public class Camera2D : Node2D
    {
        public float Zoom { get; set; } = 1f;

        public static Camera2D CurrentCameraInstance { get; private set; }

        public Camera2D(CameraConfig cfg) : base(cfg)
        {
            Zoom = cfg.Zoom;
            CurrentCameraInstance = this;
        }

        /// <summary>
        /// Returns the camera transform matrix for SpriteBatch.Begin.
        /// Centers the camera so LocalPosition maps to the screen center.
        /// </summary>
        public Matrix GetTransform() 
        { 
            var cfg = Engine.Instance.Config; 

            Vector2 screenCenter = new Vector2(cfg.RenderWidth, cfg.RenderHeight) * 0.5f; 


            Matrix transform = Matrix.CreateScale(Zoom)
                * Matrix.CreateRotationZ(GlobalTransform.Rotation)
                * Matrix.CreateTranslation(new Vector3(-GlobalTransform.Position, 0f))
                * Matrix.CreateTranslation(new Vector3(screenCenter, 0f)); 
                
            return transform; 
        }




        /// <summary>
        /// Returns the rectangle of world space currently visible by this camera
        /// </summary>
        public Rectangle GetWorldViewRectangle()
        {
            var cfg = Engine.Instance.Config;

            Matrix inverse = Matrix.Invert(GetTransform());

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

        public override void ProcessUpdate(GameTime gameTime)
        {
            base.ProcessUpdate(gameTime);

            Engine.Screen.SetCamera(GetTransform());
        }
    }
}
