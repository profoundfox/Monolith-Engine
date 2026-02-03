
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Attributes;

namespace Monolith.Graphics
{
    public class Particle
    {
        public ParticleInfo InitialInfo { get; private set; }

        private Vector2 _position;
        private float _lifespanLeft;
        private float _lifespanAmount;
        private Color _color;
        private float _opacity;

        public bool IsFinished { get; private set; }

        public Particle(Vector2 pos, ParticleInfo info)
        {
            InitialInfo = info;

            _position = pos;
            _lifespanLeft = info.Lifespan;
            _lifespanAmount = 1f;
            _color = info.ColorStart;
            _opacity = info.OpacityStart;
        }

        public void Update(float deltaTime)
        {
            _lifespanLeft -= deltaTime;
            if (_lifespanLeft <= 0f)
            {
                IsFinished = true;
                return;
            }

            _lifespanAmount = MathHelper.Clamp(_lifespanLeft / InitialInfo.Lifespan, 0, 1);
            _color = Color.Lerp(InitialInfo.ColorEnd, InitialInfo.ColorStart, _lifespanAmount);
            _opacity = MathHelper.Clamp(MathHelper.Lerp(InitialInfo.OpacityEnd, InitialInfo.OpacityStart, _lifespanAmount), 0, 1);
        }

        public void SubmitCall()
        {
            Engine.Screen.Call(new TextureDrawCall
            {
                Texture = InitialInfo.Texture,
                Color = Color.Red,
                Rotation = 0f,
                Origin = Vector2.Zero,
                Scale = new Vector2(100)
            });
        }
    }
}