using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Params;
using Monolith.Graphics;
using Monolith.Tools;
using System;
using System.Collections.Generic;

namespace Monolith.Hierarchy
{
    public class ParticleEmitter2D : Node2D
    {
        private readonly List<Particle> _particles = [];

        private float _intervalLeft;

        public EmitterParams Params { get; set; } = EmitterParams.Identity;

        public IReadOnlyList<Particle> Particles => _particles;

        public ParticleEmitter2D()
        {
            _intervalLeft = Params.Interval;
        }

        private void Spawn(Vector2 pos)
        {
            ParticleParams d = Params.Params;

            d.Lifespan = MathE.RandomFloat(Params.LifespanMin, Params.LifespanMax);
            d.Speed = MathE.RandomFloat(Params.SpeedMin, Params.SpeedMax);
            d.Angle = MathE.RandomFloat(
                Params.Angle - Params.AngleVariance,
                Params.Angle + Params.AngleVariance);

            Particle p = new(pos, d);
            _particles.Add(p);
        }

        /// <summary>
        /// Emits the particle at the emitter's positon and with the default count.
        /// </summary>
        public void Emit()
        {
            Emit(Transform.Global.Position, Params.EmitCount);
        }

        /// <summary>
        /// Emits the particle at the emitter's position with the specified count.
        /// </summary>
        /// <param name="count"></param>
        public void Emit(int count)
        {
            Emit(Transform.Global.Position, count);
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
                _intervalLeft += Params.Interval;

                Emit();
            }

            foreach (var particle in _particles)
            {
                particle.Update(delta);
            }

            _particles.RemoveAll(p => p.Info.IsFinished);
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
