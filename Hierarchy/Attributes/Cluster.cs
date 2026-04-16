using System;
using System.Collections.Generic;

namespace Monolith.Hierarchy
{
    ///<summary>
    /// WIP, prob not going to end up in final product.
    /// </summary>
    public readonly struct Cluster<T>
    {
        private readonly IReadOnlyList<T> _items;

        public bool Any => _items != null && _items.Count > 0;
        public int Count => _items?.Count ?? 0;

        public T First => _items[0];

        public Cluster(IReadOnlyList<T> items)
        {
            _items = items;
        }

        public TResult Aggregate<TResult>(
            Func<T, TResult> selector,
            Func<TResult, TResult, TResult> combine)
        {
            if (!Any)
                return default;

            TResult result = selector(_items[0]);

            for (int i = 1; i < _items.Count; i++)
            {
                result = combine(result, selector(_items[i]));
            }

            return result;
        }

        public bool AnyPair(
            Cluster<T> other,
            Func<T, T, bool> predicate
        )
        {
            for (int i = 0; i < Count; i++)
            {
                for (int j = 0; j < other.Count; j++)
                {
                    if (predicate(_items[i], other._items[j]))
                        return true;
                }
            }

            return false;
        }
    }
}
