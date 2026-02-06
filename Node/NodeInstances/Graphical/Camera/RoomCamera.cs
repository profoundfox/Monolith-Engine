using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        public override void ProcessUpdate(float delta)
        {
            base.ProcessUpdate(delta);

            if (TargetNode == null || !TargetNode.HasChildByT<CollisionShape2D>())
                return;

            var targetShape = (CollisionShape2D)TargetNode.GetFirstChildByT<CollisionShape2D>();
            var shape = targetShape.Shape;

            var pos = TargetNode.GlobalPosition;            

            var camera = Engine.Screen.GetWorldViewRectangle();

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



        public override void SubmitCall()
        {
            base.SubmitCall();
        }

        private void ShiftRoom(int dir)
        {
            Console.WriteLine(dir);
            _dir = dir;

            var camera = Engine.Screen.GetWorldViewRectangle();

            Vector2 targetPos = new Vector2(LocalPosition.X + camera.Width * dir, LocalPosition.Y);


            foreach (var action in TransitionStarted)
                action?.Invoke();


            var cameraXTween = Engine.Tween.CreateTween(t => LocalPosition = t, LocalPosition, targetPos, 0.5f, Vector2.Lerp, EasingFunctions.Linear);

            cameraXTween.SetCallbackAction
            (
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
                body.Offset(5 * _dir, 0);
            }
        }

        private void UnlockBody()
        {
            if (TargetNode is KinematicBody2D body)
            {
                
            }
        }

    }
}