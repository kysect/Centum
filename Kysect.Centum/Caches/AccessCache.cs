using System;
using System.Collections.Concurrent;

namespace Kysect.Centum.Caches
{
    internal static class AccessCache
    {
        private static readonly ConcurrentDictionary<Type, TypeAccessCache> Caches = new();

        public static Action<object, object?> SetterFunc(Type type, string name)
            => Caches
                .GetOrAdd(type, t => new TypeAccessCache(t))
                .SetterFunc(name);

        public static Func<object, object?> GetterFunc(Type type, string name)
            => Caches
                .GetOrAdd(type, t => new TypeAccessCache(t))
                .GetterFunc(name);
        
        public static Func<object, object[], object?> Function(Type type, string name)
            => Caches
                .GetOrAdd(type, t => new TypeAccessCache(t))
                .Function(name);
    }
}