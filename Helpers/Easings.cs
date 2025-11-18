using System;

namespace ConstructEngine.Helpers
{
    /// <summary>
    /// Provides a collection of easing functions used to interpolate values over time.
    /// Based on Robert Penner's easing equations and adapted for Unity / game tweening.
    /// </summary>
    public static class EasingFunctions
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        const float c2 = c1 * 1.525f;
        const float c4 = (float)(2f * Math.PI) / 3f;
        const float c5 = (float)(2f * Math.PI) / 4.5f;


        #region Linear

        /// <summary>
        /// Linear easing — no acceleration, constant speed.
        /// </summary>
        public static float Linear(float t) => t;

        #endregion


        #region Quad

        /// <summary>
        /// Quadratic ease-in — accelerating from zero velocity.
        /// </summary>
        public static float EaseInQuad(float x) => x * x;

        /// <summary>
        /// Quadratic ease-out — decelerating to zero velocity.
        /// </summary>
        public static float EaseOutQuad(float x) => 1 - (1 - x) * (1 - x);

        /// <summary>
        /// Quadratic ease-in-out — acceleration until halfway, then deceleration.
        /// </summary>
        public static float EaseInOutQuad(float x)
        {
            if (x < 0.5)
                return 0.5f * x * x;
            else
                return 1f - (float)Math.Pow(-2f * x + 2f, 2f) / 2f;
        }

        #endregion


        #region Cubic

        /// <summary>
        /// Cubic ease-in — stronger acceleration than quadratic.
        /// </summary>
        public static float EaseInCubic(float x) => x * x * x;

        /// <summary>
        /// Cubic ease-out — stronger deceleration than quadratic.
        /// </summary>
        public static float EaseOutCubic(float x) => 1f - (float)Math.Pow(1f - x, 3f);

        /// <summary>
        /// Cubic ease-in-out — acceleration then deceleration.
        /// </summary>
        public static float EaseInOutCubic(float x) =>
            x < 0.5 ? 4 * x * x * x : 1 - (float)Math.Pow(-2 * x + 2, 3) / 2;

        #endregion


        #region Quart

        /// <summary>
        /// Quartic ease-in — very strong acceleration.
        /// </summary>
        public static float EaseInQuart(float x) => x * x * x * x;

        /// <summary>
        /// Quartic ease-out — very strong deceleration.
        /// </summary>
        public static float EaseOutQuart(float x) => 1f - (float)Math.Pow(1f - x, 4f);

        /// <summary>
        /// Quartic ease-in-out — fast acceleration and deceleration curve.
        /// </summary>
        public static float EaseInOutQuart(float x) =>
            x < 0.5 ? 8 * x * x * x * x : 1 - (float)Math.Pow(-2 * x + 2, 4) / 2;

        #endregion


        #region Quint

        /// <summary>
        /// Quintic ease-in — extreme acceleration.
        /// </summary>
        public static float EaseInQuint(float x) => x * x * x * x * x;

        /// <summary>
        /// Quintic ease-out — extreme deceleration.
        /// </summary>
        public static float EaseOutQuint(float x) => 1f - (float)Math.Pow(1f - x, 5f);

        /// <summary>
        /// Quintic ease-in-out — very steep ease curve.
        /// </summary>
        public static float EaseInOutQuint(float x) =>
            x < 0.5 ? 16 * x * x * x * x * x : 1 - (float)Math.Pow(-2 * x + 2, 5) / 2;

        #endregion


        #region Sine

        /// <summary>
        /// Sine ease-in — slow start, sinusoidal curve.
        /// </summary>
        public static float EaseInSine(float x) =>
            1f - (float)Math.Cos((x * Math.PI) / 2f);

        /// <summary>
        /// Sine ease-out — slow end, sinusoidal curve.
        /// </summary>
        public static float EaseOutSine(float x) =>
            (float)Math.Sin((x * Math.PI) / 2f);

        /// <summary>
        /// Sine ease-in-out — smooth sinusoidal ease.
        /// </summary>
        public static float EaseInOutSine(float x) =>
            -(float)(Math.Cos(Math.PI * x) - 1f) / 2f;

        #endregion


        #region Expo

        /// <summary>
        /// Exponential ease-in — starts slowly, then rapidly increases.
        /// </summary>
        public static float EaseInExpo(float x) =>
            x == 0f ? 0f : (float)Math.Pow(2f, 10f * x - 10f);

        /// <summary>
        /// Exponential ease-out — starts fast, slows suddenly.
        /// </summary>
        public static float EaseOutExpo(float x) =>
            x == 1f ? 1f : 1f - (float)Math.Pow(2f, -10f * x);

