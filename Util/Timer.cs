using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Monolith.Util
{
    public class Timer
    {
        public float TimeLeft;
        public float Interval;
        public Action Callback;
        public bool Repeat;
        public bool Cancelled;
    }
}