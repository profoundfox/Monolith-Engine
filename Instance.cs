using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Monolith.Managers;
using Monolith.Params;

namespace Monolith
{
  ///<summary>
  /// The absolute abstract class for other classes.
  ///</summary>
  public abstract class Instance
  {
    ///<summary>
    /// Action for when a dynamic setter fails.
    ///</summary>
    public Action<string, object> OnSetFallback;

    ///<summary>
    /// Action for when a dynamic getter fails.
    ///</summary>
    public Action<string> OnFallback;

    private static int _nextId = 0;
    private readonly int _id;
    
    ///<summary>
    /// The name of this instance.
    ///</summary>
    ///<remarks>
    /// Since names are not always unique, same named instances are grouped together.
    ///</remarks>
    [Export]
    public string Name { get; set; }

    public Instance()
    {
      _id = Interlocked.Increment(ref _nextId);
    }
    
    ///<summary>
    /// Queues this instance to be removed from <see cref="TreeServer2D"/>.
    ///</summary>
    ///<remarks>
    /// It will be removed at the end of this frame.
    ///</remarks>
    public void QueueFree()
    {
      Engine.Tree.QueueRemove(this);
    }

    ///<summary>
    /// Immediately removes this instance from <see cref="TreeServer2D">.
    ///</summary>
    public void FreeImmediate()
    {
      Engine.Tree.RemoveNow(this);
    }
    
    ///<summary>
    /// Gets the unique id of this instance.
    ///</summary>
    ///<remarks>
    /// Since instances do not have many differentiating properties, it is not varied.
    ///</remarks>
    public override int GetHashCode()
    {
      return _id;
    }
    
    ///<summary>
    /// If the instance on the left is equal to the instance on the right.
    ///</summary>
    public override bool Equals(object obj)
    {
      return obj is Instance other && _id == other._id;
    }
    
    ///<summary>
    /// Removes all data associated with this instance.
    ///</summary>
    internal virtual void ClearData() {}
  }
}
