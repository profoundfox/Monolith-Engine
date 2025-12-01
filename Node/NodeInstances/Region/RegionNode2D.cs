using System;
using Microsoft.Xna.Framework;
using Monolith;
using Monolith.Geometry;
using Monolith.Nodes;

namespace Monolith.Nodes
{
    public record class RegionNodeConfig : Node2DConfig
    {
        public IRegionShape2D Region { get; set; }
    }

    public class RegionNode2D : Node2D, IRegionShape2D
    {
        public IRegionShape2D Region;

        public int Width { get => Region.Width; set => Region.Width = value; }
        public int Height { get => Region.Height; set => Region.Height = value; }
        
        public Point Location { get => Region.Location; set => Region.Location = value; }

        public Rectangle BoundingBox => Region.BoundingBox;

        public RegionNode2D(RegionNodeConfig cfg) : base(cfg)
        {
            Region = cfg.Region;
        }

        public void Offset(int x, int y) => Region.Offset(x, y);
        public bool Contains(Point p) => Region.Contains(p);
        public bool Contains(IRegionShape2D other) => Region.Contains(other);
        public bool Intersects(IRegionShape2D other) => Region.Intersects(other);

        public bool RayIntersect(Vector2 rayOrigin, Vector2 rayDir,
                                float maxLength, out Vector2 hitPoint,
                                out float distance)
            => Region.RayIntersect(rayOrigin, rayDir, maxLength, out hitPoint, out distance);

        public IRegionShape2D Clone() => Region.Clone();
    }

}