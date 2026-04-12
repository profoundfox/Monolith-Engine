using System;

namespace Monolith.Util
{
    public static class Await
    {
        public static void Seconds(float seconds, Action then, bool ignoreTimeScale = false)
        {
            Engine.Time.After(TimeSpan.FromSeconds(seconds), () =>
            {
                Engine.Tree.Post(then);
            }, ignoreTimeScale);
        }
    }
}
