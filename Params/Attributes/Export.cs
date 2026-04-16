using System;

namespace Monolith.Params
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
  public sealed class ExportAttribute : Attribute {  }

  public sealed class PropertyMeta
  {
    public Func<object, object> Get;
    public Action<object, object> Set;
  }
}
