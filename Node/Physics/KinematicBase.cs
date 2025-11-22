using ConstructEngine.Nodes;
using ConstructEngine.Region;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace ConstructEngine.Components
{
    public class KinematicBody2D : Node
    {
        public Vector2 Velocity;

        private float remainderX = 0;
        private float remainderY = 0;
        public bool Locked;

        public KinematicBody2D() {}



        public void UpdateKinematicBody()
        {
            if (!Locked)
            {
                Move(Velocity.X * Engine.DeltaTime, Velocity.Y * Engine.DeltaTime);
            }
            else
            {
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
                var test = Shape.Clone();
                test.Offset(sign, 0);

                if (!IsColliding(test))
                {
                    Shape.Offset(sign, 0);
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
                var test = Shape.Clone();
                test.Offset(0, sign);

                if (!IsColliding(test))
                {
                    Shape.Offset(0, sign);
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
            var test = Shape.Clone();
            test.Offset(0, 1);
            return IsColliding(test);
        }


        public bool IsOnWall()
        {
            var left = Shape.Clone();
            left.Offset(-1, 0);

            var right = Shape.Clone();
            right.Offset(1, 0);

            foreach (var body in Node.AllInstances.OfType<StaticBody2D>())
            {
                var shape = body.Shape;

                if (left.Intersects(shape) || right.Intersects(shape))
                    return true;
            }

            return false;
        }


        public bool IsColliding(IRegionShape2D shape)
        {
            foreach (var body in Node.AllInstances.OfType<StaticBody2D>())
            {
                if (shape.Intersects(body.Shape))
                    return true;
            }

            return false;
        }
    }
}
