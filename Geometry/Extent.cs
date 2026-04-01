namespace Monolith.Geometry
{
    public struct Extent
    {
        public float Width { get; set; }
        public float Height { get; set; }

        public Extent(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public override string ToString() => $"Width: {Width}, Height: {Height}";
    }
}