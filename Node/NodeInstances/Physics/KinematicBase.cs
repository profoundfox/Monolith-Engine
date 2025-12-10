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
        
        public void UpdateKinematicBody()
        {
            if (!Locked)
            {
                if (PriorityVelocity != Vector2.Zero)
                    Move(PriorityVelocity.X * Engine.DeltaTime, PriorityVelocity.Y * Engine.DeltaTime);
                else
                    Move(Velocity.X * Engine.DeltaTime, Velocity.Y * Engine.DeltaTime);

                if (IsColliding(CollisionShape2D.Shape))
                    ResolveOverlap();
            }
            else
            {
                PriorityVelocity = Vector2.Zero;
                Velocity = Vector2.Zero;
            }
        }

        public void ResolveOverlap()
        {
            if (CollisionShape2D.Disabled)
                return;

            const int maxIterations = 10; 
            int iterations = 0;

            while (IsColliding(CollisionShape2D.Shape) && iterations < maxIterations)
            {
                Vector2[] directions =
                {
                    new Vector2(1, 0),
                    new Vector2(-1, 0),
                    new Vector2(0, 1),
                    new Vector2(0, -1)
                };

                Vector2 bestOffset = Vector2.Zero;

                foreach (var dir in directions)
                {
                    var testShape = CollisionShape2D.Shape.Clone();
                    testShape.Offset(dir.ToPoint());

                    if (!IsColliding(testShape))
                    {
                        bestOffset = dir;
                        break;
                    }
                }

                Position += bestOffset;
                iterations++;
            }

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
                    Position += new Vector2(sign, 0);
                }
                else
                {
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
                    Position += new Vector2(0, sign);
                }
                else
                {
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


        public bool IsOnWall()
        {
            if (CollisionShape2D.Disabled)
                return false;
            
            var left = CollisionShape2D.Shape.Clone();
            left.Offset(-1, 0);

            var right = CollisionShape2D.Shape.Clone();
            right.Offset(1, 0);

            foreach (var body in NodeManager.AllInstances.OfType<StaticBody2D>())
            {
                var shape = body.CollisionShape2D;

                if (shape.Intersects(left) || shape.Intersects(right))
                    return true;
            }

            return false;
        }


        public bool IsColliding(IRegionShape2D Region)
        {
            if (CollisionShape2D.Disabled)
                return false;
            
            foreach (var body in NodeManager.AllInstances.OfType<StaticBody2D>())
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
