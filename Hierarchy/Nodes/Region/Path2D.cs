using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Monolith.Params;

namespace Monolith.Hierarchy
{
  public class Path2D : Node2D
  {
    [Export]
    public List<Vector2> Path { get; private set; }
    [Export]
    public Node2D Target { get; set; }

    [Export]
    public float Speed { get; set; } = 100f;

    private int currentTargetIndex = 0;
    private float segmentProcess = 0f;

    public Path2D()
    {
      Path = new List<Vector2>();
    }

    public void SetPath(params Vector2[] path)
    {
      Path = path.ToList();
      currentTargetIndex = 0;
      segmentProcess = 0f;
    }

    public void AddPath(params Vector2[] path)
    {
      Path.AddRange(path.ToList());
    }

    public override void ProcessUpdate(float delta)
    {
      base.ProcessUpdate(delta);

      if (Path == null || Path.Count < 2 || currentTargetIndex >= Path.Count - 1)
        return;

      Vector2 start = Path[currentTargetIndex];
      Vector2 end = Path[currentTargetIndex + 1];

      float distance = Vector2.Distance(start, end);

      if (distance > 0)
      {
        segmentProcess += Speed * delta / distance;
      }
      else
      {
        segmentProcess = MathHelper.Clamp(segmentProcess, 0f, 1f);
      }

      Target.LocalPosition = Vector2.Lerp(start, end, segmentProcess);

      if (segmentProcess >= 1f)
      {
        Target.LocalPosition = end;
        segmentProcess = 0f;
        currentTargetIndex++;
      }

    }
  }
}
