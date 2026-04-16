using Monolith.Geometry;
using Monolith.Prefs;

namespace Monolith
{

    public class GraphicsPrefs : IPrefSection
    {
        private readonly DirtyTracker tracker = new();

        private bool fullscreen;
        public bool Fullscreen
        {
            get => fullscreen;
            set => tracker.Set(ref fullscreen, value);
        }

        private Extent renderSize = new Extent(640, 360);
        public Extent RenderSize
        {
            get => renderSize;
            set => tracker.Set(ref renderSize, value);
        }

        public bool IsDirty => tracker.IsDirty;

        public void Apply()
        {
            Engine.Graphics.IsFullScreen = Fullscreen;

            Engine.Canvas.RenderSize = RenderSize;

            Engine.Canvas.UpdateTransform();
            Engine.Graphics.ApplyChanges();
        }
    }
}
