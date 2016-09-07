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

        private Observable<T> _parent;

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

        public static implicit operator Observable<T>(T value)
        {
            return new Observable<T> { Value = value };
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

        public Observable(Observable<T> parent)
        {
            Bind(parent);
        }

        /// <summary>
        /// For Chaining
        /// </summary>
        public void Bind(Observable<T> parent)
        {
            UnBind();
            if (_parent != null)
            {
                _parent = parent;
                _parent.OnValueChange += Set;
            }
        }


        /// <summary>
        /// For Chaining
        /// </summary>
        void UnBind()
        {
            if (_parent != null)
            {
                _parent.OnValueChange -= Set;
                _parent = null;
            }
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
