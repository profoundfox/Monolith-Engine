using Monolith.Geometry;

public interface ICollidable2D
{
    bool Collidable { get; set; }
    bool OneWay { get; set; }

    IRegionShape2D Region { get; set; }
}
