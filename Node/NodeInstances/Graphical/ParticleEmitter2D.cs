using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Attributes;
using Monolith.Graphics;

namespace Monolith.Nodes
{
    public class ParticleEmitter2D : Node2D
    {
        public ParticleInfo ParticleInfo { get; set; } = ParticleInfo.Identity;
        public int EmittedPerSecond { get; set; } = 10;

        private List<Particle> _particles = new();

        private float _emissionTimer;
        private float _spawnInterval;

        public ParticleEmitter2D()
        {
            _spawnInterval = 1f / EmittedPerSecond;
        }

        public override void Load()
        {
            base.Load();
        }

        public override void ProcessUpdate(float delta)
        {
            base.ProcessUpdate(delta);

            _emissionTimer += delta;

            while (_emissionTimer >= _spawnInterval)
            {
                _particles.Add(new Particle(Position, ParticleInfo));
                _emissionTimer -= _spawnInterval; 
            }

            foreach (var p in _particles)
                p.Update(delta);

            _particles.RemoveAll(p => p.IsFinished);
        }

        public override void SubmitCall()
        {
            base.SubmitCall();

            foreach (var p in _particles)
                p.SubmitCall();
        }

    }

}