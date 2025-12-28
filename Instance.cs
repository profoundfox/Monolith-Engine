using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monolith
{
    public interface IInstance
    {
        string Name { get; }
    }

    public interface ILoadable
    {
        void Load();
        void Unload();
    }

    public interface IUpdatable
    {
        void Update(GameTime gameTime);
    }

    public interface IDrawable
    {
        void Draw(SpriteBatch spriteBatch);
    }
}