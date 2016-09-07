// Nicholas Ventimiglia 2016-09-05
using System;
using System.Collections.Generic;

namespace Foundation.Architecture
{
    /// <summary>
    /// Generic Message broadcaster
    /// </summary>
    /// <remarks>
    /// Global Message Broker. Routed by Type
    /// </remarks>
    /// <typeparam name="TMessage">the type of message being raised</typeparam>
    public static class DomainEvents<TMessage> where TMessage : class
    {
        /// <summary>
        /// Handler Signature
        /// </summary>
        /// <param name="message"></param>
        public delegate void MessageDelegate(TMessage message);
        
        /// <summary>
        /// Event
        /// </summary>
        public static event MessageDelegate OnMessage = delegate { };
      
        /// <summary>
        /// sends a message to subscriptions
        /// </summary>
        public static void Publish(TMessage message)
        {
            OnMessage(message);
        }

        /// <summary>
        /// sends a message to subscriptions
        /// </summary>
        public static void Publish(object message)
        {
            OnMessage(message as TMessage);
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
    }

    /// <summary>
    /// NonGeneric Message broadcaster
    /// </summary>
    public static class DomainEvents
    {
        static Dictionary<Type, Delegate> _cache = new Dictionary<Type, Delegate>();
        
        /// <summary>
        /// Notifies listeners of a new message
        /// </summary>
        public static void Publish(object message, Type messageType)
        {
            if (!_cache.ContainsKey(messageType))
            {
                var info = typeof(DomainEvents<>).MakeGenericType(messageType);
                var pType = typeof(Action<object>);
                var func = Delegate.CreateDelegate(pType, info, "Publish");
                _cache.Add(messageType, func);
            }

          (_cache[messageType] as Action<object>).Invoke(message);
        }

        /// <summary>
        /// Notifies listeners of a new message
        /// </summary>
        public static void Publish<TMessage>(TMessage message) where TMessage : class 
        {
            DomainEvents<TMessage>.Publish(message);
        }
    }
}
