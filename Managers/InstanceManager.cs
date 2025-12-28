using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monolith
{
    public class InsanceManager<T> where T : IInstance
    {
        protected readonly List<T> allInstances = new();
        protected readonly Dictionary<string, List<T>> byName = new();

        protected readonly List<T> pendingAdds = new();
        protected readonly List<T> pendingRemovals = new();

        public IReadOnlyList<T> All => allInstances;

        public void QueueAdd(T instance)
        {
            pendingAdds.Add(instance);
        }

        public void QueueRemove(T instance)
        {
            pendingRemovals.Add(instance);
        }

        protected virtual void ApplyPending()
        {
            if (pendingAdds.Count > 0)
            {
                foreach (var inst in pendingAdds)
                {
                    allInstances.Add(inst);

                    if (!string.IsNullOrEmpty(inst.Name))
                    {
                        if (!byName.TryGetValue(inst.Name, out var list))
                            byName[inst.Name] = list = new List<T>();
                        
                        list.Add(inst);
                    }

                    pendingAdds.Clear();
                }
            }

            if (pendingRemovals.Count > 0)
            {
                foreach(var inst in pendingRemovals)
                {
                    allInstances.Remove(inst);

                    if (!string.IsNullOrEmpty(inst.Name)
                        && byName.TryGetValue(inst.Name, out var list))
                    {
                        list.Remove(inst);
                        if (list.Count == 0)
                            byName.Remove(inst.Name);
                    }

                }

                pendingRemovals.Clear();
            }
        }

        public IReadOnlyList<T> GetByName(string name)
         => byName.TryGetValue(name, out var list) ? list : Array.Empty<T>();

        public void LoadAll()
        {
            ApplyPending();

            foreach (var inst in allInstances.OfType<ILoadable>())
            {
                inst.Load();
            }

            ApplyPending();
        }

        public void UnloadAll()
        {
            ApplyPending();

            foreach (var inst in allInstances.OfType<ILoadable>())
            {
                inst.Unload();
            }

            ApplyPending();
        }

        public void UpdateAll(GameTime gameTime)
        {
            ApplyPending();

            foreach (var inst in allInstances.OfType<IUpdatable>())
            {
                inst.Update(gameTime);
            }

            ApplyPending();
        }

        public void DrawAll(SpriteBatch spriteBatch)
        {
            foreach (var inst in allInstances.OfType<IDrawable>())
                inst.Draw(spriteBatch);
        }

    }
}