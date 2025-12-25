using System;
using Microsoft.Xna.Framework;

namespace Monolith.Geometry
{
    public readonly record struct Transform2D
    {
        public Vector2 Position { get; init; }
        public float Rotation { get; init; }
        public Vector2 Scale { get; init; }

        public static readonly Transform2D Identity =
            new(Vector2.Zero, 0f, Vector2.One);

        public Transform2D(Vector2 position, float rotation = 0f, Vector2? scale = null)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale ?? Vector2.One;
            if (Scale.X == 0 && Scale.Y == 0)
                Scale = Vector2.One;
        }


        /// <summary>
        /// Converts this transform into a world matrix.
        /// </summary>
        public Matrix ToMatrix()
        {
            return
                Matrix.CreateScale(new Vector3(Scale, 1f)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateTranslation(new Vector3(Position, 0f));
        }

        /// <summary>
        /// Combines a parent and child transform into a global transform.
        /// </summary>
        public static Transform2D Combine(
            in Transform2D parent,
            in Transform2D child
        )
        {
            var matrix = child.ToMatrix() * parent.ToMatrix();

            var position = matrix.Translation;

            var scale = new Vector2(
                new Vector2(matrix.M11, matrix.M12).Length(),
                new Vector2(matrix.M21, matrix.M22).Length()
            );

            var rotation = (float)Math.Atan2(matrix.M21, matrix.M11);

            return new Transform2D(
                new Vector2(position.X, position.Y),
                rotation,
                scale
            );
        }
    }
}
