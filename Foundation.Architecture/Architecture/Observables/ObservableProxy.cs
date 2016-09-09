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
    public class ObservableProxy : IPropertyChanged, IDisposable
    {
        public struct DelegateInfo
        {
            public Delegate Delegate;
            public Type Type;

            public DelegateInfo(Delegate d, Type t)
            {
                Delegate = d;
                Type = t;
            }
        }


        public object Instance { get; private set; }
        public Type InstanceType { get; private set; }

        public event PropertyChanged OnPropertyChanged = delegate { };


        //

        private Dictionary<string, DelegateInfo> _cacheGet = new Dictionary<string, DelegateInfo>();
        private Dictionary<string, DelegateInfo> _cacheSet = new Dictionary<string, DelegateInfo>();
        private Dictionary<object, DelegateInfo> _cacheObs = new Dictionary<object, DelegateInfo>();

        //

        public ObservableProxy(object instance)
        {
            Instance = instance;

            if (Instance is IPropertyChanged)
            {
                ((IPropertyChanged)Instance).OnPropertyChanged += RaisePropertyChanged;
            }

            BuildCache();
        }

        //

        public void Dispose()
        {
            if (Instance is IPropertyChanged)
            {
                ((IPropertyChanged)Instance).OnPropertyChanged -= RaisePropertyChanged;
            }
            Instance = null;


            foreach (var cacheOb in _cacheObs)
            {
                var einfo = cacheOb.Key.GetType().GetEvent("OnChange");
                einfo.RemoveEventHandler(cacheOb.Key, cacheOb.Value.Delegate);
            }

            _cacheGet.Clear();
            _cacheSet.Clear();
            _cacheObs.Clear();
        }

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


        // 


        object Convert(object value, Type type)
        {
            if (type == null || value == null  || value.GetType() == type)
                return value;

            return System.Convert.ChangeType(value, type);
        }

        void BuildCache()
        {
            InstanceType = Instance.GetType();

            //Methods
            CacheMethods();
            CacheProperties();
            CacheObservables();
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

        void CacheObservables()
        {
            var members = InstanceType.GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(o => o.FieldType.GetGenericTypeDefinition() == typeof(Observable<>))
                .ToArray();

            foreach (var member in members)
            {
                if (_cacheSet.ContainsKey(member.Name) || _cacheSet.ContainsKey(member.Name))
                {
                    LogService.LogWarning("Duplicate member " + member.Name + " on " + InstanceType.Name);

                    continue;
                }

                var obs = member.GetValue(Instance);
                var otype = member.FieldType.GetGenericArguments()[0];

                var gtype = typeof(Func<>).MakeGenericType(otype);
                var get = CreateDelegate(gtype, obs, obs.GetType().GetMethod("Get"));
                _cacheGet.Add(member.Name, new DelegateInfo(get, otype));

                var stype = typeof(Action<>).MakeGenericType(otype);
                var set = CreateDelegate(stype, obs, obs.GetType().GetMethod("Set"));
                _cacheSet.Add(member.Name, new DelegateInfo(set, otype));

                var einfo = obs.GetType().GetEvent("OnChange");

                Action handler = () =>
                {
                    RaisePropertyChanged(member.Name);
                };

                einfo.AddEventHandler(obs, handler);
                _cacheObs.Add(obs, new DelegateInfo(handler, otype));
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

        public void RaisePropertyChanged(string memberName)
        {
            OnPropertyChanged(memberName);
        }
    }
}