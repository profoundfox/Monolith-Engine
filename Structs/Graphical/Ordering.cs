namespace Monolith.Structs
{
    public readonly record struct Ordering 
    {
        public int Depth { get; init; }
        public bool RelativeDepth { get; init; }
        public static readonly Ordering Identity =
            new(0, false);

        public Ordering(int depth, bool relativeDepth)
        {
            Depth = depth;
            RelativeDepth = relativeDepth;
        }

        public static Ordering Combine(
            in Ordering parent,
            in Ordering child
        )
        {
            int depth = child.RelativeDepth
                ? parent.Depth + child.Depth
                : child.Depth;

            return new Ordering(depth, child.RelativeDepth);
        }
    }
}