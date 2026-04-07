using System.Collections.Generic;
using Monolith.Prefs;

namespace Monolith
{
    public class Preferences
    {
        private readonly List<IPrefSection> sections = new();

        public GraphicsPreferences Graphics { get; }

        public Preferences()
        {
            Graphics = Register(new GraphicsPreferences());
        }

        private T Register<T>(T section) where T : IPrefSection
        {
            sections.Add(section);
            return section;
        }

        public void Apply()
        {
            foreach (var section in sections)
            {
                if (section.IsDirty)
                {
                    section.Apply();
                }
            }
        }
    }
}