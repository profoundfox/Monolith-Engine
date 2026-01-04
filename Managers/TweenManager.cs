using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monolith.Helpers;
using Monolith.Util;

namespace Monolith.Managers
{
    public class TweenManager
    {
        private readonly List<ITween> _tweens = new();
        public TweenManager() {}

        /// <summary>
        /// Adds a tween.
        /// </summary>
        /// <param name="tween"></param>
        public void AddTween<T>(Tween<T> tween)
        {
            _tweens.Add(tween);
        }

        /// <summary>
        /// Adds multiple tweens from a list.
        /// </summary>
        /// <param name="tweens"></param>
        public void AddTweens<T>(Tween<T>[] tweens)
        {
            _tweens.AddRange(tweens);
        }

        public Tween<T> CreateTween<T>
        (
            Action<T> setter,
            T start,
            T end,
            float duration,
            Func<T, T, float, T> lerpFunc,
            Func<float, float> easingFunction = null)
        {
            if (easingFunction != null)
                easingFunction = EasingFunctions.Linear;

            var tween = new Tween<T>(
                start,
                end,
                duration,
                lerpFunc,
                setter,
                easingFunction
            );

            AddTween(tween);
            return tween;
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

        public IReadOnlyList<ITween> Tweens => _tweens;
    }
}
