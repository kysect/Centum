using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kysect.Centum.Extensions;

namespace Kysect.Centum.Fields.Contracts
{
    internal static class EntityContractLocator
    {
        private static readonly ConcurrentDictionary<Type, Type> Contracts;

        static EntityContractLocator()
        {
            Dictionary<Type, Type> dict = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => a.FullName?.StartsWith("Kysect.Centum") ?? false)
                .SelectMany(a => a.DefinedTypes)
                .Where(TypeFilter)
                .ToDictionary(KeyExtractor, t => t.AsType());

            Contracts = new ConcurrentDictionary<Type, Type>(dict);
        }

        public static IEntityContract? GetContract<TEntity>()
            => GetContract(typeof(TEntity));

        public static IEntityContract? GetContract(Type typeKey)
        {
            return Contracts.TryGetValue(typeKey, out Type? type)
                ? Activator.CreateInstance(type.ThrowIfNull(nameof(type))) as IEntityContract
                : null;
        }

        private static bool TypeFilter(Type type)
        {
            if (type is not { IsAbstract: false, IsInterface: false })
                return false;

            Type[] interfaces = type.GetInterfaces();
            if (interfaces.Length is not 2)
                return false;

            Type? i = interfaces.SingleOrDefault(t => t.IsGenericType);

            return i is not null && i.GetGenericTypeDefinition().IsAssignableTo(typeof(IEntityContract<>));
        }

        private static Type KeyExtractor(TypeInfo type)
            => type.GetInterfaces().Single(t => t.IsGenericType).GetGenericArguments().Single();
    }
}