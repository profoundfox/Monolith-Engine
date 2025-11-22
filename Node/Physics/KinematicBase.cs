using ConstructEngine.Nodes;
using ConstructEngine.Region;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace ConstructEngine.Components
{
    public class KinematicBody2D : Node
    {
        public IRegionShape2D Collider;
        public Vector2 Velocity;

        private float remainderX = 0;
        private float remainderY = 0;

        public bool Locked;

        public Vector2 Position
        {
            get => new Vector2(Collider.X, Collider.Y);
            set
            {
                Collider.X = (int)value.X;
                Collider.Y = (int)value.Y;
            }
        }

        public KinematicBody2D() {}

        public void UpdateCollider(GameTime gameTime)
        {
            if (!Locked)
            {
                float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
                Move(Velocity.X * dt, Velocity.Y * dt);
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
                var test = Collider.Clone();
                test.Offset(sign, 0);

                if (!IsColliding(test))
                {
                    Collider.Offset(sign, 0);
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
                var test = Collider.Clone();
                test.Offset(0, sign);

                if (!IsColliding(test))
                {
                    Collider.Offset(0, sign);
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
            var test = Collider.Clone();
            test.Offset(0, 1);
            return IsColliding(test);
        }


        public bool IsOnWall()
        {
            var left = Collider.Clone();
            left.Offset(-1, 0);

            var right = Collider.Clone();
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
