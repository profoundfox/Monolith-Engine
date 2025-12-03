using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Monolith.Geometry;
using Monolith.Managers;

namespace Monolith.Nodes
{
    public record class DynamicBodyConfig : StaticBodyConfig
    {
        public Vector2 InitialVelocity { get; set; } = Vector2.Zero;
    }

    public class DynamicBody2D : StaticBody2D
    {
        public Vector2 Velocity;

        private float remainderX = 0;
        private float remainderY = 0;

        public DynamicBody2D(DynamicBodyConfig cfg) : base(cfg)
        {
            Velocity = cfg.InitialVelocity;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Vector2 delta = Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            Move(delta.X, delta.Y);

            PushBodies(delta);
        }

        private void Move(float moveX, float moveY)
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

        private void MoveX(int amount)
        {
            int sign = Math.Sign(amount);
            while (amount != 0)
            {
                var test = Region.Clone();
                test.Offset(sign, 0);

                // Stop moving if colliding with a static body
                if (IsColliding(test))
                    break;

                Position += new Vector2(sign, 0);
                amount -= sign;
            }
        }

        private void MoveY(int amount)
        {
            int sign = Math.Sign(amount);
            while (amount != 0)
            {
                var test = Region.Clone();
                test.Offset(0, sign);

                // Stop moving if colliding with a static body
                if (IsColliding(test))
                    break;

                Position += new Vector2(0, sign);
                amount -= sign;
            }
        }

        private void PushBodies(Vector2 delta)
        {
            foreach (var body in NodeManager.AllInstances.OfType<KinematicBody2D>())
            {
                // Skip if the body does not intersect platform region
                if (!Region.Intersects(body.Region))
                    continue;

                // Only push the body if standing on top or touching from sides
                var bodyFeet = body.Region.Clone();
                bodyFeet.Offset(0, 1); // tiny offset to detect "on top"
                if (Region.Intersects(bodyFeet) || Region.Intersects(body.Region))
                {
                    body.PriorityVelocity += delta;
                }
            }
        }

        private bool IsColliding(IRegionShape2D testRegion)
        {
            foreach (var body in NodeManager.AllInstances.OfType<StaticBody2D>())
            {
                if (body == this || !body.Collidable) continue;
                if (testRegion.Intersects(body.Region))
                    return true;
            }
            return false;
        }
    }
}
