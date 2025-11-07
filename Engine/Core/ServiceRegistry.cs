using System;
using System.Collections.Concurrent;

namespace Engine.Core
{
    public static class ServiceRegistry
    {
        private static readonly ConcurrentDictionary<Type, object> _map = new();

        public static void Clear() => _map.Clear();

        public static void Register<T>(T instance) where T : class
        {
            if (instance is null) throw new ArgumentNullException(nameof(instance));
            _map[typeof(T)] = instance;
        }

        public static bool TryGet<T>(out T? instance) where T : class
        {
            if (_map.TryGetValue(typeof(T), out var obj) && obj is T typed)
            {
                instance = typed;
                return true;
            }
            instance = null;
            return false;
        }

        /// <summary>
        /// Versucht einen Service zu holen, gibt null zurück falls nicht registriert
        /// </summary>
        public static T? TryGet<T>() where T : class
        {
            return TryGet<T>(out var instance) ? instance : null;
        }

        public static T Get<T>() where T : class
        {
            if (TryGet<T>(out var inst) && inst != null) return inst;
            throw new InvalidOperationException($"Service not registered: {typeof(T).FullName}");
        }
    }
}
