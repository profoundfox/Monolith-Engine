using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using ConstructEngine.Helpers;

namespace ConstructEngine.Util
{
    public class Tween
    {
        public float Duration { get; private set; }
        public Func<float, float> EasingFunction { get; private set; }
        public Action<float> OnUpdate { get; private set; }
        public Action OnComplete { get; private set; }

        private float elapsedTime = 0f;
        private bool isRunning = false;
        private bool isComplete = false;

        public Tween(float duration, Func<float, float> easingFunction, Action<float> onUpdate, Action onComplete = null)
        {
            Duration = duration;
            EasingFunction = easingFunction ?? EasingFunctions.Linear;
            OnUpdate = onUpdate;
            OnComplete = onComplete;
        }

        public void Start()
        {
            elapsedTime = 0f;
            isRunning = true;
        }

        public void Update()
        {
            if (!isRunning) return;

            elapsedTime += Engine.DeltaTime;

            float t = MathHelper.Clamp(elapsedTime / Duration, 0f, 1f);
            float value = EasingFunction(t);
            OnUpdate?.Invoke(value);

            if (t >= 1f)
            {
                isComplete = true;
                isRunning = false;
                OnComplete?.Invoke();
            }
        }

        public bool IsRunning() => isRunning;
        public bool IsComplete() => isComplete;
    }
}