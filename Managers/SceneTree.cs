using System;
using System.Collections.Generic;
using Monolith.Hierarchy;

namespace Monolith.Managers
{
  public class SceneTree
  {

    private readonly Node _root;

    private Node _scene;
    private readonly Stack<Node> _overlays = new();

    public SceneTree(Node root)
    {
      _root = root ?? throw new ArgumentNullException(nameof(root)); 
    }

    public Node CurrentScene => _scene;

    public Node CurrentOverlay => _overlays.Count > 0 ? _overlays.Peek() : null;

    public void SetScene(Node scene)
    {
      if (scene == null)
        throw new ArgumentNullException(nameof(scene));

      ClearOverlays();

      if (_scene != null)
      {
        _root.RemoveChild(_scene);
        _scene.QueueFree();
      }

      _scene = scene;

      _root.AddChild(scene);
    }

    public void ReloadCurrentScene()
    {
        if (_scene == null)
            return;

        ClearOverlays();

        var oldScene = _scene;

        oldScene.OnExit();

        Engine.Table.Clear();

        oldScene.OnEnter();
    }

    public void PushOverlay(Node overlay)
    {
      if (overlay == null)
        throw new ArgumentNullException(nameof(overlay));

      _overlays.Push(overlay);

      _root.AddChild(overlay);
    }

    public void PopOverlay()
    {
      if (_overlays.Count == 0)
        return;

      var overlay = _overlays.Pop();

      _root.RemoveChild(overlay);
      overlay.QueueFree();
    }

    public void ClearOverlays()
    {
      while (_overlays.Count > 0);
        PopOverlay();
    }

  }
}
