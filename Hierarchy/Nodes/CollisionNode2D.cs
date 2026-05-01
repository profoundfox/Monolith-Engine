
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using Microsoft.Xna.Framework;
using Monolith.Geometry;
using Monolith.Params;
using Monolith.Tools;

namespace Monolith.Hierarchy
{

  public class CollisionNode2D : Node2D
  {
    public List<CollisionShape2D> CollisionShapes { get => GetAll<CollisionShape2D>().ToList(); }

    ///<summary>
    /// The max layer.
    ///</summary>
    [Export]
    public int MaxLayer { get; set; } = 30;

    ///<summary>
    /// The layers for this collision node; used while checking intersection.
    /// If shapes share one or more layers they can intersect, zero layered shapes intersect with eachother.
    ///</summary>
    [Export]
    public List<int> Layers { get; private set; }

    ///<summary>
    /// The bounds of this node's shapes, represented in the form of a rectangle.
    /// e.g a circle's bounds are the top, bottom, left and right.
    ///</summary>
    [Export]
    public List<Rectangle> Bounds
    {
      get
      {
        if (CollisionShapes.Count == 0)
          return [Rectangle.Empty];

        var b = new List<Rectangle>();

        foreach (var c in CollisionShapes)
          b.Add(c.Shape.GetAABB(c.Transform.Global.Position.ToPoint()));

        return b;
      }
    }

    public CollisionNode2D() { }

    ///<summary>
    /// Adds a layer which will be used for checking intersection.
    ///</summary>
    ///<param name="layer">The layer, if it surpases the max value (default: 30); it will be clamped down.</param>
    public int AddLayer(int layer)
    {
      var finVal = Math.Clamp(layer, 0, MaxLayer);

      Layers.Add(finVal);

      return finVal;
    }

    ///<summary>
    /// Adds layers which will be used for checking intersection.
    ///</summary>
    ///<param name="layers">The layers, if they surpass the max value (default: 30); they will be clamped down.</param>
    public int[] AddLayers(params int[] layers)
    {
      var finVals = layers.ClampArray(0, MaxLayer);

      Layers.AddRange(finVals);

      return finVals;
    }

    ///<summary>
    /// Removes a layer.
    ///</summary>
    ///<param name="layer">The layer in question, if it surpasses the max value (default: 30), it will be clamped down</param>
    public int RemoveLayer(int layer)
    {
      var finVal = Math.Clamp(layer, 0, MaxLayer);

      Layers.Remove(finVal);

      return finVal;
    }

    ///<summary>
    /// Removes multiple layers.
    ///</summary>
    ///<param name="layers">The layers in question, if they surpass the max value (default: 30), they will be clamped down</param>
    public int[] RemoveLayers(params int[] layers)
    {
      var finVals = layers.ClampArray(0, MaxLayer);

      Layers.RemoveAll(item => layers.Contains(item));

      return finVals;
    }

    ///<summary>
    /// A check for whether the shape is valid for intersection.
    ///</summary>
    ///<param name="shape">The shape in question.</param>
    private bool IsValid(CollisionShape2D shape)
    {
      return shape.Disabled == false && shape?.Shape != null;
    }

    ///<summary>
    /// Checks whether this shape intersects with another specified shape.
    ///</summary>
    ///<param name="other">The other shape.</param>
    public bool Intersects(CollisionNode2D other)
    {
      return this.CollisionShapes.Any(
          myShape => other.CollisionShapes.Any(
              otherShape => myShape.Intersects(otherShape)
               && IsValid(myShape) && IsValid(otherShape)
          ));
    }

    ///<summary>
    /// Checks whether this shape intersects with a given shape at a specified offset.
    ///</summary>
    ///<param name="offset">The offset; applied to this shape's global position </param>
    ///<param name="other">The other shape, the offset is not applied to it.</param>
    public bool IntersectsAt(Vector2 offset, CollisionNode2D other)
    {
      return this.CollisionShapes.Any(
          myShape => other.CollisionShapes.Any(
              otherShape => myShape.IntersectsAt(offset, otherShape)
               && IsValid(myShape) && IsValid(otherShape)
          ));
    }

    ///<summary>
    /// Checks if this shape contains a specified position.
    ///</summary>
    ///<param name="position"> </param>
    public bool Contains(Vector2 position)
    {
      return CollisionShapes.Any(
          c => c.Shape.Contains(position.ToPoint(), Transform.Global.Position.ToPoint()) && IsValid(c));
    }

    public override void _EnterTree()
    {
      base._EnterTree();
    }

    public override void _ExitTree()
    {
      base._ExitTree();
    }

    public override void _PhysicsUpdate(float delta)
    {
      base._PhysicsUpdate(delta);
    }

    public override void _Process(float delta)
    {
      base._Process(delta);
    }

    public override void _SubmitCall()
    {
      base._SubmitCall();
    }
  }
}
