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

            Vector2 screenCenter = new Vector2(Engine.Screen.RenderTarget.Width, Engine.Screen.RenderTarget.Height) * 0.5f; 

            Matrix transform = Matrix.CreateScale(Zoom)
                * Matrix.CreateRotationZ(GlobalTransform.Rotation)
                * Matrix.CreateTranslation(new Vector3(-GlobalTransform.Position, 0f))
                * Matrix.CreateTranslation(new Vector3(screenCenter, 0f)); 
                
            return transform; 
        }

        public override void ProcessUpdate(float delta)
        {
            base.ProcessUpdate(delta);
            
            Engine.Screen.SetMatrix(GetTransform());
        }
    }
}
