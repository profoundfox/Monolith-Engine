using System;
using System.IO.Compression;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Params;

namespace Monolith.Hierarchy
{
  public class CanvasNode : Node
  {

    [Export]
    public Dual<Visibility> Visibility { get; private set; }

    [Export]
    public Dual<Ordering> Ordering { get; private set; }

    [Export]
    public Dual<Material> Material { get; private set; }

    /// <summary>
    /// The self contained visibility of this node. 
    /// </summary>
    [Export]
    public bool LocalVisible
    {
      get => Visibility.Local.Visibile;
      set
      {
        Visibility.Local = Visibility.Local with { Visibile = value };
      }
    }

    /// <summary>
    /// The self contained modulate of this node.
    /// </summary>
    [Export]
    public Color LocalModulate
    {
      get => Visibility.Local.Modulate;
      set
      {
        Visibility.Local = Visibility.Local with { Modulate = value };
      }
    }

    /// <summary>
    /// The self contained depth of this node.
    /// </summary>
    [Export]
    public int LocalDepth
    {
      get => Ordering.Local.Depth;
      set
      {
        Ordering.Local = Ordering.Local with { Depth = value };
      }
    }

    /// <summary>
    /// The self contained shader of this node.
    /// </summary>
    [Export]
    public Effect LocalShader
    {
      get => Material.Local.Shader;
      set
      {
        Material.Local = Material.Local with { Shader = value };
      }
    }

    /// <summary>
    /// The self contained sprite effects of this node.
    /// </summary>
    [Export]
    public SpriteEffects LocalSpriteEffects
    {
      get => Material.Local.SpriteEffects;
      set
      {
        Material.Local = Material.Local with { SpriteEffects = value };
      }
    }

    public CanvasNode()
    {
      Visibility = new Dual<Visibility>(Params.Visibility.Identity);
      Ordering = new Dual<Ordering>(Params.Ordering.Identity);
      Material = new Dual<Material>(Params.Material.Identity);

      Visibility.OnChanged += UpdateAttributes;
      Ordering.OnChanged += UpdateAttributes;
      Material.OnChanged += UpdateAttributes;

      UpdateAttributes();
      OnParentChanged += (node) =>
      {
        UpdateAttributes();
      };
    }

    /// <summary>
    /// Recalculates global rendering attributes and propagates them to children.
    /// </summary>
    private void UpdateAttributes()
    {
      if (GetParent() is CanvasNode parent)
      {
        Visibility.Global = Params.Visibility.Combine(parent.Visibility.Global, Visibility.Local);
        Ordering.Global = Params.Ordering.Combine(parent.Ordering.Global, Ordering.Local);
        Material.Global = Params.Material.Combine(parent.Material.Global, Material.Local);
      }
      else
      {
        Visibility.Global = Visibility.Local;
        Ordering.Global = Ordering.Local;
        Material.Global = Material.Local;
      }

      foreach (var child in Children)
      {
        if (child is CanvasNode canvasChild)
          canvasChild.UpdateAttributes();
      }
    }
  }

}
