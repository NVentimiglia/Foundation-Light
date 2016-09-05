// Nicholas Ventimiglia 2016-09-05
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Foundation.Architecture
{
    /// <summary>
    ///     Default Inject Service
    /// </summary>
    public class InjectService : IInjectService
    {
        /// <summary>
        /// Static Default Instance
        /// </summary>
        public static readonly IInjectService Instance = new InjectService();

        // RegisterTransient

        public void RegisterTransient<TInterface, TInstance>() where TInstance : class, new()
        {
            Container.Add(typeof(TInterface), new InjectReference(DefaultFactory<TInstance>()));
        }

        public void RegisterTransient<TInstance>() where TInstance : class, new()
        {
            Container.Add(typeof(TInstance), new InjectReference(DefaultFactory<TInstance>()));
        }

        public void RegisterTransient<TInterface, TInstance>(Func<TInstance> factory) where TInstance : class
        {
            Container.Add(typeof(TInterface), new InjectReference(DefaultFactory(factory)));
        }

        public void RegisterTransient<TInstance>(Func<TInstance> factory) where TInstance : class
        {
            Container.Add(typeof(TInstance), new InjectReference(DefaultFactory(factory)));
        }

        // RegisterSingleton

        public void RegisterSingleton<TInterface, TInstance>() where TInstance : class, new()
        {
            Container.Add(typeof(TInterface), new InjectReference(DefaultFactory<TInstance>(), true));
        }

        public void RegisterSingleton<TInstance>() where TInstance : class, new()
        {
            Container.Add(typeof(TInstance), new InjectReference(DefaultFactory<TInstance>(), true));
        }

        public void RegisterSingleton<TInterface, TInstance>(Func<TInstance> factory) where TInstance : class
        {
            Container.Add(typeof(TInterface), new InjectReference(DefaultFactory(factory), true));
        }

        public void RegisterSingleton<TInstance>(Func<TInstance> factory) where TInstance : class
        {
            Container.Add(typeof(TInstance), new InjectReference(DefaultFactory(factory), true));
        }

        public void RegisterSingleton<TInterface, TInstance>(TInstance instance) where TInstance : class
        {
            Container.Add(typeof(TInterface), new InjectReference(instance, true));
        }

        public void RegisterSingleton<TInstance>(TInstance instance) where TInstance : class
        {
            Container.Add(typeof(TInstance), new InjectReference(instance, true));
        }

        // Unregister

        public bool Unregister<TInterface>() where TInterface : class, new()
        {
            return Container.Remove(typeof(TInterface));
        }

        public void UnregisterAll()
        {
            Container.Clear();
        }

        // Get

        public TInterface Get<TInterface>() where TInterface : class
        {
            return Get(typeof(TInterface)) as TInterface;
        }

        public object Get(Type interfaceType)
        {
            if (!Container.ContainsKey(interfaceType))
                return null;

            var refer = Container[interfaceType];
            if (refer.Instance != null)
                return refer.Instance;

            if (refer.Factory != null)
            {
                var inst = refer.Factory();

                if (refer.IsStatic)
                {
                    refer.Instance = inst;
                    refer.Factory = null;
                }
                return inst;
            }

            return null;
        }

        // Print

        public IEnumerable<string> Print()
        {
            foreach (var reference in Container)
                if (reference.Value.Instance != null)
                    yield return string.Format("{0} with {1} instance", reference.Key, reference.Value.Instance);
                else if (reference.Value.Factory != null)
                    yield return string.Format("{0} with factory", reference.Key);
        }

        // Reflection

        public void InjectInto(object instance)
        {
            //TODO Reflector Cache
            var allFields = instance
                .GetType()
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(o => o.GetCustomAttributes(typeof(InjectAttribute), true).Length > 0)
                .ToArray();

            FieldInfo field = null;

            for (var i = 0; i < allFields.Length; i++)
                try
                {
                    field = allFields[i];

                    var type = field.FieldType;

                    field.SetValue(instance, Get(type));
                }
                catch (Exception ex)
                {
                    if (field != null)
                        throw new Exception("Failed to inject into " + field.Name + "(" + field.FieldType + ")", ex);
                    else
                        throw new Exception("Failed to inject into unknown field", ex);
                }
        }

        #region Private

        private class InjectReference
        {
            public Func<object> Factory;
            public object Instance;
            public readonly bool IsStatic;

            public InjectReference(object instance, bool isStatic = false)
            {
                Instance = instance;
                IsStatic = isStatic;
            }

            public InjectReference(Func<object> factory, bool isStatic = false)
            {
                Factory = factory;
                IsStatic = isStatic;
            }
        }

        private readonly Dictionary<Type, InjectReference> Container = new Dictionary<Type, InjectReference>();

        private Func<TInstance> DefaultFactory<TInstance>() where TInstance : class, new()
        {
            return () =>
            {
                var instance = new TInstance();
                InjectInto(instance);
                return instance;
            };
        }

        private Func<TInstance> DefaultFactory<TInstance>(Func<TInstance> factory) where TInstance : class
        {
            return () =>
            {
                var instance = factory();
                InjectInto(instance);
                return instance;
            };
        }

        #endregion
    }
}