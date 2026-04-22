using System;

namespace Monolith.Util
{
  public static class Await
  {

    ///<summary>
    /// Waits for a span of time then calls an action.
    ///</summary>
    ///<param name="time">The length as the well as the unit of time used.</param>
    ///<param name="then">The action that will be called once the timer is finished.</param>
    ///<param name="ignoreTimeScale">Wether this timer should obey to timescale standards.</param>
    public static void Span(TimeSpan time, Action then, bool ignoreTimeScale = false)
    {
      Engine.Time.After(time, () => Engine.Tree.Post(then), ignoreTimeScale);
    }

    ///<summary>
    /// Waits until a condition is true then calls an action.
    ///</summary>
    ///<param name="condition">The condition; once true the timer is finished.</param>
    ///<param name="then">The action which will be called once the timer is finished.</param>
    ///<param name="interval">The interval between checks (default = 16ms).</param>
    ///<param name="ignoreTimeScale">Wether this timer should obey to timescale standards.</param>
    public static void Until(
        Func<bool> condition,
        Action then,
        TimeSpan? interval = null,
        bool ignoreTimeScale = false)
    {
      var checkInterval = interval ?? TimeSpan.FromMilliseconds(16);

      void Check()
      {
        if (condition())
        {
          Engine.Tree.Post(then);
        }
        else
        {
          Engine.Time.After(checkInterval, Check, ignoreTimeScale);
        }
      }

      Check();
    }
  }
}
