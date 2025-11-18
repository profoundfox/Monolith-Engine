using System;
using System.Collections.Generic;
using ConstructEngine.Util;

namespace ConstructEngine.Managers
{
    public class TweenManager
    {
        private readonly List<Tween> _tweens = new();
        public TweenManager() { }

        /// <summary>
        /// Adds a tween.
        /// </summary>
        /// <param name="tween"></param>
        public void AddTween(Tween tween)
        {
            _tweens.Add(tween);
        }

        /// <summary>
        /// Adds multiple tweens from a list.
        /// </summary>
        /// <param name="tweens"></param>
        public void AddTweens(Tween[] tweens)
        {
            foreach(var t in tweens)
            {
                AddTween(t);
            }
        }
        
        /// <summary>
        /// Updates all the tweens.
        /// </summary>
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
