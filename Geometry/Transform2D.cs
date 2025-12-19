using System;
using Microsoft.Xna.Framework;

namespace Monolith.Geometry
{
    public struct Transform2D
    {
        public Vector2 Position;
        public float Rotation;
        public Vector2 Scale;

        public static readonly Transform2D Identity = new Transform2D
        {
            Position = Vector2.Zero,
            Rotation = 0f,
            Scale = Vector2.One
        };

        public Transform2D(Vector2 position, float rotation = 0f, Vector2? scale = null)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale ?? Vector2.One;
        }

        public Matrix ToMatrix()
        {
            return
                Matrix.CreateScale(new Vector3(Scale, 1f)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateTranslation(new Vector3(Position, 0f));
        }

        public static Transform2D Combine(Transform2D parent, Transform2D child)
        {
            var m = child.ToMatrix() * parent.ToMatrix();
            var position = m.Translation;
            var scale = new Vector2(
                new Vector2(m.M11, m.M12).Length(),
                new Vector2(m.M21, m.M22).Length()
            );
            var rotation = (float)Math.Atan2(m.M21, m.M11);
            return new Transform2D(new Vector2(position.X, position.Y), rotation, scale);
        }
    }
}
