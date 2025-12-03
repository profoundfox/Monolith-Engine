using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Microsoft.Xna.Framework;

namespace Monolith.UI.GUM
{
    public interface IGumUpdatable
    {
        void Update(GameTime gameTime);
    }
}
