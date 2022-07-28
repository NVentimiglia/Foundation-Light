// Nicholas Ventimiglia 2016-09-05

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Foundation.Architecture
{
    /// <summary>
    /// Reflection Proxy. Wraps around an object and caches all reflection calls for a ~300% improvement.
    /// </summary>
    public class ObservableProxy : IObservable<PropertyEvent>
    {
        #region Api

        public object Instance { get; private set; }
        public Type InstanceType { get; private set; }

        Dictionary<string, List<Action<PropertyEvent>>> Handlers = new Dictionary<string, List<Action<PropertyEvent>>>();
      
        /// <summary>
        /// Call a member
        /// </summary>
        public T Get<T>(string memberName)
        {
            try
            {
                DelegateInfo temp;
                if (_cacheGet.TryGetValue(memberName, out temp))
                {
                    var val = (temp.Delegate).DynamicInvoke();
                    return (T)Convert(val, typeof(T));
                }
                LogService.LogWarning("Unknown member " + memberName + " of " + typeof(T).Name + " on " + InstanceType.Name);
                return default(T);
            }
            catch (Exception ex)
            {
                LogService.LogError("Failed to call member " + memberName + " of " + typeof(T).Name + " with void");
                LogService.LogException(ex);

                return default(T);
            }
        }

        /// <summary>
        /// Call a member
        /// </summary>
        public void Post(string memberName, object value = null)
        {
            try
            {
                DelegateInfo temp;
                if (_cacheSet.TryGetValue(memberName, out temp))
                {
                    if (value == null)
                    {
                        (temp.Delegate as Action).Invoke();
                    }
                    else
                    {
                        (temp.Delegate).DynamicInvoke(Convert(value, temp.Type));
                    }
                }
                else
                {
                    LogService.LogWarning("Unknown member " + memberName + " on " + InstanceType.Name);
                }
            }
            catch (Exception ex)
            {
                LogService.LogError("Failed to call member " + memberName + " on " + InstanceType.Name);
                LogService.LogException(ex);
            }
        }

        /// <summary>
        /// Member specific listener
        /// </summary>
        public void Subscribe(string memberName, Action<PropertyEvent> handler)
        {
            List<Action<PropertyEvent>> events;

            if (!Handlers.TryGetValue(memberName, out events))
            {
                events = new List<Action<PropertyEvent>>();
                Handlers.Add(memberName, events);
            }

            events.Add(handler);
        }

        /// <summary>
        /// Member specific listener
        /// </summary>
        public void Unsubscribe(string memberName, Action<PropertyEvent> handler)
        {
            List<Action<PropertyEvent>> events;

            if (Handlers.TryGetValue(memberName, out events))
            {
                events = new List<Action<PropertyEvent>>();
                events.Remove(handler);
            }
        }

        public ObservableProxy(object instance)
        {
            Instance = instance;

            if (Instance is IObservable<PropertyEvent>)
            {
                ((IObservable<PropertyEvent>)Instance).OnPublish += Publish;
            }

            BuildCache();
        }

        public void Dispose()
        {
            if (Instance is IObservable<PropertyEvent>)
            {
                ((IObservable<PropertyEvent>)Instance).OnPublish -= Publish;
            }

            Instance = null;

            Handlers.Clear();
            _cacheGet.Clear();
            _cacheSet.Clear();
        }

        object Convert(object value, Type type)
        {
            if (type == null || value == null || value.GetType() == type)
                return value;

            return System.Convert.ChangeType(value, type);
        }
        #endregion

        #region IObservable

        public event Action<PropertyEvent> OnPublish = delegate { };

        public void Publish(PropertyEvent model)
        {
            OnPublish(model);
        }
        
        #endregion

        #region Caching

        struct DelegateInfo
        {
            public Delegate Delegate;
            public Type Type;

            public DelegateInfo(Delegate d, Type t)
            {
                Delegate = d;
                Type = t;
            }
        }

        private Dictionary<string, DelegateInfo> _cacheGet = new Dictionary<string, DelegateInfo>();
        private Dictionary<string, DelegateInfo> _cacheSet = new Dictionary<string, DelegateInfo>();

        void BuildCache()
        {
            InstanceType = Instance.GetType();

            //Methods
            CacheMethods();
            CacheProperties();
        }

        void CacheMethods()
        {
            var methods =
                InstanceType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => !m.IsSpecialName);

            foreach (var member in methods)
            {
                if (_cacheSet.ContainsKey(member.Name))
                {
                    LogService.LogWarning("Duplicate member " + member.Name + " on " + InstanceType.Name);

                    continue;
                }


                var ptype = member.GetParameters();

                // Note : Would be nice to invoke coroutines here, NV

                if (ptype.Length == 0)
                {
                    var del = CreateDelegate(typeof(Action), Instance, member);
                    _cacheSet.Add(member.Name, new DelegateInfo(del, null));
                }
                else
                {
                    var type = typeof(Action<>).MakeGenericType(ptype[0].ParameterType);
                    var del = CreateDelegate(type, Instance, member);
                    _cacheSet.Add(member.Name, new DelegateInfo(del, ptype[0].ParameterType));
                }
            }
        }

        void CacheProperties()
        {
            var members = InstanceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var member in members)
            {
                if (_cacheSet.ContainsKey(member.Name) || _cacheSet.ContainsKey(member.Name))
                {
                    LogService.LogWarning("Duplicate member " + member.Name + " on " + InstanceType.Name);
                    continue;
                }

                var gtype = typeof(Func<>).MakeGenericType(member.PropertyType);
                var get = CreateDelegate(gtype, Instance, member.GetGetMethod());
                _cacheGet.Add(member.Name, new DelegateInfo(get, member.PropertyType));

                var stype = typeof(Action<>).MakeGenericType(member.PropertyType);
                var set = CreateDelegate(stype, Instance, member.GetSetMethod());
                _cacheSet.Add(member.Name, new DelegateInfo(set, member.PropertyType));
            }
        }

        Delegate CreateDelegate(Type type, Object target, MethodInfo method)
        {
#if CORE
            return method.CreateDelegate(type, target);
#else
            return Delegate.CreateDelegate(type, target, method);
#endif
        }
        #endregion
    }
}