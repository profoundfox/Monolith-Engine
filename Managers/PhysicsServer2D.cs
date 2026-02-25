using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Monolith.Geometry;
using Monolith.Instances;

namespace Monolith.Managers
{
    public class PhysicsServer2D
    {
        private readonly SpatialHash<PhysicsBody2D> _broadphase;
        private readonly Dictionary<PhysicsBody2D, Rectangle> _bounds = new();
        
        public PhysicsServer2D()
        {
            _broadphase = new SpatialHash<PhysicsBody2D>();
        }

        public void RegisterBody(PhysicsBody2D body)
        {
            if (_bounds.ContainsKey(body))
                return;

            var bounds = body.Bounds;
            _bounds[body] = bounds;
            _broadphase.Insert(body);
        }

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
    
        public List<PhysicsBody2D> Query(Rectangle area)
        {
            return _broadphase.Query(area);
        }
    }
}