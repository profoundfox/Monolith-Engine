using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Geometry;
using Monolith.Graphics;
using Monolith.Tools;
using Monolith.Managers;
using Monolith.Params;
using System.IO.Compression;

namespace Monolith.Hierarchy
{

  public class Node2D : CanvasNode
  {
    public Dual<Transform2D> Transform { get; private set; }

    /// <summary>
    /// The self contained position of this node, updates child nodes' position.
    /// </summary>
    [Export]
    public Vector2 LocalPosition
    {
      get => Transform.Local.Position;
      set
      {
        Transform.Local = Transform.Local with { Position = value };
      }
    }

    /// <summary>
    /// The self contained rotation of this node, updates child node's rotation.
    /// </summary>
    public float LocalRotation
    {
      get => Transform.Local.Rotation;
      set
      {
        Transform.Local = Transform.Local with { Rotation = value };
      }
    }

    /// <summary>
    /// The self contained scale of this node, updates child node's scale.
    /// </summary>
    public Vector2 LocalScale
    {
      get => Transform.Local.Scale;
      set
      {
        Transform.Local = Transform.Local with { Scale = value };
      }
    }

    /// <summary>
    /// Signal for when the transform changes.
    /// </summary>
    public event Action<Transform2D> OnTransformChanged;


    /// <summary>
    /// Creates a new Node2D using a SpatialNodeConfig.
    /// </summary>
    public Node2D()
    {
      Transform = new(Transform2D.Identity);
      Transform.OnChanged += UpdateGlobalTransform;

      UpdateGlobalTransform();
      OnParentChanged += (node) =>
      {
        UpdateGlobalTransform();
      };
    }

    /// <summary>
    /// Recompute global transform based on parent.
    /// Automatically ProcessProcessProcessProcessUpdates children.
    /// </summary>
    internal void UpdateGlobalTransform()
    {
      if (GetParent() is Node2D parent2D)
      {
        Transform.Global = Transform2D.Combine(parent2D.Transform.Global, Transform.Local);
      }
      else
      {
        Transform.Global = Transform.Local;
      }

      OnTransformChanged?.Invoke(Transform.Global);


      foreach (var child in Children)
      {
        if (child is Node2D c2d)
          c2d.UpdateGlobalTransform();
      }
    }

    /// <summary>
    /// An offset function for adding onto the node's LocalPosition with a Vector2.
    /// Acts the same as +=.
    /// </summary>
    /// <param name="delta"></param>
    public void Offset(Vector2 delta)
    {
      LocalPosition += delta;
    }

    /// <summary>
    /// An offset function for adding onto the node's LocalPosition.
    /// Acts the same as +=.
    /// </summary>
    /// <param name="delta"></param>
    public void Offset(float x, float y)
    {
      Offset(new Vector2(x, y));
    }

    public override void OnEnter()
    {
      base.OnEnter();
    }

    public override void OnExit()
    {
      base.OnExit();
    }

    public override void ProcessUpdate(float delta)
    {
      base.ProcessUpdate(delta);
    }

    public override void SubmitCall()
    {
      base.SubmitCall();
    }
  }
}
