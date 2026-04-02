namespace Monolith.Geometry
{
    public struct Extent
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Extent(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public override string ToString() => $"Width: {Width}, Height: {Height}";
    }
}