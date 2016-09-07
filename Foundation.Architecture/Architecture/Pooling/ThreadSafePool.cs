// Nicholas Ventimiglia 2016-09-07

using System.Collections.Generic;

namespace Foundation.Architecture
{
    /// <summary>
    /// A very simple object pool
    /// </summary>
    /// <remarks>
    /// thread safe
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class ThreadSafePool<T> where T : new()
    {
        /// <summary>
        /// Global Singleton
        /// </summary>
        public static readonly ThreadSafePool<T> Default = new ThreadSafePool<T>();

        private readonly Stack<T> _items = new Stack<T>();
        private readonly object _lock = new object();

        /// <summary>
        /// Rents an item from the pool
        /// </summary>
        /// <returns></returns>
        public T Rent()
        {
            lock (_lock)
            {
                var result = _items.Count > 0 ? _items.Pop() : new T();
                return result;
            }
        }

        /// <summary>
        /// Return the item to the pool
        /// </summary>
        /// <param name="item"></param>
        public void Return(T item)
        {
            lock (_lock)
            {
                _items.Push(item);
            }
        }

        /// <summary>
        /// clears the pool
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                _items.Clear();
            }
        }
    }
}