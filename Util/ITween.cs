using System;

namespace Monolith.Util
{
    public interface ITween
    {
        void Update();
        bool IsComplete();
    }
}
