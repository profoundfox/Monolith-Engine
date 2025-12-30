using Monolith.Nodes;
using Monolith.Geometry;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Monolith.Managers;
using System.Drawing;
using Monlith.Nodes;

namespace Monolith.Nodes
{
    public record class KinematicBaseConfig : SpatialNodeConfig
    {
        public CollisionShape2D CollisionShape2D { get; set; }
    }

    public class KinematicBody2D : Node2D
    {
        public Vector2 Velocity;

        public CollisionShape2D CollisionShape2D { get; set; }

        internal Vector2 PriorityVelocity;

        private float remainderX = 0;
        private float remainderY = 0;
        public bool Locked;

        public KinematicBody2D(KinematicBaseConfig cfg) : base(cfg)
        {
            CollisionShape2D = cfg.CollisionShape2D;
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            if (Locked)
            {
                PriorityVelocity = Vector2.Zero;
                Velocity = Vector2.Zero;
                return;
            }

            Vector2 motion = PriorityVelocity != Vector2.Zero ? PriorityVelocity : Velocity;
            Move(motion.X * Engine.DeltaTime, motion.Y * Engine.DeltaTime);

            if (IsColliding(CollisionShape2D.Shape))
                ResolveOverlap();
        }


        public void ResolveOverlap()
        {
            if (CollisionShape2D.Disabled)
                return;

            var shape = CollisionShape2D.Shape;

            float pushX = 0;
            float pushY = 0;

            foreach (var body in Engine.Node.AllInstances.OfType<StaticBody2D>())
            {
                var other = body.CollisionShape2D.Shape;

                if (shape.Intersects(other))
                {
                    float overlapX = 0;
                    if (shape.BoundingBox.Right > other.BoundingBox.Left && shape.BoundingBox.Left < other.BoundingBox.Right)
                    {
                        float leftOverlap = shape.BoundingBox.Right - other.BoundingBox.Left;
                        float rightOverlap = other.BoundingBox.Right - shape.BoundingBox.Left;
                        overlapX = (leftOverlap < rightOverlap) ? -leftOverlap : rightOverlap;
                    }

                    float overlapY = 0;
                    if (shape.BoundingBox.Bottom > other.BoundingBox.Top && shape.BoundingBox.Top < other.BoundingBox.Bottom)
                    {
                        float topOverlap = shape.BoundingBox.Bottom - other.BoundingBox.Top;
                        float bottomOverlap = other.BoundingBox.Bottom - shape.BoundingBox.Top;
                        overlapY = (topOverlap < bottomOverlap) ? -topOverlap : bottomOverlap;
                    }

                    if (MathF.Abs(overlapX) < MathF.Abs(overlapY))
                        pushX += overlapX;
                    else
                        pushY += overlapY;
                }
            }

            LocalPosition += new Vector2(pushX, pushY);
            Velocity = Vector2.Zero;
        }





        public void Move(float moveX, float moveY)
        {
            remainderX += moveX;
            int mx = (int)MathF.Round(remainderX);
            remainderX -= mx;
            MoveX(mx);

            remainderY += moveY;
            int my = (int)MathF.Round(remainderY);
            remainderY -= my;
            MoveY(my);
        }


        public void MoveX(int amount)
        {
            

            int sign = Math.Sign(amount);

            while (amount != 0)
            {
                var test = CollisionShape2D.Shape.Clone();
                test.Offset(sign, 0);

                if (!IsColliding(test))
                {
                    LocalPosition += new Vector2(sign, 0);
                }
                else
                {
                    if (MathF.Sign(Velocity.X) == sign)
                        Velocity.X = 0;

                    break;
                }

                amount -= sign;
            }
        }


        public void MoveY(int amount)
        {
            int sign = Math.Sign(amount);

            while (amount != 0)
            {
                var test = CollisionShape2D.Shape.Clone();
                test.Offset(0, sign);

                if (!IsColliding(test))
                {
                    LocalPosition += new Vector2(0, sign);
                }
                else
                {
                    if (MathF.Sign(Velocity.Y) == sign)
                        Velocity.Y = 0;

                    break;
                }

                amount -= sign;
            }
        }

        public bool IsOnGround()
        {
            var test = CollisionShape2D.Shape.Clone();
            test.Offset(0, 1);
            return IsColliding(test);
        }

        public bool IsOnRoof()
        {
            var test = CollisionShape2D.Shape.Clone();
            test.Offset(0, -1);
            return IsColliding(test);
        }

        public bool IsOnWall()
        {
            return IsOnWall(out _);
        }

        public bool IsOnWall(out int wallDir)
        {
            if (CollisionShape2D.Disabled)
            {
                wallDir = default;
                return false;
            }

            var left = CollisionShape2D.Shape.Clone();
            left.Offset(-1, 0);

            var right = CollisionShape2D.Shape.Clone();
            right.Offset(1, 0);

            foreach (var body in Engine.Node.AllInstances.OfType<StaticBody2D>())
            {
                var shape = body.CollisionShape2D;

                if (shape.Intersects(left))
                {
                    wallDir = -1;
                    return true;
                }

                if (shape.Intersects(right))
                {
                    wallDir = 1;
                    return true;
                }
            }

            wallDir = default;
            return false;
        }


        public bool IsColliding(IRegionShape2D Region)
        {
            if (CollisionShape2D.Disabled)
                return false;
            
            foreach (var body in Engine.Node.AllInstances.OfType<StaticBody2D>())
            {                
                if (body.CollisionShape2D.Intersects(Region))
                    return true;
            }

            return false;
        }

        public float MoveToward(float current, float target, float maxDelta)
        {
            if (MathF.Abs(target - current) <= maxDelta) return target;
            return current + MathF.Sign(target - current) * maxDelta;
        }
    }
}
