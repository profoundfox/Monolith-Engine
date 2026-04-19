using System;

namespace Monolith.Params
{
  ///<summary>
  /// 
  ///</summary>
  public class Dual<T>
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
