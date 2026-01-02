using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monlith.Nodes;
using Monolith.Helpers;
using Monolith.Util;

namespace Monolith.Nodes
{
    public record class RoomCameraConfig : CameraConfig
    {
        [NodeRefference]
        public Node2D TargetNode { get; set; }

        public List<Action> TransitionStarted { get; set; } = new();
        public List<Action> TransitionEnded { get; set; } = new();
    }

    public enum CameraSide
    {
        None,
        Left,
        Right,
        Top,
        Bottom
    }

    public class RoomCamera : Camera2D
    {
        private bool _entered;
        private int _dir;
        
        public Node2D TargetNode { get; set; }

        public List<Action> TransitionStarted { get; set; }
        public List<Action> TransitionEnded { get; set; }

        public RoomCamera(RoomCameraConfig cfg) : base(cfg)
        {
            TargetNode = cfg.TargetNode;
            TransitionStarted = cfg.TransitionStarted;
            TransitionEnded = cfg.TransitionEnded;
        }

        public override void Load()
        {
            base.Load();

            if (TargetNode is KinematicBody2D)
            {
                TransitionStarted.Add(LockBody);
                TransitionEnded.Add(UnlockBody);
            }
        }

        public override void Unload()
        {
            base.Unload();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (TargetNode == null || !TargetNode.HasChildByT<CollisionShape2D>())
                return;

            var targetShape = (CollisionShape2D)TargetNode.GetFirstChildByT<CollisionShape2D>();
            var shape = targetShape.Shape;

            var pos = TargetNode.GlobalPosition;            

            var camera = GetWorldViewRectangle();

            CameraSide side = CameraSide.None;

            if (pos.X + shape.Width > camera.Right)
                side = CameraSide.Right;
            else if (pos.X < camera.Left)
                side = CameraSide.Left;
            else if (pos.Y < camera.Top)
                side = CameraSide.Top;
            else if (pos.Y + shape.Height > camera.Bottom)
                side = CameraSide.Bottom;

            if (!_entered)
            {
                switch (side)
                {
                    case CameraSide.Left:
                        ShiftRoom(-1);
                        break;
                    case CameraSide.Right:
                        ShiftRoom(1);
                        break;
                }
            }

            _entered = side != CameraSide.None;
        }



        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        private void ShiftRoom(int dir)
        {
            Console.WriteLine(dir);
            _dir = dir;

            var camera = GetWorldViewRectangle();

            Vector2 targetPos = new Vector2(LocalPosition.X + camera.Width * dir, LocalPosition.Y);


            foreach (var action in TransitionStarted)
                action?.Invoke();
            
            float startX = LocalPosition.X;
            var cameraXTween = new Tween(
                0.5f,
                EasingFunctions.Linear,
                t => LocalPosition = new Vector2(MathHelper.Lerp(startX, targetPos.X, t), LocalPosition.Y),
                () =>
                {
                    foreach (var action in TransitionEnded)
                        action?.Invoke();
                }
            );
        }

        private void LockBody()
        {
            if (TargetNode is KinematicBody2D body)
            {
                body.Locked = true;
                body.Offset(5 * _dir, 0);
            }
        }

        private void UnlockBody()
        {
            if (TargetNode is KinematicBody2D body)
            {
                body.Locked = false;
            }
        }

    }
}