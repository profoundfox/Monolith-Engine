using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Monolith.Util
{
    public class Timer
    {
        public TimeSpan TimeLeft;
        public TimeSpan Interval;
        public Action Callback;
        public bool Repeat;
        public bool Cancelled;
    }
}