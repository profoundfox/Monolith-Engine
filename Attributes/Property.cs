namespace Monolith.Attributes
{
    public interface IProperty<TSelf>
        where TSelf : struct
    {
        static abstract TSelf Combine(in TSelf parent, in TSelf child);
    }
}