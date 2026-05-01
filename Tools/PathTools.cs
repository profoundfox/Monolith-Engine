using System;
using System.IO;

namespace Monolith.Tools
{
  public static class PathTools
  {
    public static string BaseObject => AppContext.BaseDirectory;

    public static string Combine(params string[] parts)
        => Path.Combine(BaseObject, Path.Combine(parts));
  }
}
