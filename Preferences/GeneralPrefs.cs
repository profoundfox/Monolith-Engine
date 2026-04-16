namespace Monolith.Prefs
{
    public class GeneralPrefs : IPrefSection
    {
        private readonly DirtyTracker tracker = new();

        private string title;
        public string Title
        {
            get => title;
            set => tracker.Set(ref title, value);
        }

        public bool IsDirty => tracker.IsDirty;

        public void Apply()
        {
            Engine.Instance.Window.Title = title;
        }
    }
}
