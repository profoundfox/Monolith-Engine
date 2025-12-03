using System.Collections.Generic;
using System.Dynamic;
using Monolith.UI;
using Microsoft.Xna.Framework;

namespace Monolith.UI.GUM
{
    public static class GumManager
    {
        private readonly static List<IGumUpdatable> Updatables = new();

        public static void Register(IGumUpdatable updatable)
        {
            Updatables.Add(updatable);
        }

        public static void Unregister(IGumUpdatable updatable)
        {
            Updatables.Remove(updatable);
        }

        public static void UnregisterAll(IGumUpdatable updatable)
        {
            foreach(IGumUpdatable gumUpdatable in Updatables)
                Updatables.Remove(gumUpdatable);
        }

        public static void UpdateAll(GameTime gameTime)
        {
            foreach (var u in Updatables)
                u.Update(gameTime);
        }
    }
}