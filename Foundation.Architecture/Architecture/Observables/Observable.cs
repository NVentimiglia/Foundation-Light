// Nicholas Ventimiglia 2016-09-05
using System;

namespace Foundation.Architecture
{
    /// <summary>
    /// A property with OnChange event
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Observable<T> : IEquatable<T>, IDisposable
    {
        public event Action OnChange = delegate { };

        public event Action<T> OnValueChange = delegate { };

        private IPropertyChanged _parent;
        private string _memberName;

        private T _value;
        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                Set(value);
            }
        }

        public static implicit operator T(Observable<T> observable)
        {
            return observable.Value;
        }

        public void SetValueSilently(T value)
        {
            _value = value;
        }

        public void Set(T value)
        {
            _value = value;
            OnValueChange(value);
            OnChange();

            if (_parent != null)
            {
                _parent.RaisePropertyChanged(_memberName);
            }
        }

        public T Get()
        {
            return _value;
        }

        public bool Equals(T other)
        {
            return _value.Equals(other);
        }

        public Observable()
        {

        }

        /// <summary>
        /// For Chaining
        /// </summary>
        public Observable(string memberName, IPropertyChanged parent)
        {
            Bind(memberName, parent);
        }

        /// <summary>
        /// For Chaining
        /// </summary>
        public void Bind(string memberName, IPropertyChanged parent)
        {
            _parent = parent;
            _memberName = memberName;
        }


        /// <summary>
        /// For Chaining
        /// </summary>
        void UnBind()
        {
            _parent = null;
            _memberName = null;
        }

        /// <summary>
        /// Clears it
        /// </summary>
        public void Dispose()
        {
            UnBind();
            _value = default(T);
        }
    }
}
