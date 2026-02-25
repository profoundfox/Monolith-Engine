using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Monolith.Nodes
{
    public class Path2D : Node2D
    {
        public List<Vector2> Path { get; private set; }
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
                segmentProcess = 1f;
            }

            LocalPosition = Vector2.Lerp(start, end, segmentProcess);

            if (segmentProcess >= 1f)
            {
                segmentProcess = 0f;
                currentTargetIndex++;
            }

        }
    }
}