using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Kysect.Centum.Caches
{
    internal static class InfosCache
    {
        private const BindingFlags Flags = BindingFlags.NonPublic |
                                           BindingFlags.Instance |
                                           BindingFlags.Public |
                                           BindingFlags.Static |
                                           BindingFlags.Instance;

        private static readonly ConcurrentDictionary<Type, IReadOnlyCollection<PropertyInfo>> PropertiesInfoCache = new();
        private static readonly ConcurrentDictionary<Type, IReadOnlyCollection<FieldInfo>> FieldInfosCache = new();
        private static readonly ConcurrentDictionary<Type, IReadOnlyCollection<MethodInfo>> MethodInfos = new();
        private static readonly ConcurrentDictionary<GenericMethodInfoKey, MethodInfo> GenericMethodInfos = new();
        private static readonly ConcurrentDictionary<ConstructorInfoKey, IReadOnlyCollection<ConstructorInfo>> ConstructorInfos = new();

        public static IReadOnlyCollection<PropertyInfo> GetPropertiesInfo(Type type)
            => PropertiesInfoCache.GetOrAdd(type, _ => type.GetProperties(Flags));

        public static IReadOnlyCollection<FieldInfo> GetFieldInfos(Type type)
            => FieldInfosCache.GetOrAdd(type, _ => type.GetFields(Flags));

        public static IReadOnlyCollection<MethodInfo> GetMethodInfos(Type type)
            => MethodInfos.GetOrAdd(type, _ => type.GetMethods(Flags));

        public static MethodInfo GetGenericMethodInfo(this MethodInfo method, params Type[] genericArgs)
            => GenericMethodInfos.GetOrAdd(
                new GenericMethodInfoKey(method, genericArgs),
                k => k.MethodInfo
                    .MakeGenericMethod(k.GenericArgs));

        public static IReadOnlyCollection<ConstructorInfo> GetConstructorInfos(Type type, params Type[] genericArgs)
            => ConstructorInfos.GetOrAdd(
                new ConstructorInfoKey(type, genericArgs),
                k => k.DeclaringType
                    .MakeGenericType(genericArgs)
                    .GetConstructors(Flags));

        private sealed record ConstructorInfoKey(Type DeclaringType, Type[] GenericArgs);
        private sealed record GenericMethodInfoKey(MethodInfo MethodInfo, Type[] GenericArgs);
    }
}