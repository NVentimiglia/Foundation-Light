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

        //

        public ObservableProxy(object instance)
        {
            Instance = instance;

            if (Instance is IPropertyChanged)
            {
                ((IPropertyChanged)Instance).OnPropertyChanged += OnPropertyChanged;
            }

            BuildCache();
        }

        //

        public void Dispose()
        {
            if (Instance is IPropertyChanged)
            {
                ((IPropertyChanged)Instance).OnPropertyChanged -= OnPropertyChanged;
            }
            Instance = null;
            _cacheGet.Clear();
            _cacheSet.Clear();
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
                    UnityEngine.Debug.LogWarning("Unknown member " + memberName + " of " + typeof(T).Name + " on " + InstanceType.Name);
                    return default(T);
                }

                var funct = _cacheGet[memberName];

                return (funct as Func<T>).Invoke();
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError("Failed to call member " + memberName + " of " + typeof(T).Name + " with void");
                UnityEngine.Debug.LogException(ex);
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
                    UnityEngine.Debug.LogWarning("Unknown member " + memberName + " of " + typeof(T).Name + " on " + InstanceType.Name);
                    return;
                }

                var funct = _cacheSet[memberName];
                
                (funct as Action<T>).Invoke(value);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError("Failed to call member " + memberName + " of " + typeof(T).Name + " with " + value.GetType().Name);
                UnityEngine.Debug.LogException(ex);
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
                    UnityEngine.Debug.LogWarning("Unknown member " + memberName + " of void " + " on " + InstanceType.Name);
                    return;
                }

                var funct = _cacheSet[memberName];

                (funct as Action).Invoke();
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError("Failed to call member " + memberName + " of void " + " on " + InstanceType.Name);
                UnityEngine.Debug.LogException(ex);
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
            var methods = InstanceType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

            foreach (var member in methods)
            {
                if (_cacheSet.ContainsKey(member.Name))
                {
                    UnityEngine.Debug.LogWarning("Duplicate member " + member.Name +  " on " + InstanceType.Name);
                    continue;
                }
                
                var ptype = member.GetParameters();

                // Note : Would be nice to invoke coroutines here, NV

                if (ptype.Length == 0)
                {
                    var del = Delegate.CreateDelegate(typeof(Action), Instance, member.Name) as Action;
                    _cacheSet.Add(member.Name, del);
                }
                else
                {
                    var del = Delegate.CreateDelegate(typeof(Action<object>), Instance, member.Name) as Action;
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
                    UnityEngine.Debug.LogWarning("Duplicate member " + member.Name + " on " + InstanceType.Name);
                    continue;
                }
                
                var get = Delegate.CreateDelegate(typeof(Func<object>), Instance, member.GetGetMethod());
                _cacheGet.Add(member.Name, get);

                var set = Delegate.CreateDelegate(typeof(Action<object>), Instance, member.GetSetMethod());
                _cacheSet.Add(member.Name, set);
            }
        }

        void CacheObservables()
        {
            var members = InstanceType.GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(o => o.FieldType == typeof(Observable<>))
                .ToArray();

            foreach (var member in members)
            {
                if (_cacheSet.ContainsKey(member.Name) || _cacheSet.ContainsKey(member.Name))
                {
                    UnityEngine.Debug.LogWarning("Duplicate member " + member.Name + " on " + InstanceType.Name);
                    continue;
                }

                var obs = member.GetValue(Instance);

                var get = Delegate.CreateDelegate(typeof(Func<object>), obs, obs.GetType().GetMethod("Get"));
                _cacheGet.Add(member.Name, get);

                var set = Delegate.CreateDelegate(typeof(Action<object>), obs, obs.GetType().GetMethod("Set"));
                _cacheSet.Add(member.Name, set);
            }
        }

        ///// <summary>
        ///// Get Member Value
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="memberName"></param>
        ///// <returns></returns>
        //public T Get<T>(string memberName)
        //{
        //    //TODO sanity
        //    //TODO Cache Reflection
        //    //TODO Conversion
        //    var temp = this.GetType().GetMember(memberName)[0];
        //    if (temp is FieldInfo)
        //    {
        //        return (T)(temp as FieldInfo).GetValue(Instance);
        //    }
        //    else if (temp is PropertyInfo)
        //    {
        //        return (T)(temp as PropertyInfo).GetValue(Instance, null);
        //    }
        //    else
        //    {
        //        return (T)(temp as MethodInfo).Invoke(Instance, null);
        //    }
        //}

        ///// <summary>
        ///// Set Member Value
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="memberName"></param>
        ///// <param name="value"></param>
        //public void Set<T>(string memberName, T value)
        //{
        //    //TODO sanity
        //    //TODO Cache Reflection
        //    //TODO Conversion
        //    var temp = this.GetType().GetMember(memberName)[0];
        //    if (temp is FieldInfo)
        //    {
        //        (temp as FieldInfo).SetValue(Instance, value);
        //    }
        //    else if (temp is PropertyInfo)
        //    {
        //        (temp as PropertyInfo).SetValue(Instance, value, null);
        //    }
        //    else
        //    {
        //        (temp as MethodInfo).Invoke(Instance, new object[] { value });
        //    }
        //}

        ///// <summary>
        ///// Invoke Method Value
        ///// </summary>
        ///// <param name="memberName"></param>
        //public void Invoke(string memberName)
        //{
        //    //TODO sanity
        //    //TODO Cache Reflection
        //    //TODO Conversion
        //    var temp = this.GetType().GetMember(memberName)[0];
        //    if (temp is MethodInfo)
        //    {
        //        (temp as MethodInfo).Invoke(Instance, null);
        //    }
        //}
    }
}