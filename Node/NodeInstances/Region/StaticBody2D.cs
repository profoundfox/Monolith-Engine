using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monolith.Geometry;


namespace Monolith.Nodes
{
    
    public record class StaticBody2DConfig : Node2DConfig {}
    public class StaticBody2D : Node2D
    {
        public StaticBody2D(StaticBody2DConfig config) : base(config) {}
        
    }
}