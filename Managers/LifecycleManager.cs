using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monolith.Managers
{
    public class LifecycleManager
    {
        private readonly List<Lifecycle> allInstances = new();
        private readonly List<Lifecycle> pendingAdds = new();
        private readonly List<Lifecycle> pendingRemovals = new();

        private readonly List<Lifecycle> p = new();

        public LifecycleManager()
        {
        }

        public void QueueAdd(Lifecycle obj) 
        {
            p.Add(obj);
        }

        public void QueueRemove(Lifecycle obj)
        {
            pendingRemovals.Add(obj);
        }

        public void RemoveImmediate(Lifecycle obj)
        {
            allInstances.Remove(obj);
            obj.Unload();
        }

        private void ApplyPending()
        {
            if (p.Count > 0)
            {
                foreach (var obj in p.ToArray())
                {
                    if (pendingRemovals.Contains(obj))
                        throw new Exception($"Lifecycle added and removed in same frame: {obj}");
                    allInstances.Add(obj);
                    obj.Load();
                }
                p.Clear();
            }

            if (pendingRemovals.Count > 0)
            {
                foreach (var obj in pendingRemovals.ToArray())
                {
                    allInstances.Remove(obj);
                    obj.Unload();
                }
                pendingRemovals.Clear();
            }
        }

        public void Update(GameTime gameTime)
        {
            ApplyPending();
            foreach (var obj in allInstances.ToArray())
            {
                obj.Update(gameTime);
            }
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var obj in allInstances.ToArray())
                obj.Draw(spriteBatch);
            
            
        }

        public IReadOnlyList<Lifecycle> All => allInstances.AsReadOnly();

    }
}
