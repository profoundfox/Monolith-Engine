using ConstructEngine.Area;
using Microsoft.Xna.Framework;
using System;
using ConstructEngine.Components.Entity;
using ConstructEngine.Helpers;
using ConstructEngine.Util.Tween;

namespace ConstructEngine.Graphics
{
    public class RoomCamera : Camera
    {
        public Matrix Transform { get; private set; }
        private Tween cameraXTween;
        private Tween cameraYTween;
        Vector2 cameraTargetPosition = Vector2.Zero;

        public float LerpFactor { get; set; } = 0.1f;

        public Rectangle CameraRectangle;
        
        private bool Entered = false;

        public RoomCamera(float zoom)
        {
            Zoom = zoom;


            cameraPosition = new Vector2(Engine.VirtualWidth / 2, Engine.VirtualHeight / 2);

            CameraRectangle = new Rectangle
            (
                (int)(cameraPosition.X - (Engine.VirtualWidth / (2 * Zoom))),
                (int)(cameraPosition.Y - (Engine.VirtualHeight / (2 * Zoom))),
                (int)(Engine.VirtualWidth / Zoom),
                (int)(Engine.VirtualHeight / Zoom)
            );
        }
        
        private void UpdateCameraRectangle()
        {
            CameraRectangle.X = (int)(cameraPosition.X - (Engine.VirtualWidth / (2 * Zoom)));
            CameraRectangle.Y = (int)(cameraPosition.Y - (Engine.VirtualHeight / (2 * Zoom)));
            CameraRectangle.Width = (int)(Engine.VirtualWidth / Zoom);
            CameraRectangle.Height = (int)(Engine.VirtualHeight / Zoom);
        }
        
        
        public void Follow(Entity targetEntity)
        {
            if (targetEntity == null) return;
            
            var side = CollisionHelper.GetCameraEdge(targetEntity.KinematicBase.Collider, CameraRectangle);
            
            if (!Entered)
            {
                if (side == CollisionSide.Left)
                {
                    
                    cameraTargetPosition.X = CameraRectangle.X - CameraRectangle.Width + CameraRectangle.Width / 2;
                    CameraRectangle.X -= CameraRectangle.Width;
                    
                    targetEntity.KinematicBase.Locked = true;
                    
                    targetEntity.KinematicBase.Collider.Rect.X -= 10;
                    
                    cameraXTween = new Tween(
                        cameraPosition.X,
                        cameraTargetPosition.X,
                        0.5f,
                        EasingFunctions.Linear
                    );
                }

                if (side == CollisionSide.Right)
                {
                    
                    
                    cameraTargetPosition.X = CameraRectangle.X + CameraRectangle.Width + CameraRectangle.Width / 2;
                    CameraRectangle.X += CameraRectangle.Width;
                    
                    targetEntity.KinematicBase.Locked = true;

                    targetEntity.KinematicBase.Collider.Rect.X += 10;
                    
                    
                    cameraXTween = new Tween(
                        cameraPosition.X,
                        cameraTargetPosition.X,
                        0.5f,
                        EasingFunctions.Linear
                    );
                    
                    

                }

                
                
            }

            if (cameraXTween != null)
            {
                if (cameraXTween.IsFinished())
                {
                    targetEntity.KinematicBase.Locked = false;
                }
            }
            
            if (cameraYTween != null)
            {
                if (cameraYTween.IsFinished())
                {
                    targetEntity.KinematicBase.Locked = false;
                }
            }

            

            Entered = side != CollisionSide.None;
            
            if (cameraXTween != null && !cameraXTween.IsFinished())
            {
                cameraXTween.Update(Engine.DeltaTime);
                cameraPosition.X = cameraXTween.GetCurrentValue(cameraXTween.Normal);
            }

            if (cameraYTween != null && !cameraYTween.IsFinished())
            {
                cameraYTween.Update(Engine.DeltaTime);
                cameraPosition.Y = cameraYTween.GetCurrentValue(cameraYTween.Normal);
            }

            UpdateCameraRectangle();

            var position = Matrix.CreateTranslation(-cameraPosition.X, -cameraPosition.Y, 0f);
            var offset = Matrix.CreateTranslation(Engine.VirtualWidth / 2f, Engine.VirtualHeight / 2f, 0f);
            var scale = Matrix.CreateScale(Zoom, Zoom, 1f);

            Transform = position * scale * offset;
            
            
        }
    }
}