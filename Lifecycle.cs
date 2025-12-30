using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Managers;

namespace Monolith
{
    public abstract class Lifecycle
    {
        protected Lifecycle()
        {
            Engine.Lifecycle.QueueAdd(this);
        }

        public virtual void Load() { }
        public virtual void Unload() { }
        public virtual void Update(GameTime gameTime) {}
        public virtual void Draw(SpriteBatch spriteBatch) { }

        public virtual void QueueFree()
        {
            Engine.Lifecycle.QueueRemove(this);
        }

        public virtual void FreeImmediate()
        {
            Engine.Lifecycle.RemoveImmediate(this);
        }
    }
}