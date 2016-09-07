// Nicholas Ventimiglia 2016-09-05
using System;
using System.Collections.Generic;

namespace Foundation.Architecture
{
    public class PoolService : IPoolService
    {
        private readonly Dictionary<Type, object> _pools = new Dictionary<Type, object>();
        private readonly object _lock = new object();

        public Pool<TObject> GetPool<TObject>() where TObject : new()
        {
            var t = typeof(TObject);

            lock (_lock)
            {
                if (_pools.ContainsKey(t))
                {
                    return _pools[t] as Pool<TObject>;
                }

                var pool = Activator.CreateInstance<Pool<TObject>>();
                _pools.Add(t, pool);
                return pool;
            }
        }

        public TObject Rent<TObject>() where TObject : new()
        {
            var t = typeof(TObject);

            lock (_lock)
            {
                if (_pools.ContainsKey(t))
                {
                    return (_pools[t] as Pool<TObject>).Rent();
                }

                var pool = Activator.CreateInstance<Pool<TObject>>();
                _pools.Add(t, pool);
                return pool.Rent();
            }
        }

        public void Return<TObject>(TObject item) where TObject : new()
        {
            var t = typeof(TObject);

            lock (_lock)
            {
                if (_pools.ContainsKey(t))
                {
                    (_pools[t] as Pool<TObject>).Return(item);
                }

                var pool = Activator.CreateInstance<Pool<TObject>>();
                _pools.Add(t, pool);
                pool.Return(item);
            }
        }
    }
}