using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Monolith.Managers;
using Monolith.Params;
using Monolith.Runtime;

namespace Monolith
{
  ///<summary>
  /// The absolute abstract class for other classes.
  ///</summary>
  public abstract class Tracked : BaseObject
  {
    ///<summary>
    /// The name of this instance.
    ///</summary>
    ///<remarks>
    /// Since names are not always unique, same named instances are grouped together.
    ///</remarks>
    [Export]
    public string Name { get; set; }

    public Tracked()
    {
      Engine.Index.QueueAdd(this);
    }

    ///<summary>
    /// Queues this instance to be removed from <see cref="Index"/>.
    ///</summary>
    ///<remarks>
    /// It will be removed at the end of this frame.
    ///</remarks>
    public void QueueFree()
    {
      Engine.Index.QueueRemove(this);
    }

    ///<summary>
    /// Immediately removes this instance from <see cref="Index">.
    ///</summary>
    public void FreeImmediate()
    {
      Engine.Index.RemoveNow(this);
    }

    ///<summary>
    /// Removes all data associated with this instance.
    ///</summary>
    internal virtual void ClearData() { }
  }
}
