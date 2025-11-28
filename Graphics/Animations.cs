using System;
using System.Collections.Generic;

namespace Monolith.Graphics
{
    public class Animation
    {
        public List<MTexture> Frames { get; set; }

        public TimeSpan Delay { get; set; }


        public Animation()
        {
            Frames = new List<MTexture>();
            Delay = TimeSpan.FromMicroseconds(100);
        }

        public Animation(List<MTexture> frames, TimeSpan delay)
        {
            Frames = frames;
            Delay = delay;
        }

        
    }
}