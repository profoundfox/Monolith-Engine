
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Microsoft.Xna.Framework;
using Monolith.Geometry;
using Monolith.Hierarchy;

namespace Monolith.Util
{
  public class Stage
  {
    public virtual void OnEnter() { }
    public virtual void PhysicsUpdate(float deltaTime) { }
    public virtual void ProcessUpdate(float deltaTime) { }
    public virtual void SubmitCall() { }
    public virtual void OnExit() { }
  }
}
