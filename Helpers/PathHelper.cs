using System;
using System.IO;

namespace Monolith.Helpers
{
    public static class PathHelper
    {
        public static string Base => AppContext.BaseDirectory;

        public static string Combine(params string[] parts)
            => Path.Combine(Base, Path.Combine(parts));
    }
}