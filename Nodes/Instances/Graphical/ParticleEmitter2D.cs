using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Attributes;
using Monolith.Graphics;
using Monolith.Helpers;
using System;
using System.Collections.Generic;

namespace Monolith.Nodes
{
    public class ParticleEmitter2D : Node2D
    {
        private readonly List<Particle> _particles = [];
        
        private float _intervalLeft;

        public EmitterProperties Properties { get; set; } = EmitterProperties.Identity;
        
        public ParticleEmitter2D()
        {
            _intervalLeft = Properties.Interval;
        }

        private void Spawn(Vector2 pos)
        {
            ParticleProperties d = Properties.ParticleProperties;

            d.Lifespan = MathM.RandomFloat(Properties.LifespanMin, Properties.LifespanMax);
            d.Speed = MathM.RandomFloat(Properties.SpeedMin, Properties.SpeedMax);
            d.Angle = MathM.RandomFloat(
                Properties.Angle - Properties.AngleVariance,
                Properties.Angle + Properties.AngleVariance);

            Particle p = new(pos, d);
            _particles.Add(p);
        } 

        /// <summary>
        /// Emits the particle at the emitter's positon and with the default count.
        /// </summary>
        public void Emit()
        {
            Emit(GlobalPosition, Properties.EmitCount);
        }

        /// <summary>
        /// Emits the particle at the emitter's position with the specified count.
        /// </summary>
        /// <param name="count"></param>
        public void Emit(int count)
        {
            Emit(GlobalPosition, count);
        }

        /// <summary>
        /// Emits the particles with a specified position and count.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="count"></param>
        public void Emit(Vector2 position, int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                Spawn(position);
            }
        }
        
        public override void ProcessUpdate(float delta)
        {
            base.ProcessUpdate(delta);

            _intervalLeft -= delta;

            while (_intervalLeft <= 0f)
            {
                _intervalLeft += Properties.Interval;

                Emit();
            }

            foreach (var particle in _particles)
            {
                particle.Update(delta);
            }

            _particles.RemoveAll(p => p.isFinished);
        }

        public override void SubmitCall()
        {
            base.SubmitCall();

            foreach (var particle in _particles)
            {
                particle.Draw();
            }
        }
    }
}