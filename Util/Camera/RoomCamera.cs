using Microsoft.Xna.Framework;
using System;

using ConstructEngine.Helpers;
using ConstructEngine.Util;
using ConstructEngine.Nodes;

namespace ConstructEngine.Graphics
{
    public class RoomCamera : CTCamera
    {
        private Tween cameraXTween;
        private Tween cameraYTween;
        Vector2 cameraTargetPosition = Vector2.Zero;

        public float LerpFactor { get; set; } = 0.1f;

        public Rectangle CameraRectangle;
        
        private bool Entered = false;

        public RoomCamera(float zoom)
        {
            var cfg = Engine.Instance.Config;
            
            Zoom = zoom;


            cameraPosition = new Vector2(cfg.RenderWidth / 2, cfg.RenderHeight / 2);

            CameraRectangle = new Rectangle
            (
                (int)(cameraPosition.X - (cfg.RenderWidth / (2 * Zoom))),
                (int)(cameraPosition.Y - (cfg.RenderHeight / (2 * Zoom))),
                (int)(cfg.RenderWidth / Zoom),
                (int)(cfg.RenderHeight / Zoom)
            );
        }
        
        private void UpdateCameraRectangle()
        {
            var cfg = Engine.Instance.Config;

            CameraRectangle.X = (int)(cameraPosition.X - (cfg.RenderWidth / (2 * Zoom)));
            CameraRectangle.Y = (int)(cameraPosition.Y - (cfg.RenderHeight / (2 * Zoom)));
            CameraRectangle.Width = (int)(cfg.RenderWidth / Zoom);
            CameraRectangle.Height = (int)(cfg.RenderHeight / Zoom);
        }
        
        
        public void Follow(Node targetNode)
        {
            var cfg = Engine.Instance.Config;
            if (targetNode == null) return;

            var side = CollisionHelper.GetCameraEdge(targetNode.Shape, CameraRectangle);

            if (!Entered)
            {
                if (side == CollisionSide.Left)
                {
                    cameraTargetPosition.X = CameraRectangle.X - CameraRectangle.Width + CameraRectangle.Width / 2;
                    CameraRectangle.X -= CameraRectangle.Width;

                    if (targetNode is KinematicBody2D body2D)
                    {
                        body2D.Locked = true;
                        body2D.Shape.Offset(-10, 0);
                    }

                    cameraXTween = new Tween(
                        0.5f,
                        EasingFunctions.Linear,
                        t => cameraPosition.X = MathHelper.Lerp(cameraPosition.X, cameraTargetPosition.X, t),
                        () =>
                        {
                            if (targetNode is KinematicBody2D body2D)
                                body2D.Locked = false;
                        }

                    );
                    
                    cameraXTween.Start();
                    Engine.TweenManager.AddTween(cameraXTween);
                }

                if (side == CollisionSide.Right)
                {
                    cameraTargetPosition.X = CameraRectangle.X + CameraRectangle.Width + CameraRectangle.Width / 2;
                    CameraRectangle.X += CameraRectangle.Width;
                    
                    if (targetNode is KinematicBody2D body2D)
                    {
                        body2D.Locked = true;
                        body2D.Shape.Offset(10, 0);
                    }

                    cameraXTween = new Tween(
                        0.5f,
                        EasingFunctions.Linear,
                        t => cameraPosition.X = MathHelper.Lerp(cameraPosition.X, cameraTargetPosition.X, t),
                        () =>
                        {
                            if (targetNode is KinematicBody2D body2D)
                                body2D.Locked = false;
                        }
                    );
                    cameraXTween.Start();
                    Engine.TweenManager.AddTween(cameraXTween);
                }
            }

            Entered = side != CollisionSide.None;

            UpdateCameraRectangle();

            var position = Matrix.CreateTranslation(-cameraPosition.X, -cameraPosition.Y, 0f);
            var offset = Matrix.CreateTranslation(cfg.RenderWidth / 2f, cfg.RenderHeight / 2f, 0f);
            var scale = Matrix.CreateScale(Zoom, Zoom, 1f);

            Transform = position * scale * offset;
        }

    }
}