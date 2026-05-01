using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Monolith.Geometry;
using Monolith.Hierarchy;

namespace Monolith.Managers
{
  public class PhysicsServer2D : BaseObject
  {
    private readonly SpatialHash<PhysicsBody2D> _broadphase;
    private readonly Dictionary<PhysicsBody2D, List<Rectangle>> _bounds = new();

    public PhysicsServer2D()
    {
      _broadphase = new SpatialHash<PhysicsBody2D>();
    }

    /// <summary>
    /// Registers a physics body to the server.
    /// </summary>
    /// <param name="body">The body in question.</param>
    public void RegisterBody(PhysicsBody2D body)
    {
      if (_bounds.ContainsKey(body))
        return;

      var bounds = body.Bounds;
      _bounds[body] = bounds;
      _broadphase.Insert(body);
    }

    ///<summary>
    /// Unregisters a body from the server, this effectively disables other bodies searching for it.
    ///</summary>
    ///<param name="body">The body in question.</param>
    public void UnregisterBody(PhysicsBody2D body)
    {
      if (_bounds.TryGetValue(body, out var oldBounds))
      {
        _broadphase.Remove(body);
        _bounds.Remove(body);
      }
    }

    public void NotifyMoved(PhysicsBody2D body)
    {
      if (!_bounds.TryGetValue(body, out var oldBounds))
        return;

      var newBounds = body.Bounds;

      if (newBounds != oldBounds)
      {
        _broadphase.Update(body, oldBounds);
        _bounds[body] = newBounds;
      }
    }

    public List<PhysicsBody2D> Query(List<Rectangle> area)
    {
      return _broadphase.Query(area);
    }
  }
}
