using System;
using System.Collections.Generic;

namespace Foundation.Architecture.Misc
{
    /// <summary>
    /// Pool Manager
    /// </summary>
    /// <remarks>
    /// Thread Safe
    /// </remarks>
    public static class PoolService
    {
        static readonly Dictionary<Type, object> _pools = new Dictionary<Type, object>();
        static readonly object _lock = new object();

        /// <summary>
        /// Returns an object to the correct pool
        /// </summary>
        public static T Rent<T>() where T : new()
        {
            var t = typeof(T);

            lock (_lock)
            {
                if (_pools.ContainsKey(t))
                {
                    return (_pools[t] as Pool<T>).Rent();
                }

                var pool = Activator.CreateInstance<Pool<T>>();
                _pools.Add(t, pool);
                return pool.Rent();
            }
        }

        /// <summary>
        /// Returns an object to the correct pool
        /// </summary>
        /// <param name="item"></param>
        public static void Return<T>(T item) where T : new()
        {
            var t = typeof(T);

            lock (_lock)
            {
                if (_pools.ContainsKey(t))
                {
                    (_pools[t] as Pool<T>).Return(item);
                }

                var pool = Activator.CreateInstance<Pool<T>>();
                _pools.Add(t, pool);
                pool.Return(item);
            }

        }
    }
}