using System;
using Monolith.Managers;

namespace Monolith.Util
{
  ///<summary>
  /// A struct keeping information for a timer.
  ///</summary>
  internal struct DelayedAction
  {
    ///<summary>
    /// The time remaining, represented as <see cref="TimeSpan"/>.
    ///</summary>
    public TimeSpan Remaining;

    ///<summary>
    /// The action which will be called once finished.
    ///</summary>
    public Action Callback;
    
    ///<summary>
    /// A toggle for whether <see cref="TimeOwner.TimeScale"/> should be ignored.
    ///</summary>
    ///<remarks>
    /// When toggled; the speed of this will not be altered.
    /// This is encouraged to be set when having a timer that itself sets the timescale.
    ///</remarks>
    public bool IgnoreTimeScale;

    public DelayedAction(TimeSpan remaining, Action callback, bool ignoreTimeScale)
    {
      Remaining = remaining;
      Callback = callback;
      IgnoreTimeScale = ignoreTimeScale;
    }
  }
}
