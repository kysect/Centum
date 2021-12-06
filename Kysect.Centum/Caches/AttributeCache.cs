using System;
using System.Collections.Concurrent;
using System.Reflection;
using Kysect.Centum.Extensions;

namespace Kysect.Centum.Caches
{
    internal static class AttributeCache
    {
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, Attribute>> TypeAttributes = new();

        public static TAttribute GetTypeAttribute<TAttribute>(Type type)
            where TAttribute : Attribute
        {
            return (TAttribute) TypeAttributes
                .GetOrAdd(type, _ => new ConcurrentDictionary<Type, Attribute>())
                .GetOrAdd(typeof(TAttribute), _ => type.GetCustomAttribute<TAttribute>(true).ThrowIfNull(nameof(TAttribute)));
        }
    }
}