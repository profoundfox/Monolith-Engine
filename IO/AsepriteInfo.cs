using System.Collections.Generic;

namespace Monolith.IO
{
    public class AsepriteFrameRect {
        public int x { get; set; }
        public int y { get; set; }
        public int w { get; set; }
        public int h { get; set; }
    }

    public class AsepriteFrame {
        public AsepriteFrameRect frame { get; set; }
        public int duration { get; set; }
    }

    public class AsepriteTag {
        public string name { get; set; }
        public int from { get; set; }
        public int to { get; set; }
        public string direction { get; set; }
    }

    public class AsepriteMeta {
        public List<AsepriteTag> frameTags { get; set; }
    }

    public class AsepriteData {
        public Dictionary<string, AsepriteFrame> frames { get; set; }
        public AsepriteMeta meta { get; set; }
    }
}