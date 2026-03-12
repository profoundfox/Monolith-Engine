using System;

namespace Monolith.Attributes
{
    public interface ITween
    {
        void Update(float delta);
        bool IsComplete();
    }
}
