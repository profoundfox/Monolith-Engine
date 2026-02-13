using System;
using Monolith.Nodes;

namespace Monolith.Attributes
{    
    public abstract record class NodeConfig
    {   
        /// <summary>
        /// The name of the node
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// The parent of the node.
        /// </summary>
        public Node Parent { get; init; }

        public abstract Type NodeType { get; }
    }

}