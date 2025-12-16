using Microsoft.Xna.Framework;
using System;
using Monolith.Helpers;
using Monolith.Util;
using Monolith.Nodes;
using Monolith.Geometry;

namespace Monolith.Util
{
    public class RoomCamera : MCamera
    {
        private Tween cameraXTween;

        public Rectangle CameraRectangle;
        public float LerpFactor { get; set; } = 0.1f;
        public float PosAdd = 10f;

        private bool entered = false;
        private Vector2 targetPosition = Vector2.Zero;

        public RoomCamera(float zoom, Vector2 startPos)
        {
            Zoom = zoom;

            Position = startPos;
            
            UpdateCameraRectangle();
        }

                
        public RoomCamera(float zoom)
        {
            Zoom = zoom;

            Position = Vector2.Zero;

            UpdateCameraRectangle();
        }

        private void UpdateCameraRectangle()
        {
            var cfg = Engine.Instance.Config;

            CameraRectangle = new Rectangle(
                (int)(Position.X - cfg.RenderWidth * 0.5f / Zoom),
                (int)(Position.Y - cfg.RenderHeight * 0.5f / Zoom),
                (int)(cfg.RenderWidth / Zoom),
                (int)(cfg.RenderHeight / Zoom)
            );
        }

        public void Follow(KinematicBody2D body)
        {
            if (body == null)
                return;

            var side = CollisionHelper.GetCameraEdge(
                body.CollisionShape2D.Shape,
                CameraRectangle
            );

            if (!entered)
            {
                switch (side)
                {
                    case CollisionSide.Left:
                        ShiftRoom(body, -1);
                        break;

                    case CollisionSide.Right:
                        ShiftRoom(body, +1);
                        break;
                }
            }

            entered = side != CollisionSide.None;

            UpdateCameraRectangle();
        }

        private void ShiftRoom(KinematicBody2D body, int dir)
        {
            CameraRectangle.X += dir * CameraRectangle.Width;
            targetPosition.X = CameraRectangle.X + CameraRectangle.Width / 2f;

            body.Locked = true;
            body.Position += new Vector2(PosAdd * dir, 0);

            cameraXTween = new Tween(
                0.5f,
                EasingFunctions.Linear,
                t => Position.X = MathHelper.Lerp(Position.X, targetPosition.X, t),
                () => body.Locked = false
            );

            cameraXTween.Start();
            Engine.TweenManager.AddTween(cameraXTween);
        }
    }

}
