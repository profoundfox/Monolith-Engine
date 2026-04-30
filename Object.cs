using System;
using System.Threading;

namespace Monolith
{
  ///<summary>
  /// The absolute abstract base class that all others inherit.
  ///</summary>
  public abstract class Object
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

    public Object()
    {
      _id = Interlocked.Increment(ref _nextId);
    }

    ///<summary>
    /// If the object on the left is equal to the instance on the right.
    ///</summary>
    public override bool Equals(object obj)
    {
      return obj is Tracked other && _id == other._id;
    }

    ///<summary>
    /// Checks if left is equal to right.
    ///</summary>
    ///<param name="left">The left extent.</param>
    ///<param name="right">The right extent.</param>
    public static bool operator ==(Object left, Object right)
    {
      return left.Equals(right);
    }

    ///<summary>
    /// Checks if left is not equal to the right.
    ///</summary>
    ///<param name="left">The left extent.</param>
    ///<param name="right">The right exten.</param>
    public static bool operator !=(Object left, Object right)
    {
      return !left.Equals(right);
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
