// Nicholas Ventimiglia 2016-09-06

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Foundation.Architecture
{
    /// <summary>
    /// Generic Message broadcaster with support for filtering based on specific handlers
    /// </summary>
    /// <remarks>
    /// E.G. Send Message to Game Object
    /// </remarks>
    /// <typeparam name="TRoute">The type of route key, generally string or game object</typeparam>
    /// <typeparam name="TMessage">the type of message being raised</typeparam>
    public static class ObjectEvents<TRoute, TMessage> where TMessage : class where TRoute : class
    {
        /// <summary>
        /// Handler Signature
        /// </summary>
        /// <param name="message"></param>
        public delegate void MessageDelegate(TMessage message);

        /// <summary>
        /// All Listeners / Observers
        /// </summary>
        static readonly Dictionary<TRoute, MessageDelegate> _listeners = new Dictionary<TRoute, MessageDelegate>();

        /// <summary>
        /// sends a message to subscriptions
        /// </summary>
        public static void Publish(TRoute route, TMessage message)
        {
            if (_listeners.ContainsKey(route))
            {
                _listeners[route](message);
            }
        }

        /// <summary>
        /// sends a message to subscriptions
        /// </summary>
        public static void Publish(object route, object message)
        {
            if (_listeners.ContainsKey(route as TRoute))
            {
                _listeners[route as TRoute](message as TMessage);
            }
        }

        /// <summary>
        /// Adds a route
        /// </summary>
        public static void Subscribe(TRoute route, MessageDelegate handler)
        {
            if (!_listeners.ContainsKey(route))
            {
                _listeners.Add(route, delegate { });
            }
            _listeners[route] = Delegate.Combine(_listeners[route], handler) as MessageDelegate;
        }

        /// <summary>
        /// removes a handler
        /// </summary>
        public static void Unsubscribe(TRoute route, MessageDelegate handler)
        {
            if (_listeners.ContainsKey(route))
            {
                _listeners[route] = Delegate.Remove(_listeners[route], handler) as MessageDelegate;
            }
        }

        /// <summary>
        /// removes all handlers
        /// </summary>
        public static void Unsubscribe(TRoute route)
        {
            if (_listeners.ContainsKey(route))
            {
                _listeners.Remove(route);
            }
        }


        /// <summary>
        /// removes all handlers
        /// </summary>
        public static void Clear()
        {
            _listeners.Clear();
        }
    }

    /// <summary>
    /// NonGeneric Message broadcaster
    /// </summary>
    public static class ObjectEvents
    {
        static Dictionary<Type, Dictionary<Type, Delegate>> _cache = new Dictionary<Type, Dictionary<Type, Delegate>>();

        /// <summary>
        /// Notifies listeners of a new message
        /// </summary>
        public static void Publish(object route, object message, Type routeType, Type messageType)
        {
            if (!_cache.ContainsKey(routeType))
            {
                _cache.Add(routeType, new Dictionary<Type, Delegate>());
            }

            var inner = _cache[routeType];

            if (!inner.ContainsKey(messageType))
            {
                var info = typeof(ObjectEvents<,>).MakeGenericType(routeType, messageType);
                var pType = typeof(Action<object, object>);

#if CORE
                var func = info.GetMethod("Publish").CreateDelegate(pType, null);
#else
                var func = Delegate.CreateDelegate(pType, info, "Publish");
#endif
                inner.Add(messageType, func);
            }

            (inner[messageType] as Action<object, object>).Invoke(route, message);
        }

        /// <summary>
        /// Notifies listeners of a new message
        /// </summary>
        public static void Publish<TRoute, TMessage>(TRoute route, TMessage message) where TMessage : class
            where TRoute : class
        {
            ObjectEvents<TRoute, TMessage>.Publish(route, message);
        }
    }
}