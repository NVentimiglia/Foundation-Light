// Nicholas Ventimiglia 2016-08-28

using System;
using System.Collections.Generic;

namespace Foundation.Architecture
{
    /// <summary>
    ///     API contract for the injection service
    /// </summary>
    public interface IInjectService
    {
        #region Reflection

        /// <summary>
        ///     Injects dependencies into the object using reflection and the Inject annotation
        /// </summary>
        /// <remarks>
        ///     Reflection, Expensive
        /// </remarks>
        /// <param name="instance">the instance to find dependencies for</param>
        void InjectInto(object instance);

        #endregion

        #region Print

        /// <summary>
        ///     Prints the contents of the container
        /// </summary>
        IEnumerable<string> Print();

        #endregion

        #region RegisterTransient

        /// <summary>
        ///     Registers a new service. Transient is a new instance per get.
        /// </summary>
        void RegisterTransient<TInterface, TInstance>() where TInstance : class, new();

        /// <summary>
        ///     Registers a new service. Transient is a new instance per get.
        /// </summary>
        void RegisterTransient<TInstance>() where TInstance : class, new();

        /// <summary>
        ///     Registers a new service. Transient is a new instance per get.
        /// </summary>
        void RegisterTransient<TInterface, TInstance>(Func<TInstance> factory) where TInstance : class;

        /// <summary>
        ///     Registers a new service. Transient is a new instance per get.
        /// </summary>
        void RegisterTransient<TInstance>(Func<TInstance> factory) where TInstance : class;

        #endregion

        #region RegisterSingleton

        /// <summary>
        ///     Registers a new service. Singleton is a one instance forever.
        /// </summary>
        void RegisterSingleton<TInterface, TInstance>() where TInstance : class, new();

        /// <summary>
        ///     Registers a new service. Singleton is a one instance forever.
        /// </summary>
        void RegisterSingleton<TInstance>() where TInstance : class, new();

        /// <summary>
        ///     Registers a new service. Singleton is a one instance forever.
        /// </summary>
        void RegisterSingleton<TInterface, TInstance>(Func<TInstance> factory) where TInstance : class;

        /// <summary>
        ///     Registers a new service. Singleton is a one instance forever.
        /// </summary>
        void RegisterSingleton<TInstance>(Func<TInstance> factory) where TInstance : class;

        /// <summary>
        ///     Registers a new service. Singleton is a one instance forever.
        /// </summary>
        void RegisterSingleton<TInterface, TInstance>(TInstance instance) where TInstance : class;

        /// <summary>
        ///     Registers a new service. Singleton is a one instance forever.
        /// </summary>
        void RegisterSingleton<TInstance>(TInstance instance) where TInstance : class;

        #endregion

        #region Unregister

        /// <summary>
        ///     Removes a reference from the container
        /// </summary>
        bool Unregister<TInterface>() where TInterface : class, new();

        /// <summary>
        ///     Clears the container
        /// </summary>
        void UnregisterAll();

        #endregion

        #region Get

        /// <summary>
        ///     Get an instance from the container
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <returns></returns>
        TInterface Get<TInterface>() where TInterface : class;

        /// <summary>
        ///     Get an instance from the container
        /// </summary>
        /// <returns></returns>
        object Get(Type type);

        #endregion
    }
}