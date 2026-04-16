namespace Monolith.Prefs
{
    public class AudioPrefs : IPrefSection
    {
        private readonly DirtyTracker tracker = new();

        public bool IsDirty => tracker.IsDirty;

        public void Apply()
        {

        }
    }
}
