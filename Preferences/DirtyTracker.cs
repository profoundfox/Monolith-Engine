using System.Collections.Generic;

namespace Monolith.Prefs
{
    public class DirtyTracker
    {
        public bool IsDirty { get; private set; }

        public void Set<T>(ref T field, T value)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                IsDirty = true;
            }
        }

        public void MarkClean() => IsDirty = false;
    }
}