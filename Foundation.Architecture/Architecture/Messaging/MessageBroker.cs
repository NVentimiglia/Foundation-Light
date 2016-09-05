using System;
using System.Collections.Generic;

namespace Foundation.Architecture
{
    /// <summary>
    /// Generic Message broadcaster
    /// </summary>
    /// <remarks>
    /// I would like to avoid using this class.
    /// I prefer simple events or or a controller based routing mechanism.
    /// Added due to 'DomainEvents' 
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public static class MessageBroker<T>
    {
        public delegate void MessageDelegate(T message);

        public delegate void RoutedMessageDelegate(uint channel, T message);

        /// <summary>
        /// Event
        /// </summary>
        public static event MessageDelegate OnMessage = delegate { };

        /// <summary>
        /// Event
        /// </summary>
        public static event RoutedMessageDelegate OnRoutedMessage = delegate { };

        /// <summary>
        /// sends a message to subscriptions
        /// </summary>
        public static void Receive(uint channel, T message)
        {
            OnRoutedMessage(channel, message);
        }

        /// <summary>
        /// sends a message to subscriptions
        /// </summary>
        public static void Receive(T message)
        {
            OnMessage(message);
        }

        /// <summary>
        /// Adds a route
        /// </summary>
        public static void Subscribe(MessageDelegate handler)
        {
            OnMessage += handler;
        }

        /// <summary>
        /// removes a route
        /// </summary>
        public static void Unsubscribe(MessageDelegate handler)
        {
            OnMessage -= handler;
        }

        /// <summary>
        /// Adds a route
        /// </summary>
        public static void Subscribe(RoutedMessageDelegate handler)
        {
            OnRoutedMessage += handler;
        }

        /// <summary>
        /// removes a route
        /// </summary>
        public static void Unsubscribe(RoutedMessageDelegate handler)
        {
            OnRoutedMessage -= handler;
        }
    }

    /// <summary>
    /// NonGeneric Message broadcaster
    /// </summary>
    public static class MessageBroker
    {
        static Dictionary<Type, Action<ushort, object>> _routedcache = new Dictionary<Type, Action<ushort, object>>();
        static Dictionary<Type, Action<object>> _cache = new Dictionary<Type, Action<object>>();
        
        /// <summary>
        /// Notifies listeners of a new message
        /// </summary>
        public static void Publish(ushort channel, object message, Type type)
        {
            if (!_routedcache.ContainsKey(type))
            {
                var info = typeof(MessageBroker<>).MakeGenericType(type).GetMethod("Publish");
                var func = (Action<ushort, object>)Delegate.CreateDelegate(typeof(Action<ushort, object>), info);
                _routedcache.Add(type, func);
            }

            _routedcache[type](channel, message);
        }

        /// <summary>
        /// Notifies listeners of a new message
        /// </summary>
        public static void Publish<T>(ushort channel, T message)
        {
            MessageBroker<T>.Receive(channel, message);
        }

        /// <summary>
        /// Notifies listeners of a new message
        /// </summary>
        public static void Publish(object message, Type type)
        {
            if (!_cache.ContainsKey(type))
            {
                var info = typeof(MessageBroker<>).MakeGenericType(type).GetMethod("Publish");
                var func = (Action<object>)Delegate.CreateDelegate(typeof(Action<object>), info);
                _cache.Add(type, func);
            }

            _cache[type](message);
        }

        /// <summary>
        /// Notifies listeners of a new message
        /// </summary>
        public static void Publish<T>(T message)
        {
            MessageBroker<T>.Receive(message);
        }
    }
}