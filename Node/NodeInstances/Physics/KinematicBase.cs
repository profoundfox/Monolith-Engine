using Monolith.Nodes;
using Monolith.Geometry;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Monolith.Managers;

namespace Monolith.Nodes
{
    public record class KinematicBaseConfig : SpatialNodeConfig
    {
        
    }

    public class KinematicBody2D : Node2D
    {
        public Vector2 Velocity;

        internal Vector2 PriorityVelocity;

        private float remainderX = 0;
        private float remainderY = 0;
        public bool Locked;

        public KinematicBody2D(KinematicBaseConfig config) : base(config) {}
        
        public void UpdateKinematicBody()
        {
            if (!Locked)
            {
                if (PriorityVelocity != Vector2.Zero)
                    Move(PriorityVelocity.X * Engine.DeltaTime, PriorityVelocity.Y * Engine.DeltaTime);
                else
                    Move(Velocity.X * Engine.DeltaTime, Velocity.Y * Engine.DeltaTime);
            }
            else
            {
                PriorityVelocity = Vector2.Zero;
                Velocity = Vector2.Zero;
            }
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
                var test = Region.Clone();
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
                var test = Region.Clone();
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
            var test = Region.Clone();
            test.Offset(0, 1);
            return IsColliding(test);
        }


        public bool IsOnWall()
        {
            var left = Region.Clone();
            left.Offset(-1, 0);

            var right = Region.Clone();
            right.Offset(1, 0);

            foreach (var body in NodeManager.AllInstances.OfType<StaticBody2D>())
            {
                if (body.Collidable == false) return false;
                
                var Region = body.Region;

                if (left.Intersects(Region) || right.Intersects(Region))
                    return true;
            }

            return false;
        }


        public bool IsColliding(IRegionShape2D Region)
        {
            foreach (var body in NodeManager.AllInstances.OfType<StaticBody2D>())
            {
                if (body.Collidable == false) return false;
                
                if (Region.Intersects(body.Region))
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
