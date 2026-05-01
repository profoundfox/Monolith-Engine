using System;
using System.Threading;
using Monolith.Params;

namespace Monolith
{
  ///<summary>
  /// The absolute abstract base class that all others inherit.
  ///</summary>
  public abstract class BaseObject
  {
    ///<summary>
    /// Action for when a dynamic setter fails.
    ///</summary>
    [Export]
    public Action<string, object> OnSetFallback { get; set; }

    ///<summary>
    /// Action for when a dynamic getter fails.
    ///</summary>
    [Export]
    public Action<string> OnFallback { get; set; }
    
    private static int _nextId = 0;
    private readonly int _id;

    public BaseObject()
    {
      _id = Interlocked.Increment(ref _nextId);
    }

    ///<summary>
    /// If the object on the left is equal to the object on the right.
    ///</summary>
    public override bool Equals(object obj)
    {
      return obj is BaseObject other && _id == other._id;
    }

    ///<summary>
    /// Checks if left is equal to right.
    ///</summary>
    ///<param name="left">The left extent.</param>
    ///<param name="right">The right extent.</param>
    public static bool operator ==(BaseObject left, BaseObject right)
    {
      if (ReferenceEquals(left, right)) return true;
      if (left is null || right is null) return false;
      return left.Equals(right);
    }

    ///<summary>
    /// Checks if left is not equal to the right.
    ///</summary>
    ///<param name="left">The left extent.</param>
    ///<param name="right">The right exten.</param>
    public static bool operator !=(BaseObject left, BaseObject right)
    {
      return !(left == right);
    }

    ///<summary>
    /// Gets the unique id of this object.
    ///</summary>
    ///<remarks>
    /// Since objects do not have many differentiating properties, it is not varied.
    ///</remarks>
    public override int GetHashCode()
    {
      return _id;
    }

    ///<summary>
    ///
    ///</summary>
    public override string ToString()
    {
      return ($"{GetType().Name}#{GetHashCode()}");
    }
  }
}
