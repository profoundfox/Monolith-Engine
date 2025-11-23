using System;
using System.Threading.Tasks;

namespace Monolith.Util
{
    public static class CTimer
    {
        /// <summary>
        /// Waits for seconds represented as a float, calls an action once the timer runs out.
        /// </summary>
        /// <param name="seconds"></param>
        /// <param name="callback"></param>
        public static async void Wait(float seconds, Action callback)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));
            callback?.Invoke();
        }
    }
}