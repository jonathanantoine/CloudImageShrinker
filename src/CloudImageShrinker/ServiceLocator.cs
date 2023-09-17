using System;
using System.Collections.Generic;

namespace CloudImageShrinker
{
    public class ServiceLocator
    {
        private static readonly Lazy<ServiceLocator> _Instance = new Lazy<ServiceLocator>(() => new ServiceLocator());

        protected ServiceLocator()
        {
        }

        protected static ServiceLocator Instance => _Instance.Value;

        private static readonly Dictionary<Type, object> _Services = new Dictionary<Type, object>();

        public static void RegisterSingleton<T>(T implementation)
        {
            lock (_Instance)
            {
                _Services[typeof(T)] = implementation;
            }
        }

        public static T Resolve<T>()
        {
            lock (_Instance)
            {
                return (T) _Services[typeof(T)];
            }
        }

        /// <summary>
        /// Return the service or null if not already registered.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The service or null if not already registered.</returns>
        public static T TryResolve<T>() where T : class
        {
            lock (_Instance)
            {
                if (_Services.TryGetValue(typeof(T), out object service))
                {
                    return service as T;
                }
            }

            return null;
        }

        public static bool TryResolve<T>(out T service) where T : class
        {
            lock (_Instance)
            {
                if (_Services.TryGetValue(typeof(T), out object serviceAsObject))
                {
                    service = serviceAsObject as T;
                    return true;
                }
            }

            service = null;
            return false;
        }
    }

}