        /// <summary>
        /// Exponential ease-in-out — extreme acceleration and deceleration.
        /// </summary>
        public static float EaseInOutExpo(float x)
        {
            if (x == 0f) return 0f;
            if (x == 1f) return 1f;

            if (x < 0.5f)
                return (float)Math.Pow(2f, 20f * x - 10f) / 2f;
            else
                return (2f - (float)Math.Pow(2f, -20f * x + 10f)) / 2f;
        }

        #endregion


        #region Circ

        /// <summary>
        /// Circular ease-in — starts slow, accelerates in a circular arc.
        /// </summary>
        public static float EaseInCirc(float x) =>
            1f - (float)Math.Sqrt(1f - Math.Pow(x, 2f));

        /// <summary>
        /// Circular ease-out — decelerates in a circular arc.
        /// </summary>
        public static float EaseOutCirc(float x) =>
            (float)Math.Sqrt(1f - Math.Pow(x - 1f, 2f));

        /// <summary>
        /// Circular ease-in-out — circular motion style curve.
        /// </summary>
        public static float EaseInOutCirc(float x)
        {
            if (x < 0.5f)
                return (1f - (float)Math.Sqrt(1f - (float)Math.Pow(2f * x, 2f))) / 2f;
            else
                return ((float)Math.Sqrt(1f - (float)Math.Pow(-2f * x + 2f, 2f)) + 1f) / 2f;
        }

        #endregion


        #region Back

        /// <summary>
        /// Back ease-in — overshoots before accelerating forward.
        /// </summary>
        public static float EaseInBack(float x) =>
            c3 * x * x * x - c1 * x * x;

        /// <summary>
        /// Back ease-out — overshoots then settles.
        /// </summary>
        public static float EaseOutBack(float x) =>
            1f + c3 * (float)Math.Pow(x - 1f, 3f) + c1 * (float)Math.Pow(x - 1f, 2f);

        /// <summary>
        /// Back ease-in-out — overshoots on both sides.
        /// </summary>
        public static float EaseInOutBack(float x)
        {
            if (x < 0.5f)
                return (float)(Math.Pow(2f * x, 2f) * ((c2 + 1f) * 2f * x - c2)) / 2f;
            else
                return (float)(Math.Pow(2f * x - 2f, 2f) * ((c2 + 1f) * (x * 2f - 2f) + c2) + 2f) / 2f;
        }

        #endregion


        #region Elastic

        /// <summary>
        /// Elastic ease-in — starts slow, then snaps forward with oscillation.
        /// </summary>
        public static float EaseInElastic(float x)
        {
            if (x == 0f) return 0f;
            if (x == 1f) return 1f;
            return -(float)(Math.Pow(2f, 10f * x - 10f) * Math.Sin((x * 10f - 10.75f) * c4));
        }

        /// <summary>
        /// Elastic ease-out — overshoots and oscillates before settling.
        /// </summary>
        public static float EaseOutElastic(float x)
        {
            if (x == 0f) return 0f;
            if (x == 1f) return 1f;
            return (float)(Math.Pow(2f, -10f * x) * Math.Sin((x * 10f - 0.75f) * c4)) + 1f;
        }

        /// <summary>
        /// Elastic ease-in-out — dramatic elastic stretch in both directions.
        /// </summary>
        public static float EaseInOutElastic(float x)
        {
            const float c5 = (float)(2 * Math.PI / 4.5);

            if (x == 0f) return 0f;
            else if (x == 1f) return 1f;
            else if (x < 0.5f)
                return -(float)(Math.Pow(2, 20 * x - 10) * Math.Sin((20 * x - 11.125f) * c5)) / 2f;
            else
                return (float)(Math.Pow(2, -20 * x + 10) * Math.Sin((20 * x - 11.125f) * c5)) / 2f + 1f;
        }

        #endregion


        #region Bounce

        /// <summary>
        /// Bounce ease-in — reversed bounce motion.
        /// </summary>
        public static float EaseInBounce(float x) =>
            1f - EaseOutBounce(1f - x);

        /// <summary>
        /// Bounce ease-out — simulates a bouncing ball effect.
        /// </summary>
        public static float EaseOutBounce(float t)
        {
            if (t < 1 / 2.75f)
                return 7.5625f * t * t;
            else if (t < 2 / 2.75f)
            {
                t -= 1.5f / 2.75f;
                return 7.5625f * t * t + 0.75f;
            }
            else if (t < 2.5 / 2.75)
            {
                t -= 2.25f / 2.75f;
                return 7.5625f * t * t + 0.9375f;
            }
            else
            {
                t -= 2.625f / 2.75f;
                return 7.5625f * t * t + 0.984375f;
            }
        }

        /// <summary>
        /// Bounce ease-in-out — combined bounce at start and end.
        /// </summary>
        public static float EaseInOutBounce(float x)
        {
            if (x < 0.5f)
                return (1f - EaseOutBounce(1f - 2f * x)) / 2f;
            else
                return (1f + EaseOutBounce(2f * x - 1f)) / 2f;
        }

        #endregion
    }
}
