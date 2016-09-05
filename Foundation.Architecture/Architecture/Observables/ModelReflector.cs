// Nicholas Ventimiglia 2016-09-05
using System;
using System.Reflection;

namespace Foundation.Architecture
{
    /// <summary>
    /// Reflection Proxy
    /// </summary>
    /// <remarks>
    /// This class will house 100% of all reflection. 
    /// It will implement reflection caching so there should be no runtime penalty.
    /// </remarks>
    public class ModelReflector : IPropertyChanged, IDisposable
    {
        public object Instance;
        public event PropertyChanged OnPropertyChanged = delegate { };
        
        public ModelReflector(object instance)
        {
            Instance = instance;

            if(Instance is IPropertyChanged)
            {
                ((IPropertyChanged)Instance).OnPropertyChanged += OnPropertyChanged;
            }
        }

        public void Dispose()
        {
            if (Instance is IPropertyChanged)
            {
                ((IPropertyChanged)Instance).OnPropertyChanged -= OnPropertyChanged;
            }
            Instance = null;
        }
        
        /// <summary>
        /// Get Member Value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public T Get<T>(string memberName)
        {
            //TODO sanity
            //TODO Cache Reflection
            //TODO Conversion
            var temp = this.GetType().GetMember(memberName)[0];
            if (temp is FieldInfo)
            {
                return (T)(temp as FieldInfo).GetValue(Instance);
            }
            else if (temp is PropertyInfo)
            {
                return (T)(temp as PropertyInfo).GetValue(Instance, null);
            }
            else
            {
                return (T)(temp as MethodInfo).Invoke(Instance, null);
            }
        }

        /// <summary>
        /// Set Member Value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memberName"></param>
        /// <param name="value"></param>
        public void Set<T>(string memberName, T value)
        {
            //TODO sanity
            //TODO Cache Reflection
            //TODO Conversion
            var temp = this.GetType().GetMember(memberName)[0];
            if (temp is FieldInfo)
            {
                (temp as FieldInfo).SetValue(Instance, value);
            }
            else if (temp is PropertyInfo)
            {
                (temp as PropertyInfo).SetValue(Instance, value, null);
            }
            else
            {
                (temp as MethodInfo).Invoke(Instance, new object[] { value });
            }
        } 
        
        /// <summary>
           /// Invoke Method Value
           /// </summary>
           /// <param name="memberName"></param>
        public void Invoke(string memberName)
        {
            //TODO sanity
            //TODO Cache Reflection
            //TODO Conversion
            var temp = this.GetType().GetMember(memberName)[0];
            if(temp is MethodInfo)
            {
                (temp as MethodInfo).Invoke(Instance, null);
            }
        }
    }
}