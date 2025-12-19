using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monlith.Nodes;
using Monolith.Attributes;
using Monolith.Geometry;
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
    public class RoomCamera : Camera2D
    {
        private bool _entered;
        private int _dir;
        
        public Node2D TargetNode { get; set; }

        public List<Action> TransitionStarted { get; set; }
        public List<Action> TransitionEnded { get; set; }

        public RectangleShape2D CameraRectangle;

        public Area2D TopArea { get; set; }
        public Area2D BottomArea { get; set; }
        public Area2D RightArea { get; set; }
        public Area2D LeftArea { get; set; }


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

            TopArea = new Area2D(new AreaConfig
            {
                Position = new Vector2(GlobalPosition.X, GlobalPosition.Y - 55),
                CollisionShape2D = new CollisionShape2D(new CollisionShapeConfig
                {
                    Shape = new RectangleShape2D(100, 10),
                    Position = Vector2.Zero
                })
            });

            BottomArea = new Area2D(new AreaConfig
            {
                Position = new Vector2(GlobalPosition.X, GlobalPosition.Y + 55),
                CollisionShape2D = new CollisionShape2D(new CollisionShapeConfig
                {
                    Shape = new RectangleShape2D(100, 10),
                    Position = Vector2.Zero
                })
            });

            LeftArea = new Area2D(new AreaConfig
            {
                Position = new Vector2(GlobalPosition.X - 55, GlobalPosition.Y),
                CollisionShape2D = new CollisionShape2D(new CollisionShapeConfig
                {
                    Shape = new RectangleShape2D(10, 100),
                    Position = Vector2.Zero
                })
            });

            RightArea = new Area2D(new AreaConfig
            {
                Position = new Vector2(GlobalPosition.X + 55, GlobalPosition.Y),
                CollisionShape2D = new CollisionShape2D(new CollisionShapeConfig
                {
                    Shape = new RectangleShape2D(10, 100),
                    Position = Vector2.Zero
                })
            });
        }


        public override void Unload()
        {
            base.Unload();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (TargetNode == null || !TargetNode.HasChild<CollisionShape2D>())
                return;

            CollisionShape2D targetShape;

        
            targetShape = (CollisionShape2D)TargetNode.GetFirstChildByT<CollisionShape2D>();

            var side = CollisionHelper.GetCameraEdge(
                targetShape.Shape,
                CameraRectangle
            );


            if (!_entered)
            {
                switch (side)
                {
                    case CollisionSide.Left:
                        ShiftRoom(-1);
                        break;

                    case CollisionSide.Right:
                        ShiftRoom(1);
                        break;
                }
            }

            _entered = side != CollisionSide.None;

            UpdateCameraRectangle();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            DrawHelper.DrawRegionShapeHollow(TopArea.CollisionShape2D.Shape, Color.Green, 2);
            DrawHelper.DrawRegionShapeHollow(BottomArea.CollisionShape2D.Shape, Color.Green, 2);
            DrawHelper.DrawRegionShapeHollow(RightArea.CollisionShape2D.Shape, Color.Green, 2);
            DrawHelper.DrawRegionShapeHollow(LeftArea.CollisionShape2D.Shape, Color.Green, 2);
        }

        private void UpdateCameraRectangle()
        {
            var cfg = Engine.Instance.Config;

            CameraRectangle = new RectangleShape2D(
                (int)(GlobalPosition.X - cfg.RenderWidth * 0.5f / Zoom),
                (int)(GlobalPosition.Y - cfg.RenderHeight * 0.5f / Zoom),
                (int)(cfg.RenderWidth / Zoom),
                (int)(cfg.RenderHeight / Zoom)
            );
        }

        private void ShiftRoom(int dir)
        {
            _dir = dir;
            
            float startX = Position.X;
            float targetX = startX + dir * CameraRectangle.Width;

            foreach (var action in TransitionStarted)
                action?.Invoke();

            var cameraXTween = new Tween(
                0.5f,
                EasingFunctions.Linear,
                t => Position = new Vector2(MathHelper.Lerp(startX, targetX, t), Position.Y),
                () =>
                {
                    foreach (var action in TransitionEnded)
                        action?.Invoke();
                }
            );

            cameraXTween.Start();
            Engine.TweenManager.AddTween(cameraXTween);
        }


        private void LockBody()
        {
            if (TargetNode is KinematicBody2D body)
            {
                
                body.Locked = true;
                body.Offset(1 * _dir, 0);
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