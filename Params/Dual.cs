using System;

namespace Monolith.Params
{
  ///<summary>
  /// A property which has two instances, one global and one local. 
  ///</summary>
  public class Dual<T> : BaseObject
  {
    private T local;

    ///<summary>
    /// The local value, calls <see cref="OnChanged"/>.
    ///</summary>
    public T Local
    {
      get => local;
      set
      {
        local = value;
        OnChanged?.Invoke();
      }
    }

    ///<summary>
    /// The global value.
    ///</summary>
    public T Global { get; internal set; }

    ///<summary>
    /// The action which gets called once <see cref="Local"/> gets set.
    ///</summary>
    public event Action OnChanged;

    public Dual(T initial)
    {
      local = initial;
      Global = initial;
    }
  }
}
