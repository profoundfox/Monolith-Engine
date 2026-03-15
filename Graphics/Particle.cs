
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Attributes;
using Monolith.Managers;

namespace Monolith.Graphics
{
    public class Particle
    {
        private readonly ParticleProperties _data;
        private Vector2 _position;
        private float _lifespanLeft;
        private float _lifespanAmount;
        private Color _color;
        private float _opacity;
        public bool isFinished = false;
        private float _scale;
        private Vector2 _origin;
        private Vector2 _direction;

        public Particle(Vector2 pos, ParticleProperties data)
        {
            _data = data;
            _lifespanLeft = data.Lifespan;
            _lifespanAmount = 1f;
            _position = pos;
            _color = data.ColorStart;
            _opacity = data.OpacityStart;
            _origin = new(_data.Texture.Bounds.Width / 2, _data.Texture.Bounds.Height / 2);

            if (data.Speed != 0)
            {
                _direction = new Vector2(
                    (float)Math.Sin(_data.Angle),
                    (float)Math.Cos(_data.Angle)
                );
            }
            else
            {
                _direction = Vector2.Zero;
            }
        }

        public void Update(float delta)
        {
            _lifespanLeft -= delta;
            if (_lifespanLeft <= 0f)
            {
                isFinished = true;
                return;
            }

            _lifespanAmount = MathHelper.Clamp(_lifespanLeft / _data.Lifespan, 0, 1);
            _color = Color.Lerp(_data.ColorEnd, _data.ColorStart, _lifespanAmount);
            _opacity = MathHelper.Clamp(MathHelper.Lerp(_data.OpacityEnd, _data.OpacityStart, _lifespanAmount), 0, 1);
            _scale = MathHelper.Lerp(_data.SizeEnd, _data.SizeStart, _lifespanAmount) / _data.Texture.Bounds.Width;
            _position += _direction * _data.Speed * delta;
        }

        public void Draw()
        {
            Engine.Canvas.Call(new TextureDrawCall
            {
                Texture = _data.Texture,
                Position = _position,
                Color = _color * _opacity,
                Rotation = 0f,
                Origin = _origin,
                Scale = new Vector2(_scale),
                Depth = 99,
            });
        }
    }

}