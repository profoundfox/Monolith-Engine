using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Attributes;
using Monolith.Graphics;
using System;
using System.Collections.Generic;

namespace Monolith.Nodes
{
    public class ParticleEmitter2D : Node2D
    {
        private List<Particle> _particles;
        private float _timeSinceLastEmission;

        public ParticleProperties Properties { get; set; } = ParticleProperties.Identity;
        public MTexture Texture { get; set; } = Engine.Pixel;
        public float EmissionRate { get; set; } = 50f;

        public ParticleEmitter2D()
        {
            _particles = new List<Particle>();
            _timeSinceLastEmission = 0f;
        }

        private void EmitParticles(float delta)
        {
            _timeSinceLastEmission += delta;

            while (_timeSinceLastEmission >= 1.0f / EmissionRate)
            {
                _timeSinceLastEmission -= 1.0f / EmissionRate;

                var particle = new Particle(GlobalPosition, Properties);
                _particles.Add(particle);
            }
        }

        public override void ProcessUpdate(float delta)
        {
            base.ProcessUpdate(delta);

            EmitParticles(delta);

            for (int i = _particles.Count - 1; i >= 0; i--)
            {
                var particle = _particles[i];

                particle.Update(delta);

                if (particle.IsExpired)
                {
                    _particles.RemoveAt(i);
                }
                else
                {
                    _particles[i] = particle;
                }
            }
        }

        public override void SubmitCall()
        {
            base.SubmitCall();

            foreach (var particle in _particles)
            { 
                var drawPosition = particle.Position - GlobalPosition;
                var particleColor = particle.Properties.Color;
                var scale = new Vector2(particle.Properties.Size);


                Engine.Canvas.Call(new TextureDrawCall
                {
                    Texture = Texture,
                    Position = drawPosition,
                    Color = particleColor,
                    Rotation = particle.Properties.Rotation,
                    Origin = Texture.Center,
                    Scale = scale,
                    Effects = GlobalSpriteEffects,
                    Depth = GlobalDepth,
                    SpriteBatchConfig = SpriteBatchConfig.Default with
                    {
                        Effect = GlobalShader
                    }
                });
            }
        }
    }
}