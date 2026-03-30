using System;
using System.IO;

namespace Monolith.Tools
{
    public static class PathTools
    {
        public static string Base => AppContext.BaseDirectory;

        public static string Combine(params string[] parts)
            => Path.Combine(Base, Path.Combine(parts));
    }
}