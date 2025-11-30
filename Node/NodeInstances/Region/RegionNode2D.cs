using System;
using Microsoft.Xna.Framework;
using Monolith;
using Monolith.Geometry;
using Monolith.Nodes;

public record class RegionNodeConfig : Node2DConfig
{
    public IRegionShape2D Shape { get; set; }
}

public class RegionNode2D : Node2D
{
    public IRegionShape2D Shape;

    public int Width { get => Shape.Width; set => Shape.Width = value; }
    public int Height { get => Shape.Height; set => Shape.Height = value; }
    
    public Point Location { get => Shape.Location; set => Shape.Location = value; }

    public RegionNode2D(RegionNodeConfig cfg) : base(cfg)
    {
        Shape = cfg.Shape;
    }

    public void Offset(int x, int y)
    {
        Shape.Offset(x, y);
    }

    public bool Contains(Point p)
    {
        return Shape.Contains(p);
    }

    public bool Contains(IRegionShape2D other)
    {
        return Shape.Contains(other);
    }
    
    public bool Intersects(IRegionShape2D other)
    {
        return Shape.Intersects(other);
    }

    public bool RayIntersect(Vector2 rayOrigin, Vector2 rayDir, float maxLength, out Vector2 hitPoint, out float distance)
    {
        return Shape.RayIntersect(rayOrigin, rayDir, maxLength, out hitPoint, out distance);
    }




}