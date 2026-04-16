using System;

namespace Monolith.Params
{
    public class AudioOutput
    {
        ///<summary>
        /// The max value which is allowed.
        ///</summary>
        public int MaxValue { get; set; } = 100;

        private int _value;

        ///<summary>
        /// The value of this audio output; clamps the value to the max value.
        ///</summary>
        public int Value
        {
            get => _value;
            set
            {
                _value = Math.Clamp(value, 0, MaxValue);
            }
        }

        ///<summary>
        /// Turns the current value into a percentage of the max value.
        ///</summary>
        public float Percent()
        {
            return _value / MaxValue * 100;
        }

    }
}
