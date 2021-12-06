using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Kysect.Centum.Caches
{
    internal static class CastCache
    {
        private static readonly ConcurrentDictionary<Type, MethodInfo> Casts = new ConcurrentDictionary<Type, MethodInfo>();
        private static readonly MethodInfo GenericMethod = InfosCache
            .GetMethodInfos(typeof(CastCache))
            .Single(m => m.Name.Equals(nameof(ExecuteCast)));

        public static object? CastTo(this object value, Type type)
            => Casts
                .GetOrAdd(type, _ => GenericMethod.MakeGenericMethod(type))
                .Invoke(null, new[] { value });

        private static object? ExecuteCast<T>(object value) where T : class
            => value as T;
    }
}