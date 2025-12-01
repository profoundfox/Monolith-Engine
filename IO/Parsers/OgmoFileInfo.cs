using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace Monolith.IO
{
    public class OgmoFileInfo
    {
        public class Root
        {
            public string ogmoVersion { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public int offsetX { get; set; }
            public int offsetY { get; set; }
            public List<Layer> layers { get; set; }
        }

        public class Layer
        {
            public string texturePath { get; set; }
            public string name { get; set; }
            public string _eid { get; set; }
            public int offsetX { get; set; }
            public int offsetY { get; set; }
            public int gridCellWidth { get; set; }
            public int gridCellHeight { get; set; }
            public int gridCellsX { get; set; }
            public int gridCellsY { get; set; }
            
            public float depth {get; set;}


            public string tileset { get; set; }
            public string region { get; set; }
            public List<int> data { get; set; }

            public List<Entity> entities { get; set; }
            public List<Decal> decals { get; set; }
            
            public string folder { get; set; }

            public int exportMode { get; set; }
            public int arrayMode { get; set; }
        }
        
        
        public class Decal
        {
            public int x { get; set; }
            
            public int y { get; set; }
            
            public int scaleX { get; set; }
            
            public int scaleY { get; set; }
            
            public float rotation { get; set; }
            
            public string texture { get; set; }
            
            public float originX { get; set; }
            
            public float originY { get; set; }
        }
        
        public class Entity
        {
            public string name { get; set; }
            public int x { get; set; }
            public int y { get; set; }
            public int width { get; set; } = 0;
            public int height { get; set; } = 0;

            public Dictionary<string, JsonElement> values { get; set; }

            public List<Node> nodes { get; set; }
            
        }

        public class Node
        {
            public int x { get; set; }
            public int y { get; set;}
        }


        public static List<string> GetInfo(string path)
        {
            List<string> jsonLines = File.ReadAllLines(path).ToList();
            return jsonLines;
        }

    }

}