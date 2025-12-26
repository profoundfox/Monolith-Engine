using System;

namespace Monolith.Input
{
    public interface ISource<T> where T : Enum
    {
        /// <summary>
        /// Polls the device and updates its internal snapshot.
        /// Must be called once per frame.
        /// </summary>
        void Update();

        /// <summary>
        /// Returns true if the input is currently down.
        /// </summary>
        bool IsDown(T input);

        /// <summary>
        /// Returns true if the input is currently up.
        /// </summary>
        bool IsUp(T input);
    }
}