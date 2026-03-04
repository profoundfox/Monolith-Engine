using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Managers;

namespace Monolith.Nodes
{
    public class Camera2D : Node2D
    {
        public Vector2 Zoom { get; set; } = Vector2.One;

        public Camera2D() {}

        /// <summary>
        /// Returns the camera transform matrix for SpriteBatch.Begin.
        /// Centers the camera so LocalPosition maps to the Canvas center.
        /// </summary>
        public Matrix GetTransform() 
        { 

            Vector2 CanvasCenter = new Vector2(Engine.Canvas.RenderTarget.Width / Zoom.X, Engine.Canvas.RenderTarget.Height / Zoom.Y) * 0.5f; 

            Matrix transform = Matrix.CreateScale(Zoom.X, Zoom.Y, 1f)
                * Matrix.CreateRotationZ(GlobalTransform.Rotation)
                * Matrix.CreateTranslation(new Vector3(-GlobalTransform.Position, 0f))
                * Matrix.CreateTranslation(new Vector3(CanvasCenter, 0f)); 
                
            return transform; 
        }

        public override void ProcessUpdate(float delta)
        {
            base.ProcessUpdate(delta);
            
            Engine.Canvas.SetMatrix(GetTransform());
        }
    }
}
