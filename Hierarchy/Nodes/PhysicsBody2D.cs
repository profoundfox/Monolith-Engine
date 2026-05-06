
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Monolith.Geometry;
using Monolith.Tools;

namespace Monolith.Hierarchy
{
  ///<summary>
  /// The class for all bodies which posses phsyics and are required to be queued from the server.
  ///</summary>
  public class PhysicsBody2D : CollisionNode2D, IHashAble
  {
    public PhysicsBody2D() { }

    public override void _EnterTree()
    {
      OnChildAdded += (node) =>
      {
        if (node is not CollisionShape2D)
          return;

        Core.Physics.RegisterBody(this);
        Core.Physics.NotifyMoved(this);
      };

      OnTransformChanged += (transform) =>
      {
        Core.Physics.NotifyMoved(this);
      };


      base._EnterTree();

      if (!CollisionShapes.IsEmpty())
      {
        Core.Physics.RegisterBody(this);
        Core.Physics.NotifyMoved(this);
      }
    }

    public override void _ExitTree()
    {
      Core.Physics.UnregisterBody(this);

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
