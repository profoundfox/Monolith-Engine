using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Monolith.Helpers;
using Monolith.Managers;

namespace Monolith.Util
{
    public class Tween<T> : ITween
    {
        public float Duration { get; private set; }
        public Func<float, float> EasingFunction { get; private set; }
        public Action<T> OnUpdate { get; private set; }

        private Action callbackAction;
        private float elapsedTime = 0f;
        private bool isRunning = false;
        private bool isComplete = false;
        private readonly Func<T, T, float, T> _lerpFunc;
        private readonly T _start;
        private readonly T _end;

        internal Tween(
            T start,
            T end,
            float duration,
            Func<T, T, float, T> lerpFunc,
            Action<T> onUpdate,
            Func<float, float> easingFunction = null
            )
        {
            if (easingFunction == null)
                easingFunction = EasingFunctions.Linear;

            _start = start;
            _end = end;
            Duration = duration;
            OnUpdate = onUpdate;
            _lerpFunc = lerpFunc;
            EasingFunction = easingFunction;
            

            Engine.Tween.AddTween(this);

            Start();
        }

        public void Start()
        {
            elapsedTime = 0f;
            isRunning = true;
        }

        public void SetCallbackAction(Action action)
        {
            callbackAction = action;
        }

        public void Update()
        {
            if (!isRunning) return;

            elapsedTime += Engine.DeltaTime;

            float t = MathHelper.Clamp(elapsedTime / Duration, 0f, 1f);
            float eased = EasingFunction(t);
            OnUpdate?.Invoke(_lerpFunc(_start, _end, eased));

            if (t >= 1f)
            {
                isComplete = true;
                isRunning = false;
                callbackAction?.Invoke();
            }
        }

        public bool IsRunning() => isRunning;
        public bool IsComplete() => isComplete;
    }

}