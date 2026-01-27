using System.Collections.Generic;
using System.Text.Json;

namespace Monolith.IO
{
    public class OgmoFileInfo
    {
        public class Root
        {
            public string OgmoVersion { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public int OffsetX { get; set; }
            public int OffsetY { get; set; }
            public string BackgroundColor { get; set; }
            public List<Layer> Layers { get; set; }
        }

        public class Layer
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public int OffsetX { get; set; }
            public int OffsetY { get; set; }
            public float Depth { get; set; }

            public string Tileset { get; set; }
            public string Region { get; set; }
            public int GridCellWidth { get; set; }
            public int GridCellHeight { get; set; }
            public int GridCellsX { get; set; }
            public int GridCellsY { get; set; }
            public List<int> Data { get; set; }

            public List<Entity> Entities { get; set; }

            public List<Decal> Decals { get; set; }

            public string Folder { get; set; }
            public int ExportMode { get; set; }
            public int ArrayMode { get; set; }
        }

        public class Decal
        {
            public int X { get; set; }
            public int Y { get; set; }
            public float ScaleX { get; set; } = 1f;
            public float ScaleY { get; set; } = 1f;
            public float Rotation { get; set; } = 0f;
            public string Texture { get; set; }
            public float OriginX { get; set; } = 0f;
            public float OriginY { get; set; } = 0f;
        }

        public class Entity
        {
            public string Name { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
            public int Width { get; set; } = 0;
            public int Height { get; set; } = 0;
            public Dictionary<string, JsonElement> Values { get; set; }
            public List<Node> Nodes { get; set; }
        }

        public class Node
        {
            public int X { get; set; }
            public int Y { get; set; }
        }
    }
}
