using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monolith.Util;

namespace Monolith.Managers
{
    public class TweenManager
    {
        private readonly List<Tween> _tweens = new();
        public TweenManager() {}

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
            _tweens.AddRange(tweens);
        }
        
        /// <summary>
        /// Updates all the tweens.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            for (int i = _tweens.Count - 1; i >= 0; i--)
            {
                _tweens[i].Update();
                if (_tweens[i].IsComplete())
                    _tweens.RemoveAt(i);
            }
        }

        public IReadOnlyList<Tween> Tweens => _tweens;
    }
}
