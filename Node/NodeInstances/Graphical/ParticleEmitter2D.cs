using System.Collections.Generic;
using Monolith.Graphics;

namespace Monolith.Nodes
{
    public record class ParticleEmitterConfig : CanvasNodeConfig
    {
        
    }
    public class ParticleEmitter2D : CanvasNode
    {
        private List<Particle> _particles = new();

        public ParticleEmitter2D(ParticleEmitterConfig cfg) : base(cfg) {  }

        public override void Load()
        {
            base.Load();
        }

        public override void ProcessUpdate(float delta)
        {
            base.ProcessUpdate(delta);

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