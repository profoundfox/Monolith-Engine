using System;
using Microsoft.Xna.Framework;

namespace Monolith.Geometry
{
  ///<summary>
  /// A struct with a width and height, used primarily for geometrical shapes without a location.
  ///</summary>
  public readonly struct Extent : IEquatable<Extent>
  {
    ///<summary>
    /// The width of this extent.
    ///</summary>
    public int Width { get; }

    ///<summary>
    /// The height of this extent.
    ///</summary>
    public int Height { get; }

    /// <summary>
    /// Creates a new extent with specified <see cref="Width"/> and <see cref="Height"/>.
    /// </summary>
    /// <param name="width">Value for <see cref="Width"/>.</param>
    /// <param name="height">Value for <see cref="Height"/>.</param>
    public Extent(int width, int height)
    {
      Width = width;
      Height = height;
    }

    ///<summary>
    /// Creates a new extent with unified parameters for <see cref="Width"/> and <see cref="Height"/>
    ///</summary>
    ///<param name="unified">The unified value which both <see cref="Width"/> and <see cref="Height"/> are set to.</param>
    public Extent(int unified)
    {
      Width = unified;
      Height = unified;
    }

    ///<summary>
    /// Creates a new extent with a point as the reference type.
    ///</summary>
    ///<param name="value">The point; the X value sets <see cref="Width"/> and Y sets <see cref="Height">.</param>
    public Extent(Point value)
    {
      Width = value.X;
      Height = value.Y;
    }

    ///<summary>
    /// Checks if this extent is equal to another specified extent.
    /// It checks if the width and height are equal to one another.
    ///</summary>
    ///<param name="other">The other extent.</param>
    public bool Equals(Extent other)
    {
      return Width == other.Width && Height == other.Height;
    }

    ///<summary>
    /// Checks if this is equal to another specified object.
    ///</summary>
    ///<param name="obj">The object in question.</param>
    public override bool Equals(object? obj)
    {
      return obj is Extent other && Equals(other);
    }

    ///<summary>
    /// Turns this extent into a unique hashcode based on the width and height.
    ///</summary>
    public override int GetHashCode()
    {
      return HashCode.Combine(Width, Height);
    }

    ///<summary>
    /// Checks if left is equal to right.
    ///</summary>
    ///<param name="left">The left extent.</param>
    ///<param name="right">The right extent.</param>
    public static bool operator ==(Extent left, Extent right)
    {
      return left.Equals(right);
    }

    ///<summary>
    /// Checks if left is not equal to the right.
    ///</summary>
    ///<param name="left">The left extent.</param>
    ///<param name="right">The right exten.</param>
    public static bool operator !=(Extent left, Extent right)
    {
      return !left.Equals(right);
    }

    ///<summary>
    /// Turns this into a printable string.
    ///</summary>
    public override string ToString()
    {
      return $"Width: {Width}, Height: {Height}";
    }
  }
}

