using Monolith.Nodes;

namespace Monolith.Attributes
{    
    public record class NodeConfig
    {
        /// <summary>
        /// Optional parent object.
        /// </summary>
        public Node Parent { get; set; }

        /// <summary>
        /// Optional name. Defaults to the node class name.
        /// </summary>
        public string Name { get; set; }
    }
}