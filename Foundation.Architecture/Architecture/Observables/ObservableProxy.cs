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
        public object Instance { get; private set; }
        public Type InstanceType { get; private set; }

        public event PropertyChanged OnPropertyChanged = delegate { };


        //

        private Dictionary<string, Delegate> _cacheGet = new Dictionary<string, Delegate>();
        private Dictionary<string, Delegate> _cacheSet = new Dictionary<string, Delegate>();
        private Dictionary<object, Action> _cacheObs = new Dictionary<object, Action>();

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
                einfo.RemoveEventHandler(cacheOb.Key, cacheOb.Value);
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
                if (!_cacheGet.ContainsKey(memberName))
                {
                    Logger.LogWarning("Unknown member " + memberName + " of " + typeof(T).Name + " on " +
                                      InstanceType.Name);

                    return default(T);
                }

                var funct = _cacheGet[memberName];

                return (funct as Func<T>).Invoke();
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to call member " + memberName + " of " + typeof(T).Name + " with void");
                Logger.LogException(ex);

                return default(T);
            }
        }

        /// <summary>
        /// Call a member
        /// </summary>
        public void Set<T>(string memberName, T value)
        {
            try
            {
                if (!_cacheSet.ContainsKey(memberName))
                {
                    Logger.LogWarning("Unknown member " + memberName + " of " + typeof(T).Name + " on " +
                                      InstanceType.Name);

                    return;
                }

                var funct = _cacheSet[memberName];

                (funct as Action<T>).Invoke(value);
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to call member " + memberName + " of " + typeof(T).Name + " with " +
                                value.GetType().Name);
                Logger.LogException(ex);
            }
        }

        /// <summary>
        /// Call a member
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Invoke(string memberName)
        {
            try
            {
                if (!_cacheSet.ContainsKey(memberName))
                {
                    Logger.LogWarning("Unknown member " + memberName + " of void " + " on " + InstanceType.Name);

                    return;
                }

                var funct = _cacheSet[memberName];

                (funct as Action).Invoke();
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to call member " + memberName + " of void " + " on " + InstanceType.Name);
                Logger.LogException(ex);
            }
        }

        // 

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
                    Logger.LogWarning("Duplicate member " + member.Name + " on " + InstanceType.Name);

                    continue;
                }


                var ptype = member.GetParameters();

                // Note : Would be nice to invoke coroutines here, NV

                if (ptype.Length == 0)
                {
                    var del = Delegate.CreateDelegate(typeof(Action), Instance, member.Name);
                    _cacheSet.Add(member.Name, del);
                }
                else
                {
                    var type = typeof(Action<>).MakeGenericType(ptype[0].ParameterType);
                    var del = Delegate.CreateDelegate(type, Instance, member.Name);
                    _cacheSet.Add(member.Name, del);
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
                    Logger.LogWarning("Duplicate member " + member.Name + " on " + InstanceType.Name);

                    continue;
                }

                var gtype = typeof(Func<>).MakeGenericType(member.PropertyType);
                var get = Delegate.CreateDelegate(gtype, Instance, member.GetGetMethod());
                _cacheGet.Add(member.Name, get);

                var stype = typeof(Action<>).MakeGenericType(member.PropertyType);
                var set = Delegate.CreateDelegate(stype, Instance, member.GetSetMethod());
                _cacheSet.Add(member.Name, set);
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
                    Logger.LogWarning("Duplicate member " + member.Name + " on " + InstanceType.Name);

                    continue;
                }

                var obs = member.GetValue(Instance);
                var otype = member.FieldType.GetGenericArguments()[0];

                var gtype = typeof(Func<>).MakeGenericType(otype);
                var get = Delegate.CreateDelegate(gtype, obs, obs.GetType().GetMethod("Get"));
                _cacheGet.Add(member.Name, get);

                var stype = typeof(Action<>).MakeGenericType(otype);
                var set = Delegate.CreateDelegate(stype, obs, obs.GetType().GetMethod("Set"));
                _cacheSet.Add(member.Name, set);

                var einfo = obs.GetType().GetEvent("OnChange");

                Action handler = () => { RaisePropertyChanged(member.Name); };

                einfo.AddEventHandler(obs, handler);
                _cacheObs.Add(obs, handler);
            }
        }


        public void RaisePropertyChanged(string memberName)
        {
            OnPropertyChanged(memberName);
        }
    }
}