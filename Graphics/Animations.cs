using System;
using System.Collections.Generic;

namespace Monolith.Graphics
{
    public class Animation
    {
        public List<MTexture> Frames { get; set; }

        private float _delay;

        public float Delay
        {
            get => _delay;
            set => _delay = value / 1000f;
        }

        public Animation()
        {
            Frames = new List<MTexture>();
            Delay = 0f;
        }

        public Animation(List<MTexture> frames, float delay)
        {
            Frames = frames;
            Delay = delay;
        }

        
    }
}