// Nicholas Ventimiglia 2016-09-05
using System;
using System.Collections.Generic;

namespace Foundation.Architecture.Misc
{
    public class PoolService : IPoolService
    {
        private readonly Dictionary<Type, object> _pools = new Dictionary<Type, object>();
        private readonly object _lock = new object();

        public T Rent<T>() where T : new()
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

        public void Return<T>(T item) where T : new()
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