using System;

namespace Monolith.Attributes
{
    public interface ITween
    {
        void Update();
        bool IsComplete();
    }
}
