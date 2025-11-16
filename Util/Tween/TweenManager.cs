using System;
using System.Collections.Generic;

namespace ConstructEngine.Util.Tween
{
    public class TweenManager
    {
        private static TweenManager _instance;
        private readonly List<Tween> _tweens = new();
        public TweenManager() { }
        public void Add(Tween tween)
        {
            _tweens.Add(tween);
        }

        public void Update()
        {
            for (int i = _tweens.Count - 1; i >= 0; i--)
            {
                _tweens[i].Update();
                if (_tweens[i].IsComplete())
                    _tweens.RemoveAt(i);
            }
        }
    }
}
