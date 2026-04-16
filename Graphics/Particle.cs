using System;
using Microsoft.Xna.Framework;
using Monolith.Params;

namespace Monolith.Graphics
{
    public record struct ParticleInfo
    {
        public Vector2 Position { get; set; }
        public float LifespanLeft { get; set; }
        public float LifespanAmount { get; set; }
        public Color Color { get; set; }
        public float Opacity { get; set; }
        public bool IsFinished { get; set; }
        public float Scale { get; set; }
        public Vector2 Origin { get; set; }
        public Vector2 Direction { get; set; }
    }

    public class Particle
    {
        private readonly ParticleParams _initialData;
        private ParticleInfo _info;

        public ParticleParams InitialData
        {
            get => _initialData;
        }

        public ParticleInfo Info
        {
            get => _info;
        }

        public Particle(Vector2 pos, ParticleParams data)
        {
            _initialData = data;

            _info.LifespanLeft = _initialData.Lifespan;
            _info.LifespanAmount = 1f;
            _info.Position = pos;
            _info.Color = _initialData.ColorStart;
            _info.Opacity = _initialData.OpacityStart;
            _info.Origin = new(
                _initialData.Texture.Bounds.Width / 2,
                _initialData.Texture.Bounds.Height / 2
            );

            if (_initialData.Speed != 0)
            {
                _info.Direction = new Vector2(
                    (float)Math.Sin(_initialData.Angle),
                    (float)Math.Cos(_initialData.Angle)
                );
            }
            else
            {
                _info.Direction = Vector2.Zero;
            }
        }

        public void Update(float delta)
        {
            _info.LifespanLeft -= delta;

            if (_info.LifespanLeft <= 0f)
            {
                _info.IsFinished = true;
                return;
            }

            _info.LifespanAmount = MathHelper.Clamp(
                _info.LifespanLeft / _initialData.Lifespan,
                0,
                1
            );

            _info.Color = Color.Lerp(
                _initialData.ColorEnd,
                _initialData.ColorStart,
                _info.LifespanAmount
            );

            _info.Opacity = MathHelper.Clamp(
                MathHelper.Lerp(
                    _initialData.OpacityEnd,
                    _initialData.OpacityStart,
                    _info.LifespanAmount
                ),
                0,
                1
            );

            _info.Scale = MathHelper.Lerp(
                _initialData.SizeEnd,
                _initialData.SizeStart,
                _info.LifespanAmount
            ) / _initialData.Texture.Bounds.Width;

            _info.Position += _info.Direction * _initialData.Speed * delta;
        }

        public void Draw()
        {
            Engine.Canvas.Call(new TextureDrawCall
            {
                Texture = _initialData.Texture,
                Params = CanvasParams.Identity with
                {
                    Position = _info.Position,
                    Color = _info.Color * _info.Opacity,
                    Rotation = 0f,
                    Origin = _info.Origin,
                    Scale = new Vector2(_info.Scale),
                },
                Depth = 99
            });
        }
    }
}
