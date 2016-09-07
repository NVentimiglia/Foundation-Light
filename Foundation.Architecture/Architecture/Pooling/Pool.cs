// Nicholas Ventimiglia 2016-09-05
using System.Collections.Generic;

namespace Foundation.Architecture
{
    /// <summary>
    /// A very simple object pool
    /// </summary>
    /// <remarks>
    /// Not thread safe
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class Pool<T> where T : new()
    {
        private readonly Stack<T> _items = new Stack<T>();

        /// <summary>
        /// Rents an item from the pool
        /// </summary>
        /// <returns></returns>
        public T Rent()
        {
            var result = _items.Count > 0 ? _items.Pop() : new T();
            return result;
        }

        /// <summary>
        /// Return the item to the pool
        /// </summary>
        /// <param name="item"></param>
        public void Return(T item)
        {
            _items.Push(item);
        }

        /// <summary>
        /// clears the pool
        /// </summary>
        public void Clear()
        {
            _items.Clear();
        }
    }
}
