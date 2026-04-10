using System;

namespace Monolith.Attributes
{
  public class Dual<T>
  {
      private T local;

      public T Local
      {
          get => local;
          set
          {
              local = value;
              OnChanged?.Invoke();
          }
      }

      public T Global { get; internal set; }

      internal Action OnChanged;

      public Dual(T initial)
      {
          local = initial;
          Global = initial;
      }
  }
}
