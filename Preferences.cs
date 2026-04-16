using System.Collections.Generic;
using Monolith.Prefs;

namespace Monolith
{
    public class Preferences
    {
        private readonly List<IPrefSection> sections = new();

        /// <summary>
        /// The preferences for the screen, such as fullscreen and the render size.
        /// </summary>
        public GraphicsPrefs Graphics { get; }

        ///<summary>
        /// General preferences, such as the title.
        ///</summary>
        public GeneralPrefs General { get; }

        ///<summary>
        /// The preferences for audio.
        ///</summary>
        public AudioPrefs Audio { get; }

        /// <summary>
        /// Engine and Monogame wide preferences.
        /// </summary>
        public Preferences()
        {
            Graphics = Register(new GraphicsPrefs());
            General = Register(new GeneralPrefs());
            Audio = Register(new AudioPrefs());
        }

        /// <summary>
        /// Registers an new section, which can be configured and will be marked as dirty; so that its apply can be called.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="section">The section which will be added.</param>
        /// <returns></returns>
        public T Register<T>(T section) where T : IPrefSection
        {
            sections.Add(section);
            return section;
        }

        /// <summary>
        /// Applies the currently chosen settings for all modified preference sections.
        /// </summary>
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
