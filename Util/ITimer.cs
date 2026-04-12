using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Monolith.Util
{
  public interface ITimer
  {
      ///<summary>
      /// The action which will be called when the timer finishes.
      ///</summary>
      Action Callback { get; set; }
      
      ///<summary>
      /// Whether the timer is cancelled or not.
      ///</summary>
      bool Cancelled { get; set; }

      ///<summary>
      /// Updates the timer; when it returns false the timer has finished.
      ///</summary>
      bool Update(TimeSpan scaledDelta, TimeSpan unscaledDelta);
  }
}
