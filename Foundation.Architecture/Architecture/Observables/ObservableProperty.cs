// Nicholas Ventimiglia 2016-09-05
using System;

namespace Foundation.Architecture
{
    /// <summary>
    /// Wrapps a property with on change internally
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableProperty<T> : IEquatable<T>, IObservable<T>
    {
        private event Action<T> _onPublish = delegate { };
        public event Action<T> OnPublish
        {
            add
            {
                _onPublish += value;
                value(Value);
            }
            remove
            {
                _onPublish -= value;
            }
        }

        private IObservable<PropertyEvent> _parent;
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

        /// <summary>
        /// CTOR
        /// </summary>
        public ObservableProperty() { }

        /// <summary>
        /// For Chaining
        /// </summary>
        public ObservableProperty(string memberName, IObservable<PropertyEvent> parent)
        {
            Bind(memberName, parent);
        }

        /// <summary>
        /// Set value without notifing subscribers
        /// </summary>
        /// <param name="value"></param>
        public void SetValueSilently(T value)
        {
            _value = value;
        }

        /// <summary>
        /// Set value
        /// </summary>
        /// <param name="value"></param>
        public void Set(T value)
        {
            _value = value;
            _onPublish(value);

            if (_parent != null)
            {
                _parent.Publish(new PropertyEvent
                {
                    Sender = _parent,
                    Name = _memberName,
                    Value = _value,
                });
            }
        }

        /// <summary>
        /// Get value
        /// </summary>
        /// <returns></returns>
        public T Get()
        {
            return _value;
        }

        /// <summary>
        /// Notify Listeners
        /// </summary>
        /// <param name="model"></param>
        public void Publish(T model)
        {
            Value = model;
        }

        /// <summary>
        /// For Chaining MVVM
        /// </summary>
        public void Bind(string memberName, IObservable<PropertyEvent> parent)
        {
            _parent = parent;
            _memberName = memberName;
        }

        /// <summary>
        /// Clears it
        /// </summary>
        public void Dispose()
        {
            _parent = null;
            _memberName = null;
            _value = default(T);
            _onPublish = delegate { };
        }

        public bool Equals(T other)
        {
            return _value.Equals(other);
        }

        public static implicit operator T(ObservableProperty<T> observable)
        {
            return observable.Value;
        }

    }
}
