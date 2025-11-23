using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Microsoft.Xna.Framework;

namespace Monolith.UI
{
    public interface IGumUpdatable
    {
        void Update(GameTime gameTime);
    }
}
